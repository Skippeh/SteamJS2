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
            LoadJavascript(context);
        }

        private void LoadJavascript(CefV8Context context)
        {
            var global = context.GetGlobal();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttributes(typeof(JavascriptBindingAttribute), true).Length > 0))
            {
                var attribute = (JavascriptBindingAttribute)type.GetCustomAttributes(typeof(JavascriptBindingAttribute), true)[0];
                object instance = null;
                if (!type.IsAbstract) // If not static, create instance.
                    instance = Activator.CreateInstance(type);

                var jsObject = JSGC.Register(CefV8Value.CreateObject(null));

                foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public))
                {
                    jsObject.SetValue(field.Name.ToCamelCase(), V8Utility.ToV8Value(field.GetValue(instance)), CefV8PropertyAttribute.ReadOnly);
                }

                foreach (var method in type.GetMethods())
                {
                    jsObject.SetValue(method.Name.ToCamelCase(), CefV8Value.CreateFunction(method.Name, new CefV8HandlerMethodInfo(instance, method, context.GetBrowser())), CefV8PropertyAttribute.ReadOnly);
                }

                global.SetValue(attribute.ObjectName.ToCamelCase(), jsObject, CefV8PropertyAttribute.ReadOnly);
            }
        }
    }
}