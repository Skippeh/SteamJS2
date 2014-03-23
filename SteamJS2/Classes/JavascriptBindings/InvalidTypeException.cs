using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamJS2.JavascriptBindings;

namespace SteamJS2.Classes.JavascriptBindings
{
    public class InvalidTypeException : JavascriptException
    {
        public string Parameter;

        public InvalidTypeException(string parameter)
        {
            Parameter = parameter;
        }

        public override string Message
        {
            get { return "Parameter '" + Parameter + "' is not the correct type."; }
        }
    }
}