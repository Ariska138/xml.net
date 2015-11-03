using System;
using System.Collections;
using System.Reflection;
using System.Xml.Linq;

namespace Xml.Net
{
    public static partial class XmlConvert
    {
        private const XmlConvertOptions DefaultConvertOptions = XmlConvertOptions.None;

        private static string GetClassIdentifier(object obj)
        {
            var xmlConvertible = obj as XmlConvertible;
            if (xmlConvertible != null)
            {
                return xmlConvertible.XmlIdentifier;
            }

            var typeInfo = obj.GetType().GetTypeInfo();
            return GetMemberIdentifier(typeInfo);
        }

        private static string GetMemberIdentifier(MemberInfo info)
        {
            var nameAttribute = info.GetCustomAttribute(typeof(XmlConvertElementNameAttribute));
            if (nameAttribute != null)
            {
                return info.Name;
            }

            return info.Name;
        }

        private static XElement GetChildElement(string name, XElement parent)
        {
            var element = parent.Element(name);
            if (element == null) { /*No such element*/ }

            return element;
        }

        private static bool IsFundamentalPrimitive(object obj)
        {
            return IsFundamentalPrimitive(obj.GetType());
        }

        private static bool IsFundamentalPrimitive(Type type) => (
            type.Equals(typeof(string))
            || type.Equals(typeof(string))
            || type.Equals(typeof(char))
            || type.Equals(typeof(sbyte))
            || type.Equals(typeof(short))
            || type.Equals(typeof(int))
            || type.Equals(typeof(long))
            || type.Equals(typeof(byte))
            || type.Equals(typeof(ushort))
            || type.Equals(typeof(uint))
            || type.Equals(typeof(ulong))
            || type.Equals(typeof(double))
            || type.Equals(typeof(float))
            || type.Equals(typeof(decimal))
            || type.Equals(typeof(bool))
            || type.Equals(typeof(DateTime))
            );

        private static bool IsList(object obj)
        {
            return IsList(obj.GetType());
        }

        private static bool IsList(Type type)
        {
            return typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        private static bool IsDictionary(object obj)
        {
            return IsDictionary(obj.GetType());
        }

        private static bool IsDictionary(Type type)
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        private static bool IsSupported(object obj)
        {
            return IsFundamentalPrimitive(obj) || IsList(obj);
        }
    }
}
