﻿namespace CefSharp
{
    public enum KeyType
    {
        RawKeyDown,
        KeyDown,
        KeyUp,
        Char,
    };

    public interface IKeyboardHandler
    {
        bool OnKeyEvent(IWebBrowser browser, KeyType type, int code, int modifiers, bool isSystemKey);
    }
}
