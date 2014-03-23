using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SteamJS2.Classes;
using SteamJS2.Classes.JavascriptBindings;
using Xilium.CefGlue;

namespace SteamJS2.JavascriptBindings.Implementations
{
    [JavascriptBinding("steamjs")]
    public class Utils
    {
        public string getPath(string relativePath)
        {
            return Path.GetFullPath(relativePath);
        }

        public object test(int returnThis)
        {
            return returnThis;
        }

        public void testCallback(object[] test, CefV8Value callback, CefBrowser browser)
        {
            var context = browser.GetMainFrame().V8Context;

            if (!callback.IsFunction)
                throw new InvalidTypeException("callback");

            TaskUtility.PostTask(CefThreadId.Renderer, () =>
            {
                Thread.Sleep(2000);
                return new object[] {test};
            },
            objects =>
            {
                context.Enter();
                CefV8Value[] v8ValueArray = V8Utility.ToV8ValueArray(objects);
                callback.ExecuteFunctionWithContext(context, null, v8ValueArray);
                context.Exit();
            });
        }

        public List<int> testList()
        {
            return new List<int>() {0, 1, 2, 3};
        }
    }
}