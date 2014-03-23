using System;
using System.Diagnostics;
using System.Threading;
using Xilium.CefGlue;

namespace SteamJS2
{
    internal class MyApp : CefApp
    {
        private readonly MyRenderProcessHandler myProcessHandler;
        private readonly MyBrowserProcessHandler myBrowserHandler;

        public MyApp()
        {
            myProcessHandler = new MyRenderProcessHandler();
            myBrowserHandler = new MyBrowserProcessHandler();
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return myProcessHandler;
        }

        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            return myBrowserHandler;
        }
    }

    internal class MyBrowserProcessHandler : CefBrowserProcessHandler
    {
        protected override void OnContextInitialized()
        {
        }
    }
}