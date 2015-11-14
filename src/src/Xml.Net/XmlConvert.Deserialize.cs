using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Linq;

namespace Xml.Net
{
    public static partial class XmlConvert
    {
        /// <summary>
        /// Deserializes the XML string to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="value">The XML string to deserialize.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static T DeserializeObject<T>(string value) where T : new()
        {
            return DeserializeObject<T>(value, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XML string to the specified .NET type using options.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="value">The XML string to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static T DeserializeObject<T>(string value, XmlConvertOptions options) where T : new()
        {
            return (T)DeserializeObject(typeof(T), value, options);
        }

        /// <summary>
        /// Deserializes the XML string to the specified .NET type.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="value">The XML string to deserialize.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static object DeserializeObject(Type type, string value)
        {
            return DeserializeObject(type, value, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XML string to the specified .NET type using options.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="value">The XML string to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static object DeserializeObject(Type type, string value, XmlConvertOptions options)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            return DeserializeXElement(type, XElement.Parse(value), options);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="element">The XElement to deserialize.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static T DeserializeXElement<T>(XElement element) where T : new()
        {
            return DeserializeXElement<T>(element, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type using options.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="element">The XElement to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static T DeserializeXElement<T>(XElement element, XmlConvertOptions options)
        {
            return (T)DeserializeXElement(typeof(T), element, options);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="element">The XElement to deserialize.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static object DeserializeXElement(Type type, XElement element)
        {
            return DeserializeXElement(type, element, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type using options.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="element">The XElement to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static object DeserializeXElement(Type type, XElement element, XmlConvertOptions options)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            
            var value = Activator.CreateInstance(type);
            var identifier = GetClassIdentifier(value);

            var properties = type.GetTypeInfo().DeclaredProperties;

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    DeserializeProperty(property, value, element, options);
                }
            }

            return value;
        }

        /// <summary>
        /// Deserializes the XElement to the specified property using options.
        /// </summary>
        /// <param name="property">The property to deserialize the XElement into.</param>
        /// <param name="parentObject">The object that owns the property.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the property.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        private static void DeserializeProperty(PropertyInfo property, object parentObject, XElement parentElement, XmlConvertOptions options)
        {
            if (property == null) { return; }
            if (parentObject == null) { return; }
            if (parentElement == null) { return; }

            var name = GetMemberIdentifier(property);
            var type = property.PropertyType;

            var propertyElement = GetChildElement(name, parentElement);
            
            var value = DeserializeObjectInternal(type, propertyElement, options);
            if (value != null)
            {
                property.SetValue(parentObject, value);
            }
            else
            {
                //Handle not parsable error
            }
        }

        /// <summary>
        /// Deserializes the XElement to the object of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the object.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        private static object DeserializeObjectInternal(Type type, XElement parentElement, XmlConvertOptions options)
        {
            if (type == null) { return null; }
            if (parentElement == null) { return null; }

            var value = parentElement.Value;
            if (value == null) { return null; }


            if (IsFundamentalPrimitive(type))
            {
                return DeserializeFundamentalPrimitive(type, parentElement, options);
            }
            else if (IsList(type))
            {
                return DeserializeList(type, parentElement, options);
            }
            else if (IsDictionary(type))
            {
                return DeserializeDictionary(type, parentElement, options);
            }
            else
            {
                return DeserializeXElement(type, parentElement, options);
            }
        }

        /// <summary>
        /// Deserializes the XElement to the fundamental primitive (e.g. string, int etc.) of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the fundamental primitive to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the fundamental primitive.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized fundamental primitive from the XElement.</returns>
        private static object DeserializeFundamentalPrimitive(Type type, XElement parentElement, XmlConvertOptions options)
        {
            if (type == null) { return null; }
            if (parentElement == null) { return null; }

            try
            {
                return Convert.ChangeType(parentElement.Value, type);
            }
            catch(InvalidCastException)
            {
                return null;
            }
        }

        /// <summary>
        /// Deserializes the XElement to the list (e.g. List<T>, Array of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the list to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the list.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized list from the XElement.</returns>
        private static object DeserializeList(Type type, XElement parent, XmlConvertOptions options)
        {
            if (type == null) { return null; }
            if (parent == null) { return null; }

            var list = (IList)Activator.CreateInstance(type);
            
            var elements = parent.Elements();
            if (elements == null) { return list; }
            
            foreach (var element in elements)
            {
                var elementType = GetElementType(element, type, 0); 

                if (elementType != null)
                {
                    var obj = DeserializeObjectInternal(elementType, element, options);

                    if (obj != null)
                    {
                        list.Add(obj);
                    }
                    else
                    {
                        //Error parsing key and/or value
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Deserializes the XElement to the dictionary (e.g. Dictionary<TKey, TValue>, HashTable of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the dictionary to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the dictionary.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized dictionary from the XElement.</returns>
        private static object DeserializeDictionary(Type type, XElement parentElement, XmlConvertOptions options)
        {
            if (type == null) { return null; }
            if (parentElement == null) { return null; }

            var dictionary = (IDictionary)Activator.CreateInstance(type);

            var elements = parentElement.Elements();
            if (elements == null) { return dictionary; }

            foreach (var element in elements)
            {
                var keyValueElements = element.Elements();
                if (keyValueElements == null) { return dictionary; }

                var keyValueElementsList = new List<XElement>(keyValueElements);

                if (keyValueElementsList.Count < 2)
                {
                    //No fully formed key value pair
                    return dictionary;
                }

                var keyElement = keyValueElementsList[0];
                var valueElement = keyValueElementsList[1];
                                
                var keyType = GetElementType(keyElement, type, 0);
                var valueType = GetElementType(valueElement, type, 1);

                
                if (keyType != null && valueType != null)
                {
                    var key = DeserializeObjectInternal(keyType, keyElement, options);
                    var value = DeserializeObjectInternal(valueType, valueElement, options);
                    
                    if (key != null && value != null)
                    {
                        dictionary.Add(key, value);
                    }
                    else
                    {
                        //Error parsing key and/or value
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the type of an object from its serialized XElement (e.g. if it has a type attribute) and its parent container type (e.g. if it is generic).
        /// </summary>
        /// <param name="element">The XElement representing the object used to get the type.</param>
        /// <param name="parentType">The type of the object's parent container used to check if the object is in a generic container.</param>
        /// <param name="genericArgumentIndex">The index of the object's type in the list of its parent container's generic arguments.</param>
        /// <returns>The type of an object from its serialized XElement and its parent container type.</returns>
        private static Type GetElementType(XElement element, Type parentType, int genericArgumentIndex)
        {
            Type type = null;

            var typeString = element?.Attribute("Type")?.Value;
            if (typeString != null)
            {
                type = Type.GetType(typeString);
            }
            
            if (type == null)
            {
                var genericArguments = parentType?.GetTypeInfo().GenericTypeArguments;
                if (genericArguments != null && genericArgumentIndex >= 0 && genericArguments.Length > genericArgumentIndex)
                {
                    type = genericArguments[genericArgumentIndex];
                }
            }

            return type;
        }
    }
}
