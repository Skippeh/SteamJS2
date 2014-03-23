using System;

namespace SteamJS2.JavascriptBindings
{
    public class JavascriptException : Exception
    {
        private string message;

        public JavascriptException() : this("") { }
        public JavascriptException(string message)
        {
            this.message = message;
        }

        public override string Message
        {
            get { return message; }
        }
    }
}