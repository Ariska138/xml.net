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
        public static T DeserializeObject<T>(string xml) where T : new()
        {
            return DeserializeObject<T>(xml, DefaultConvertOptions);
        }

		public static T DeserializeObject<T>(string xml, XmlConvertOptions options) where T : new()
        {
            if (xml == null) { throw new ArgumentNullException(nameof(xml)); }

            return DeserializeXElement<T>(XElement.Parse(xml), options);
        }

        public static T DeserializeXElement<T>(XElement element) where T : new()
        {
            return DeserializeXElement<T>(element, DefaultConvertOptions);
        }

        public static T DeserializeXElement<T>(XElement element, XmlConvertOptions options)
        {
            return (T)DeserializeXElement(typeof(T), element, options);
        }

        public static object DeserializeXElement(Type type, XElement element)
        {
            return DeserializeXElement(type, element, DefaultConvertOptions);
        }

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
                    DeserializeProperty(property, value, element);
                }
            }

            return value;
        }

        private static void DeserializeProperty<T>(PropertyInfo property, T obj, XElement parent)
        {
            if (property == null) { return; }
            if (obj == null) { return; }
            if (parent == null) { return; }

            var name = property.Name;
            var type = property.PropertyType;

            var propertyElement = GetChildElement(name, parent);

            var value = DeserializeObjectInternal(type, propertyElement);
            if (value != null)
            {
                property.SetValue(obj, value);
            }
            else
            {
                //Handle not parsable error
            }
        }

        private static object DeserializeObjectInternal(Type type, XElement parent)
        {
            if (type == null) { return null; }
            if (parent == null) { return null; }

            var value = parent.Value;
            if (value == null) { return null; }
            
            if (IsFundamentalPrimitive(type))
            {
                return DeserializeFundamentalPrimitive(type, parent);
            }
            else if (IsList(type))
            {
                return DeserializeList(type, parent);
            }
            else if (IsDictionary(type))
            {
                return DeserializeDictionary(type, parent);
            }

            return null;
        }

        private static object DeserializeFundamentalPrimitive(Type type, XElement parent)
        {
            if (type == null) { return null; }
            if (parent == null) { return null; }

            try
            {
                return Convert.ChangeType(parent.Value, type);
            }
            catch(InvalidCastException)
            {
                return null;
            }
        }

        private static object DeserializeList(Type type, XElement parent)
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
                    var obj = DeserializeObjectInternal(elementType, element);

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

        private static object DeserializeDictionary(Type type, XElement parent)
        {
            if (type == null) { return null; }
            if (parent == null) { return null; }

            var dictionary = (IDictionary)Activator.CreateInstance(type);

            var elements = parent.Elements();
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
                    var key = DeserializeObjectInternal(keyType, keyElement);
                    var value = DeserializeObjectInternal(valueType, valueElement);
                    
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
