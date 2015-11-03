using System;
using System.Collections;
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
            if (property.GetCustomAttribute<XmlConvertIgnoredAttribute>() != null)
            {
                return;
            }

            var name = GetMemberIdentifier(property);
            var value = property.GetValue(obj);
            
            SerializeObjectInternal(name, value, parent, options);
        }

        private static void SerializeObjectInternal(string name, object obj, XElement parent, XmlConvertOptions options)
        {
            if (obj == null) { return; }

            if (IsFundamentalPrimitive(obj))
            {
                SerializeFundamentalPrimitive(name, obj, parent, options);
            }
            else if (IsList(obj))
            {
                SerializeList(name, obj, parent, options);
            }
            else if (IsDictionary(obj))
            {
                SerializeDictionary(name, obj, parent, options);
            }
        }

        private static void SerializeFundamentalPrimitive(string name, object obj, XElement parent, XmlConvertOptions options)
        {
            var stringValue = Convert.ToString(obj);
            var element = new XElement(name, stringValue);

            SetupSerializedElement(obj, element, parent, options);
        }

        private static void SerializeList(string name, object obj, XElement parent, XmlConvertOptions options)
        {
            var element = new XElement(name);

            var list = (IList)obj;
            for (int i = 0; i < list.Count; i++)
            {
                var collectionName = "Element" + i.ToString();
                var collectionValue = list[i];

                SerializeObjectInternal(collectionName, collectionValue, element, options);
            }

            SetupSerializedElement(obj, element, parent, options);
        }

        private static void SerializeDictionary(string name, object obj, XElement parent, XmlConvertOptions options)
        {
            //TODO: Serialize Dictionaries
        }

        private static void SetupSerializedElement(object obj, XElement element, XElement parent, XmlConvertOptions options)
        {
            if (!options.HasFlag(XmlConvertOptions.ExcludeTypes))
            {
                var typeName = obj.GetType().FullName;
                element.Add(new XAttribute("Type", typeName));
            }

            parent.Add(element);
        }
    }
}
