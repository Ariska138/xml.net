using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

namespace Xml.Net
{
    public static partial class XmlConvert
    {
        public static string SerializeObject(object obj)
        {
            return SerializeObject(obj, DefaultConvertOptions);
        }

        public static string SerializeObject(object obj, XmlConvertOptions options)
        {
            return SerializeXElement(obj, options).ToString();
        }

        public static XElement SerializeXElement(object obj)
        {
            return SerializeXElement(obj, DefaultConvertOptions);
        }

        public static XElement SerializeXElement(object obj, XmlConvertOptions options)
        {
            if (obj == null) { throw new ArgumentNullException(nameof(obj)); }
            if (IsFundamentalPrimitive(obj)) { throw new ArgumentException("Cannot serialize a fundamental primative", nameof(obj)); }
            
            var identifier = GetClassIdentifier(obj);            
            var element = new XElement(identifier);

            var properties = obj.GetType().GetRuntimeProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    SerializeProperty(property, obj, element, options);
                }
            }

            return element;
        }

        private static void SerializeProperty(PropertyInfo property, object obj, XElement parent, XmlConvertOptions options)
        {
            if (property == null) { return; }
            if (obj == null) { return; }
            if (parent == null) { return; }

            if (IsIgnoredProperty(property))
            {
                return;
            }

            var name = GetMemberIdentifier(property);
            var value = property.GetValue(obj);
            
            string elementNames = null;
            string keyNames = null;
            string valueNames = null;
            
            if (IsList(property.PropertyType))
            {
                elementNames = GetCollectionElementName(property);
            }
            else if (IsDictionary(property.PropertyType))
            {
                elementNames = GetCollectionElementName(property);

                var dictionaryNames = GetDictionaryElementName(property);
                keyNames = dictionaryNames.Key;
                valueNames = dictionaryNames.Value;
            }

            SerializeObjectInternal(name, value, parent, options, elementNames, keyNames, valueNames);
        }

        private static void SerializeObjectInternal(string name, object obj, XElement parent, XmlConvertOptions options, string elementNames, string keyNames, string valueNames)
        {
            if (name == null) { return; }
            if (obj == null) { return; }
            if (parent == null) { return; }

            if (IsFundamentalPrimitive(obj))
            {
                SerializeFundamentalPrimitive(name, obj, parent, options);
            }
            else if (IsList(obj))
            {
                if (elementNames == null) { elementNames = "Element"; }

                SerializeList(name, obj, parent, options, elementNames);
            }
            else if (IsDictionary(obj))
            {
                if (elementNames == null) { elementNames = "Element"; }
                if (keyNames == null) { keyNames = "Key"; }
                if (valueNames == null) { valueNames = "Value"; }

                SerializeDictionary(name, obj, parent, options, elementNames, keyNames, valueNames);
            }
        }

        private static void SerializeFundamentalPrimitive(string name, object obj, XElement parent, XmlConvertOptions options)
        {
            if (name == null) { return; }
            if (obj == null) { return; }
            if (parent == null) { return; }

            var stringValue = Convert.ToString(obj);
            var element = new XElement(name, stringValue);

            SetupSerializedElement(obj, element, parent, options);
        }

        private static void SerializeList(string name, object obj, XElement parent, XmlConvertOptions options, string elementNames)
        {
            if (name == null) { return; }
            if (obj == null) { return; }
            if (parent == null) { return; }

            var element = new XElement(name);

            var list = (IList)obj;
            foreach (var value in list)
            {
                SerializeObjectInternal(elementNames, value, element, options, null, null, null);
            }

            SetupSerializedElement(obj, element, parent, options);
        }
                
        private static void SerializeDictionary(string name, object obj, XElement parent, XmlConvertOptions options, string elementNames, string keyNames, string valueNames)
        {
            if (name == null) { return; }
            if (obj == null) { return; }
            if (parent == null) { return; }
            if (elementNames == null) { return; }

            var element = new XElement(name);

            var dictionary = (IDictionary)obj;
            foreach (DictionaryEntry value in dictionary)
            {
                var childElement = new XElement(elementNames);

                SerializeObjectInternal(keyNames, value.Key, childElement, options, null, null, null);
                SerializeObjectInternal(valueNames, value.Value, childElement, options, null, null, null);

                element.Add(childElement);
            }

            SetupSerializedElement(obj, element, parent, options);
        }

        private static bool IsIgnoredProperty(PropertyInfo property)
        {
            return property == null || property.GetCustomAttribute<XmlConvertIgnoredAttribute>() != null;
        }

        private static string GetCollectionElementName(PropertyInfo property)
        {
            var name = "Element";

            var elementNameAttribute = property?.GetCustomAttribute<XmlConvertElementsNameAttribute>();
            if (elementNameAttribute?.Name != null)
            {
                return elementNameAttribute.Name;
            }

            return name;
        }

        private static KeyValuePair<string, string> GetDictionaryElementName(PropertyInfo property)
        {
            var keyName = "Key";
            var valueName = "Value";

            var elementNameAttribute = property?.GetCustomAttribute<XmlConvertKeyValueElementAttribute>();
            if (elementNameAttribute != null)
            {
                if (elementNameAttribute.KeyName != null)
                {
                    keyName = elementNameAttribute.KeyName;
                }

                if (elementNameAttribute.ValueName != null)
                {
                    valueName = elementNameAttribute.ValueName;
                }
            }

            return new KeyValuePair<string, string>(keyName, valueName);
        }

        private static void SetupSerializedElement(object obj, XElement element, XElement parent, XmlConvertOptions options)
        {
            if (obj == null) { return; }
            if (element == null) { return; }
            if (parent == null) { return; }

            if (!options.HasFlag(XmlConvertOptions.ExcludeTypes))
            {
                var typeName = obj.GetType().FullName;
                element.Add(new XAttribute("Type", typeName));
            }

            parent.Add(element);
        }
    }
}
