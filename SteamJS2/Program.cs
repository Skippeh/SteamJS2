using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Steam4NetApi;
using SteamJS2.Forms;
using SteamJS2.JavascriptBindings.Implementations;
using Xilium.CefGlue;

namespace SteamJS2
{
    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            if (!OpenSteamApi.LoadSteamClient())
            {
                return 0;
            }

            CefRuntime.Load();
            var mainArgs = new CefMainArgs(args);
            var app = new MyApp();

            var settings = new CefSettings()
            {
                SingleProcess = true,
                MultiThreadedMessageLoop = false,
                RemoteDebuggingPort = new Random().Next(10000, short.MaxValue),
                LogSeverity = CefLogSeverity.ErrorReport
            };

            CefRuntime.Initialize(mainArgs, settings, app);
            SteamEventHandler.StartListening();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Idle += (sender, eventArgs) =>
            {
                CefRuntime.DoMessageLoopWork();
                TaskUtility.CheckTasks();
            };

            Application.Run(new ChromiumForm("file:///" + Path.GetFullPath("html/index.html")));

            CefRuntime.Shutdown();
            OpenSteamApi.Shutdown();
            return 0;
        }
    }
}