using System;
using Xilium.CefGlue;

namespace SteamJS2
{
    public class FunctionUserData : CefUserData
    {
        public CefV8Value Value;
        public object Instance;

        public FunctionUserData(object instance, CefV8Value value)
        {
            Instance = instance;
            Value = value;
        }

        ~FunctionUserData()
        {
            Console.WriteLine("~FunctionUserData " + Instance);
        }
    }
}