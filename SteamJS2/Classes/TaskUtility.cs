using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xilium.CefGlue;

namespace SteamJS2
{
    public static class TaskUtility
    {
        private class RunningTask
        {
            public CefThreadId ThreadId;
            public Func<object> Task;
            public Action<object[]> Callback;
            public bool Done;
            public object Result;

            public RunningTask(CefThreadId threadId, Func<object> task, Action<object[]> callback)
            {
                ThreadId = threadId;
                Task = task;
                Callback = callback;
                Done = false;
            }

            public void DoWorkAsync()
            {
                var bgWorker = new BackgroundWorker();
                bgWorker.DoWork += (sender, args) => args.Result = Task();
                bgWorker.RunWorkerCompleted += (sender, args) =>
                {
                    Result = args.Result;
                    Done = true;
                };

                bgWorker.RunWorkerAsync();
            }
        }

        private static List<RunningTask> tasks = new List<RunningTask>();  

        public static void PostTask(CefThreadId threadId, Func<object> task, Action<object[]> callback)
        {
            var runningTask = new RunningTask(threadId, task, callback);
            tasks.Add(runningTask);
            runningTask.DoWorkAsync();
        }

        public static void CheckTasks()
        {
            foreach (var task in tasks.ToArray().Where(task => task.Done))
            {
                tasks.Remove(task);
                CefRuntime.PostTask(task.ThreadId, new CefTaskTaskUtility(task.Result, task.Callback));
            }
        }

        private class CefTaskTaskUtility : CefTask
        {
            private readonly object result;
            private readonly Action<object[]> callback;
        
            public CefTaskTaskUtility(object result, Action<object[]> callback)
            {
                this.result = result;
                this.callback = callback;
            }
        
            protected override void Execute()
            {
                if (result != null && result.GetType().IsArray)
                    callback.Invoke((object[])result);
                else
                    callback.Invoke(new[] {result});
            }
        }
    }
}