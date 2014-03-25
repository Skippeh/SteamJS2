using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SteamJS2.Extensions;
using SteamJS2.JavascriptBindings;
using Xilium.CefGlue;

namespace SteamJS2
{
    internal class MyRenderProcessHandler : CefRenderProcessHandler
    {
        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            Console.WriteLine("ContextCreated");
            LoadJavascript(context);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            Console.WriteLine("OnContextReleased");
        }

        private void LoadJavascript(CefV8Context context)
        {
            var global = context.GetGlobal();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttributes(typeof (JavascriptBindingAttribute), true).Length > 0))
            {
                var attribute = (JavascriptBindingAttribute)type.GetCustomAttributes(typeof (JavascriptBindingAttribute), true)[0];
                object instance = null;

                if (!type.IsAbstract) // If not static, create instance.
                    instance = Activator.CreateInstance(type);

                var jsObject = V8Utility.CreateV8Object(instance ?? type);

                global.SetValue(attribute.ObjectName.ToCamelCase(), jsObject, CefV8PropertyAttribute.ReadOnly);
            }
        }
    }
}