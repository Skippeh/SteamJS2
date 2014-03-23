using System;
using System.Collections.Generic;
using Xilium.CefGlue;

namespace SteamJS2
{
    public static class JSGC
    {
        private static List<CefV8Value> registeredObjects = new List<CefV8Value>();

        private static TimeSpan interval = new TimeSpan(0, 0, 0, 1);
        private static DateTime lastUpdate = DateTime.Now - interval;

        public static CefV8Value Register(CefV8Value v8Object)
        {
            registeredObjects.Add(v8Object);
            return v8Object;
        }

        public static void Cleanup()
        {

        }

        public static void Update()
        {
            if (DateTime.Now - lastUpdate > interval)
            {
                Cleanup();
                lastUpdate = DateTime.Now;
            }
        }
    }
}