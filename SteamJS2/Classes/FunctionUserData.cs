using System;
using Xilium.CefGlue;

namespace SteamJS2
{
    public class FunctionUserData : CefUserData
    {
        public object Instance;

        public FunctionUserData(object instance)
        {
            Instance = instance;
        }
    }
}