﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SteamJS2.Extensions;
using SteamJS2.JavascriptBindings;
using Xilium.CefGlue;

namespace SteamJS2
{
    /// <summary>
    /// Only usable in the renderer thread!
    /// </summary>
    public static class V8Utility
    {
        private static readonly HashSet<Type> numericTypes = new HashSet<Type>
        {
            typeof (Byte), typeof (Decimal),
            typeof (Double), typeof (Int16),
            typeof (Int32), typeof (Int64),
            typeof (UInt16), typeof (UInt32),
            typeof (UInt64), typeof (SByte)
        };

        public static CefV8Value[] ToV8ValueArray(object[] objects)
        {
            var v8Values = new CefV8Value[objects.Length];

            for (int i = 0; i < objects.Length; ++i)
            {
                v8Values[i] = ToV8Value(objects[i]);
            }

            return v8Values;
        }

        public static CefV8Value ToV8Value(object obj)
        {
            if (obj == null)
                return CefV8Value.CreateNull();

            var v8Object = obj as CefV8Value;
            if (v8Object != null)
                return JSGC.Register(v8Object);

            var type = obj.GetType();

            if (type.IsArray)
            {
                var objArray = new object[((Array)obj).Length];

                for (int i = 0; i < objArray.Length; ++i)
                    objArray[i] = ((Array)obj).GetValue(i);

                var jsArray = CefV8Value.CreateArray(objArray.Length);
                for (int i = 0; i < objArray.Length; ++i)
                {
                    jsArray.SetValue(i, ToV8Value(objArray[i]));
                }

                return jsArray;
            }

            if (numericTypes.Contains(type))
                return JSGC.Register(CefV8Value.CreateDouble(Convert.ToDouble(obj)));

            if (type.IsEnum)
                return JSGC.Register(CefV8Value.CreateInt(Convert.ToInt32(obj)));

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return JSGC.Register(CefV8Value.CreateBool((bool)obj));
                case TypeCode.Char:
                case TypeCode.String:
                    return JSGC.Register(CefV8Value.CreateString(obj.ToString()));
                case TypeCode.DateTime:
                    return JSGC.Register(CefV8Value.CreateDate((DateTime)obj));
                case TypeCode.Empty:
                    return JSGC.Register(CefV8Value.CreateUndefined());
                case TypeCode.DBNull:
                    return JSGC.Register(CefV8Value.CreateNull());
                case TypeCode.Object:
                    {
                        var jsObject = CefV8Value.CreateObject(null);
                        foreach (var field in type.GetFields())
                        {
                            jsObject.SetValue(field.Name.ToCamelCase(), ToV8Value(field.GetValue(obj)), CefV8PropertyAttribute.ReadOnly);
                        }

                        foreach (var method in type.GetMethods())
                        {
                            jsObject.SetValue(method.Name.ToCamelCase(), CefV8Value.CreateFunction(method.Name, new CefV8HandlerMethodInfo(obj, method, CefV8Context.GetCurrentContext().GetBrowser())), CefV8PropertyAttribute.ReadOnly);
                        }

                        return JSGC.Register(jsObject);
                    }
            }

            throw new NotImplementedException("Unable to create V8Value from type " + type.Name + "(typecode: " + Type.GetTypeCode(type) + ")");
        }

        public static object GetCLRObject(CefV8Value v8Value)
        {
            if (!v8Value.IsValid)
                throw new Exception("CefV8Value.IsValid is false!");

            var v8ValuesList = new List<CefV8Value>();

            if (!v8Value.IsArray)
                v8ValuesList.Add(v8Value);
            else
            {
                var arrayLength = v8Value.GetArrayLength();
                for (int i = 0; i < arrayLength; ++i)
                    v8ValuesList.Add(v8Value.GetValue(i));
            }

            var result = new List<object>();
            foreach (var value in v8ValuesList)
            {
                if (value.IsBool)
                    result.Add(value.GetBoolValue());
                if (value.IsDate)
                    result.Add(value.GetDateValue());
                if (value.IsDouble)
                    result.Add(value.GetDoubleValue());
                if (value.IsFunction)
                    return v8Value;
                if (value.IsNull || value.IsUndefined)
                    result.Add(null);
                if (value.IsString)
                    result.Add(value.GetStringValue());
                if (value.IsObject)
                {
                    // Todo: Return dictionary<object, object>.
                    throw new JavascriptException("Can not convert javascript object to CLR type.");
                }
            }

            if (v8Value.IsArray)
                return result.ToArray();

            return result[0];
        }
    }
}