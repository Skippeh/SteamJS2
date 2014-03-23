﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamJS2.JavascriptBindings;

namespace SteamJS2.Classes.JavascriptBindings.Implementations
{
    [JavascriptBinding("gc")]
    public class _JSGC
    {
        public void Collect()
        {
            GC.Collect();
        }
    }
}