using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Xml.Net
{
    public static partial class XmlConvert
    {
        /// <summary>
        /// Serializes the specified object to a XML string.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The XML string representation of the object.</returns>
        public static string SerializeObject(object value)
        {
            return SerializeObject(value, DefaultConvertOptions);
        }

        /// <summary>
        /// Serializes the specified object to a XML string using options.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        /// <returns>The XML string representation of the object.</returns>
        public static string SerializeObject(object value, XmlConvertOptions options)
        {
            return SerializeXElement(value, options).ToString();
        }

        /// <summary>
        /// Serializes the specified object to a XElement.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The XDocument representation of the object.</returns>
        public static XElement SerializeXElement(object value)
        {
            return SerializeXElement(value, DefaultConvertOptions);
        }

        /// <summary>
        /// Serializes the specified object to a XElement using options.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        /// <returns>The XDocument representation of the object.</returns>
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

        /// <summary>
        /// Serializes the specified property into a XElement using options.
        /// </summary>
        /// <param name="property">The property to serialize.</param>
        /// <param name="parentObject">The object that owns the property.</param>
        /// <param name="parentElement">The element in which to serialize the property.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static void SerializeProperty(PropertyInfo property, object parentObject, XElement parentElement, XmlConvertOptions options)
        {
            if (property == null) { return; }
            if (parentObject == null) { return; }
            if (parentElement == null) { return; }

            if (IsIgnoredProperty(property))
            {
                return;
            }

            var propertyName = GetMemberIdentifier(property);
            var propertyValue = property.GetValue(parentObject);
            
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

            SerializeObjectInternal(propertyValue, propertyName, parentElement, elementNames, keyNames, valueNames, options);
        }

        /// <summary>
        /// Serializes the specified property into a XElement using options.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="name">The name of the object to serialize.</param>
        /// <param name="parentElement">The element in which to serialize the object.</param>
        /// <param name="elementNames">The optional custom name of collection elements.</param>
        /// <param name="keyNames">The optional custom name of dictionary key elements.</param>
        /// <param name="valueNames">The optional custom name of dictionary value elements.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static void SerializeObjectInternal(object value, string name, XElement parentElement, string elementNames, string keyNames, string valueNames, XmlConvertOptions options)
        {
            if (value == null) { return; }
            if (name == null) { return; }
            if (parentElement == null) { return; }

            if (IsFundamentalPrimitive(value))
            {
                SerializeFundamentalPrimitive(value, name, parentElement, options);
            }
            else if (IsList(value))
            {
                if (elementNames == null) { elementNames = "Element"; }

                SerializeList(value, name, parentElement, elementNames, options);
            }
            else if (IsDictionary(value))
            {
                if (elementNames == null) { elementNames = "Element"; }
                if (keyNames == null) { keyNames = "Key"; }
                if (valueNames == null) { valueNames = "Value"; }

                SerializeDictionary(value, name, parentElement, elementNames, keyNames, valueNames, options);
            }
        }

        /// <summary>
        /// Serializes a fundamental primitive object (e.g. string, int etc.) into a XElement using options.
        /// </summary>
        /// <param name="value">The primitive to serialize.</param>
        /// <param name="name">The name of the primitive to serialize.</param>
        /// <param name="parentElement">The element in which to serialize the primitive.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static void SerializeFundamentalPrimitive(object value, string name, XElement parentElement, XmlConvertOptions options)
        {
            if (name == null) { return; }
            if (value == null) { return; }
            if (parentElement == null) { return; }

            var stringValue = Convert.ToString(value);
            var element = new XElement(name, stringValue);

            SetupSerializedElement(value, element, parentElement, options);
        }

        /// <summary>
        /// Serializes a list (e.g. List<T>, Array etc.) into a XElement using options.
        /// </summary>
        /// <param name="value">The list to serialize.</param>
        /// <param name="name">The name of the list to serialize.</param>
        /// <param name="parentElement">The element in which to serialize the list.</param>
        /// <param name="elementNames">The custom name of collection elements.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static void SerializeList(object value, string name, XElement parentElement
            , string elementNames, XmlConvertOptions options)
        {
            if (name == null) { return; }
            if (value == null) { return; }
            if (parentElement == null) { return; }

            var element = new XElement(name);

            var list = (IList)value;
            foreach (var childValue in list)
            {
                SerializeObjectInternal(childValue, elementNames, element, null, null, null, options);
            }

            SetupSerializedElement(value, element, parentElement, options);
        }

        /// <summary>
        /// Serializes a dictionary (e.g. List<TKey, TValue>, HashTable etc.) into a XElement using options.
        /// </summary>
        /// <param name="value">The dictionary to serialize.</param>
        /// <param name="name">The name of the dictionary to serialize.</param>
        /// <param name="parentElement">The element in which to serialize the list.</param>
        /// <param name="elementNames">The custom name of collection elements.</param>
        /// <param name="keyNames">The optional custom name of dictionary key elements.</param>
        /// <param name="valueNames">The optional custom name of dictionary value elements.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static void SerializeDictionary(object value, string name, XElement parentElement, string elementNames, string keyNames, string valueNames, XmlConvertOptions options)
        {
            if (name == null) { return; }
            if (value == null) { return; }
            if (parentElement == null) { return; }
            if (elementNames == null) { return; }

            var element = new XElement(name);

            var dictionary = (IDictionary)value;
            foreach (DictionaryEntry dictionaryEntry in dictionary)
            {
                var childElement = new XElement(elementNames);

                SerializeObjectInternal(dictionaryEntry.Key, keyNames, childElement, null, null, null, options);
                SerializeObjectInternal(dictionaryEntry.Value, valueNames, childElement, null, null, null, options);

                element.Add(childElement);
            }

            SetupSerializedElement(value, element, parentElement, options);
        }

        /// <summary>
        /// Checks if a property should not be serialized.
        /// </summary>
        /// <param name="property">The property to check.</param>
        private static bool IsIgnoredProperty(PropertyInfo property)
        {
            return property == null || property.GetCustomAttribute<XmlConvertIgnoredAttribute>() != null;
        }

        /// <summary>
        /// Gets the custom name of collection elements of a property.
        /// </summary>
        /// <param name="property">The property to use.</param>
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

        /// <summary>
        /// Gets the custom name of collection key and value elements of a property.
        /// </summary>
        /// <param name="property">The property to use.</param>
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

        /// <summary>
        /// Formats and adds a serialized object XElement to a parent element with options.
        /// </summary>
        /// <param name="value">The object serialized.</param>
        /// <param name="element">The serialized XElement of the object.</param>
        /// <param name="parentElement">The parent element of the serialized XElement.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static void SetupSerializedElement(object value, XElement element, XElement parentElement, XmlConvertOptions options)
        {
            if (value == null) { return; }
            if (element == null) { return; }
            if (parentElement == null) { return; }

            if (!options.HasFlag(XmlConvertOptions.ExcludeTypes))
            {
                var typeName = value.GetType().FullName;
                element.Add(new XAttribute("Type", typeName));
            }

            parentElement.Add(element);
        }
    }
}
