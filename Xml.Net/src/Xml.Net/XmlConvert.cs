using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Xml.Net
{
    /// <summary>
    /// The class that serializes or deserializes .NET objects.
    /// </summary>
    public static partial class XmlConvert
    {
        /// <summary>
        /// Provides the default options for formatting, serializing and deserializing objects.
        /// </summary>
        private const XmlConvertOptions DefaultConvertOptions = XmlConvertOptions.None;

        /// <summary>
        /// Gets the XML identifier of the class from an object of that class.
        /// </summary>
        /// <param name="obj">The object to use.</param>
        /// <returns>The XML identifier of the class.</returns>
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

        /// <summary>
        /// Gets the XML identifier of the member.
        /// </summary>
        /// <param name="memberInfo">The information about the member to use.</param>
        /// <returns>The XML identifier of the member.</returns>
        private static string GetMemberIdentifier(MemberInfo memberInfo)
        {
            var nameAttribute = memberInfo.GetCustomAttribute(typeof(XmlConvertCustomElementAttribute));
            if (nameAttribute != null)
            {
                return memberInfo.Name;
            }

            return memberInfo.Name;
        }

        /// <summary>
        /// Gets the name-specified child XElement of the parent XElement.
        /// </summary>
        /// <param name="name">The name of the child XElement to get.</param>
        /// <param name="parent">The parent of the child XElement to get.</param>
        /// <returns>The name-specified child XElement of the parent XElement.</returns>
        private static XElement GetChildElement(string name, XElement parent)
        {
            var element = parent.Element(name);
            if (element == null) { /*No such element*/ }

            return element;
        }

        /// <summary>
        /// Checks if the object is a fundamental primitive object (e.g string, int etc.).
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>The boolean value indicating whether the object is a fundamental primitive.</returns>
        private static bool IsFundamentalPrimitive(object value)
        {
            return IsFundamentalPrimitive(value.GetType());
        }

        /// <summary>
        /// Checks if the type is a fundamental primitive object (e.g string, int etc.).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The boolean value indicating whether the type is a fundamental primitive.</returns>
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

        /// <summary>
        /// Checks if the object is a list (e.g List<T>, Array etc.).
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>The boolean value indicating whether the object is a list.</returns>
        private static bool IsList(object value)
        {
            return IsList(value.GetType());
        }

        /// <summary>
        /// Checks if the type is a list (e.g List<T>, Array etc.).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The boolean value indicating whether the type is a list.</returns>
        private static bool IsList(Type type)
        {
            return typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())
                || typeof(IReadOnlyList<>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Checks if the object is a dictionary (e.g Dictionary<TKey, TValue>, HashTable etc.).
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>The boolean value indicating whether the type is a dictionary.</returns>
        private static bool IsDictionary(object value)
        {
            return IsDictionary(value.GetType());
        }

        /// <summary>
        /// Checks if the object is a dictionary (e.g Dictionary<TKey, TValue>, HashTable etc.).
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>The boolean value indicating whether the object is a dictionary.</returns>
        private static bool IsDictionary(Type type)
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Checks if the object can be serialized or deserialized into XML.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>The boolean value indicating whether the type can be serialized or deserialized into XML.</returns>
        private static bool IsSupported(object obj)
        {
            return IsFundamentalPrimitive(obj) || IsList(obj);
        }
    }
}
