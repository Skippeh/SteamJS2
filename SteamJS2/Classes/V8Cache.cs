using System;
using System.Collections.Generic;
using System.Reflection;
using Xilium.CefGlue;

namespace SteamJS2
{
    internal static class V8Cache
    {
        private static Dictionary<Type, MethodInfo[]> methodInfos = new Dictionary<Type, MethodInfo[]>();
        private static Dictionary<Type, FieldInfo[]> fieldInfos = new Dictionary<Type, FieldInfo[]>();
        private static Dictionary<MethodInfo, CefV8Handler> methodHandlers = new Dictionary<MethodInfo, CefV8Handler>();

        public static MethodInfo[] GetMethodInfos(Type type)
        {
            if (methodInfos.ContainsKey(type))
                return methodInfos[type];

            return null;
        }

        public static MethodInfo[] SetMethodInfos(Type type, MethodInfo[] methods)
        {
            methodInfos[type] = methods;
            return methods;
        }

        public static FieldInfo[] GetFieldInfos(Type type)
        {
            if (fieldInfos.ContainsKey(type))
                return fieldInfos[type];

            return null;
        }

        public static FieldInfo[] SetFieldInfos(Type type, FieldInfo[] fields)
        {
            fieldInfos[type] = fields;
            return fields;
        }

        public static CefV8Handler GetMethodHandler(MethodInfo method)
        {
            if (methodHandlers.ContainsKey(method))
                return methodHandlers[method];

            return null;
        }

        public static CefV8Handler SetMethodHandler(MethodInfo method, CefV8Handler methodHandler)
        {
            methodHandlers[method] = methodHandler;
            return methodHandler;
        }
    }
}