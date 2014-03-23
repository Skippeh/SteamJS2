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
        //public static bool IsBrowserProcess;

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

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);
            if (exitCode != -1)
                return exitCode;

            var settings = new CefSettings()
            {
                SingleProcess = true,
                MultiThreadedMessageLoop = false,
                RemoteDebuggingPort = new Random().Next(10000, short.MaxValue),
                LogSeverity = CefLogSeverity.ErrorReport
            };

            CefRuntime.Initialize(mainArgs, settings, app);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Idle += (sender, eventArgs) =>
            {
                CefRuntime.DoMessageLoopWork();
                TaskUtility.CheckTasks();
                JSGC.Update();
            };

            Application.Run(new ChromiumForm("file:///" + Path.GetFullPath("html/index.html")));

            CefRuntime.Shutdown();
            return 0;
        }
    }
}