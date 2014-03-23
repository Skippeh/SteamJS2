using System;
using System.Collections.Generic;
using System.Net.Mime;
using Xilium.CefGlue;

namespace SteamJS2.JavascriptBindings.Implementations
{
    [JavascriptBinding("events")]
    public static class JSEvents
    {
        private static readonly Dictionary<string, List<BoundCallback>> boundEvents;
        private static readonly object lockObject = new object();

        static JSEvents()
        {
            boundEvents = new Dictionary<string, List<BoundCallback>>();
        }

        public static void Register(string name, CefV8Value callback, CefV8Value _this, CefBrowser browser)
        {
            if (!boundEvents.ContainsKey(name))
                boundEvents[name] = new List<BoundCallback>();

            boundEvents[name].Add(new BoundCallback(callback, CefV8Context.GetCurrentContext(), _this, browser));
        }

        public static bool Remove(string name, CefV8Value callback)
        {
            if (boundEvents.ContainsKey(name))
            {
                foreach (var ev in boundEvents[name])
                {
                    if (ev.FunctionCallback == callback)
                    {
                        boundEvents[name].Remove(ev);
                        return true;
                    }
                }
            }

            return false;
        }

        internal static void Execute(string name, params object[] args)
        {
            lock (lockObject)
            {
                if (!boundEvents.ContainsKey(name))
                    return;

                var toRemove = new Queue<BoundCallback>();
                foreach (var ev in boundEvents[name])
                {
                    if (!ev.IsValid || !ev.Execute(args))
                        toRemove.Enqueue(ev);
                }

                while (toRemove.Count > 0)
                    boundEvents[name].Remove(toRemove.Dequeue());
            }
        }
    }

    public class BoundCallback
    {
        public readonly CefV8Value CallbackThis;
        public readonly CefV8Value FunctionCallback;
        public readonly CefV8Context Context;
        private readonly CefBrowser browser;
        public bool IsValid { get; private set; }

        public BoundCallback(CefV8Value functionCallback, CefV8Context context, CefV8Value callbackThis, CefBrowser browser)
        {
            FunctionCallback = functionCallback;
            this.Context = context;
            this.CallbackThis = callbackThis;
            this.browser = browser;
            IsValid = context.IsValid;
        }

        public bool Execute(params object[] args)
        {
            try
            {
                return CefRuntime.PostTask(CefThreadId.Renderer, new MyCefTask(this, args));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                IsValid = false;
                return false;
            }
        }

        private class MyCefTask : CefTask
        {
            private readonly BoundCallback boundCallback;
            private readonly object[] args;
            private static readonly object lockObject = new object();

            public MyCefTask(BoundCallback boundCallback, object[] args)
            {
                this.boundCallback = boundCallback;
                this.args = args;
            }

            protected override void Execute()
            {
                lock (lockObject)
                {
                    var v8Args = new CefV8Value[args.Length];

                    boundCallback.Context.Enter();

                    for (int i = 0; i < v8Args.Length; ++i)
                    {
                        v8Args[i] = V8Utility.ToV8Value(args[i]);
                    }

                    boundCallback.FunctionCallback.ExecuteFunctionWithContext(boundCallback.Context, boundCallback.CallbackThis, v8Args);

                    foreach (var v8arg in v8Args)
                        v8arg.Dispose();

                    boundCallback.Context.Exit();
                }
            }
        }
    }
}