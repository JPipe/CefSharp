namespace CefSharp.Internals
{
    public class SubProcessProxySupport
    {
        private const string BaseAddress = "net.pipe://localhost";
        private const string ServiceName = "CefSharpSubProcessProxy";

        /// <summary>
        /// MaxReceivedMessageSize - 16MB in bytes
        /// </summary>
        public const int MaxReceivedMessageSize = 16777216;

         public static string GetServiceName(int parentProcessId, int browserId)
        {
            return string.Join("/", BaseAddress, ServiceName, parentProcessId, browserId);
        }
    }
}
