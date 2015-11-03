using System;
using System.Collections;
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

            var properties = type.GetRuntimeProperties();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    DeserializeProperty(property, value, element);
                }
            }

            return value;
        }

        private static void DeserializeProperty<T>(PropertyInfo property, T obj, XElement element)
        {
            var name = property.Name;
            var type = property.PropertyType;

            var propertyElement = GetChildElement(name, element);
            if (propertyElement != null)
            {
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
        }

        private static object DeserializeObjectInternal(Type type, XElement element)
        {
            var value = element.Value;
            if (value == null) { /*No value*/ return null; }
            
            if (IsFundamentalPrimitive(type))
            {
                return DeserializeFundamentalPrimitive(type, element);
            }
            else if (IsList(type))
            {
                return DeserializeList(type, element);
            }

            return null;
        }

        private static object DeserializeFundamentalPrimitive(Type type, XElement element)
        {
            try
            {
                return Convert.ChangeType(element.Value, type);
            }
            catch(InvalidCastException)
            {
                return null;
            }
        }

        private static object DeserializeList(Type type, XElement element)
        {
            var list = (IList)Activator.CreateInstance(type);
            var childElements = element.Elements();
            if (childElements == null) { return list; }
            
            foreach (var childElement in childElements)
            {
                Type childType = null;

                var typeString = childElement.Attribute("Type")?.Value;
                if (typeString != null)
                {
                    childType = Type.GetType(typeString);
                }

                if (childType == null)
                {
                    var genericArguments = type.GetTypeInfo().GenericTypeArguments;
                    if (genericArguments != null && genericArguments.Length > 0)
                    {
                        childType = genericArguments[0];                        
                    }
                }

                if (childType != null)
                {
                    var obj = DeserializeObjectInternal(childType, childElement);

                    if (obj != null)
                    {
                        list.Add(obj);
                    }
                }
            }

            return list;
        }
    }
}
