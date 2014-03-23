using System;

namespace SteamJS2.JavascriptBindings
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class JavascriptBindingAttribute : Attribute
    {
        public string ObjectName;

        public JavascriptBindingAttribute(string objectName)
        {
            ObjectName = objectName;
        }
    }
}