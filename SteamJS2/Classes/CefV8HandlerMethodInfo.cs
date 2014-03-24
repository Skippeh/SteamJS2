using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using SteamJS2.Classes.JavascriptBindings;
using SteamJS2.JavascriptBindings;
using SteamJS2.JavascriptBindings.Implementations;
using Xilium.CefGlue;

namespace SteamJS2
{
    public class CefV8HandlerMethodInfo : CefV8Handler
    {
        private object instance;
        private MethodInfo method;
        private string methodHead;
        private ParameterInfo[] parameters;

        public CefV8HandlerMethodInfo(object instance, MethodInfo method)
        {
            this.instance = instance;
            this.method = method;
            methodHead = getMethodHead();
            parameters = method.GetParameters();
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            exception = null;
            returnValue = null;

            var browser = CefV8Context.GetCurrentContext().GetBrowser();

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var objArguments = new List<object>(arguments.Length);

                if (arguments.Length > parameters.Length) // More arguments given than the amount of parameters in method.
                    throw new TargetParameterCountException();

                for (int i = 0; i < arguments.Length; ++i)
                {
                    var clrObject = V8Utility.GetCLRObject(arguments[i]);
                    Type paramType = parameters[i].ParameterType;

                    if (clrObject != null && i < parameters.Length && paramType != clrObject.GetType())
                    {
                        if (!paramType.IsEnum)
                        {
                            if (!paramType.IsArray)
                            {
                                clrObject = ConvertType(clrObject, paramType);
                            }
                            else
                            {
                                var clrObjectArray = (object[])clrObject;
                                var convertedClrObject = new object[clrObjectArray.Length];

                                for (int j = 0; j < clrObjectArray.Length; ++j)
                                {
                                    convertedClrObject[j] = ConvertType(clrObjectArray[j], paramType.GetElementType());
                                }

                                clrObject = convertedClrObject;

                                if (paramType != clrObjectArray.GetType())
                                {
                                    var genericArray = Array.CreateInstance(paramType.GetElementType(), convertedClrObject.Length);

                                    for (int j = 0; j < convertedClrObject.Length; ++j)
                                    {
                                        genericArray.SetValue(convertedClrObject[j], j);
                                    }
                                    clrObject = genericArray;
                                }
                            }
                        }
                        else
                        {
                            clrObject = Convert.ChangeType(clrObject, typeof (Int32));
                            clrObject = Enum.Parse(paramType, clrObject.ToString());
                        }
                    }

                    objArguments.Add(clrObject);
                }

                if (parameters.Length > arguments.Length && parameters[arguments.Length].ParameterType == typeof (CefBrowser))
                {
                    objArguments.Add(browser);
                }

                sw.Stop();
                //Console.WriteLine(sw.Elapsed.TotalMilliseconds + "ms to convert function arguments to CLR.");

                var result = method.Invoke(instance, objArguments.ToArray());
                returnValue = V8Utility.ToV8Value(result);
            }
            catch (FormatException ex)
            {
                exception = ex.Message + " " + methodHead;
            }
            catch (InvalidTypeException ex)
            {
                exception = ex.Message + " " + methodHead;
            }
            catch (JavascriptException ex)
            {
                exception = ex.Message;
            }
            catch (TargetInvocationException ex)
            {
                exception = ex.InnerException.Message;
            }
            catch (TargetParameterCountException)
            {
                exception = "Parameter count mismatch. " + methodHead;
            }
            catch (InvalidCastException ex)
            {
                exception = ex.Message + " " + methodHead;
            }

            return true;
        }

        private object ConvertType(object obj, Type targetType)
        {
            try
            {
                return Convert.ChangeType(obj, targetType);
            }
            catch (InvalidCastException)
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(targetType);
                    return converter.ConvertFrom(obj);
                }
                catch (NotSupportedException)
                {
                    throw new InvalidCastException("Unable to cast " + obj.GetType().Name + " to " + targetType.Name);
                }
            }
            //var typeConverter = TypeDescriptor.GetConverter(targetType);
            //return typeConverter.ConvertFrom(obj);
        }

        private string getMethodHead()
        {
            var builder = new StringBuilder();

            builder.Append(method.Name + "(");

            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; ++i)
            {
                var param = parameters[i];
                var name = param.Name;
                string paramTypeName = param.ParameterType.Name;

                if (param.IsOut)
                    builder.Append("out " + name);
                else if (param.IsOptional)
                    builder.Append(paramTypeName + " " + name + " = " + (param.DefaultValue ?? "null"));
                else
                {
                    builder.Append(paramTypeName + " " + name);
                }

                if (i < parameters.Length - 1)
                    builder.Append(", ");
                else
                    builder.Append(")");
            }

            if (parameters.Length == 0)
                builder.Append(")");

            return builder.ToString();
        }
    }
}