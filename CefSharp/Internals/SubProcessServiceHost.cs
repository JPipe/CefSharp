﻿using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace CefSharp.Internals
{
    public class SubProcessServiceHost : ServiceHost
    {
        public SubProcessProxy Service { get; set; }

        public SubProcessServiceHost()
            : base(typeof(SubProcessProxy), new Uri[0])
        {
        }

        public static SubProcessServiceHost Create(int parentProcessId, int browserId)
        {
            var host = new SubProcessServiceHost();
            AddDebugBehavior(host);

            //use absultadress for hosting 
            //http://stackoverflow.com/questions/10362246/two-unique-named-pipes-conflicting-and-invalidcredentialexception
            var serviceName = SubProcessProxySupport.GetServiceName(parentProcessId, browserId);

            KillExistingServiceIfNeeded(serviceName);

            Kernel32.OutputDebugString("Setting up IJavascriptProxy using service name: " + serviceName);
            host.AddServiceEndpoint(
                typeof(ISubProcessProxy),
                new NetNamedPipeBinding { MaxReceivedMessageSize = SubProcessProxySupport.MaxReceivedMessageSize },
                new Uri(serviceName)
            );

            host.Open();
            return host;
        }

        private static void KillExistingServiceIfNeeded(string serviceName)
        {
            // It might be that there is an existing process already bound to this port. We must get rid of that one, so that the
            // endpoint address gets available for us to use.
            try
            {
                var channelFactory = new ChannelFactory<ISubProcessProxy>(
                    new NetNamedPipeBinding(),
                    new EndpointAddress(serviceName)
                    );
                channelFactory.Open(TimeSpan.FromSeconds(1));
                var javascriptProxy = channelFactory.CreateChannel();
                javascriptProxy.Terminate();
            }
            catch
            {
                // We assume errors at this point are caused by things like the endpoint not being present (which will happen in
                // the first render subprocess instance).
            }
        }

        private static void AddDebugBehavior(ServiceHostBase host)
        {
            var serviceDebugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();

            if (serviceDebugBehavior == null)
            {
                serviceDebugBehavior = new ServiceDebugBehavior
                {
                    IncludeExceptionDetailInFaults = true
                };
                host.Description.Behaviors.Add(serviceDebugBehavior);
            }
            else
            {
                serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
            }
        }
    }
}