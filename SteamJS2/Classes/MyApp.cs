using System;
using System.Diagnostics;
using System.Threading;
using Xilium.CefGlue;

namespace SteamJS2
{
    internal class MyApp : CefApp
    {
        private readonly MyRenderProcessHandler myProcessHandler;

        public MyApp()
        {
            myProcessHandler = new MyRenderProcessHandler();
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return myProcessHandler;
        }
    }
}