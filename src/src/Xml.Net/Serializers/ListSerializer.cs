using System;
using System.Collections;
using System.Xml.Linq;

namespace Xml.Net.Serializers
{
    internal static class ListSerializer
    {
        /// <summary>
        /// Serializes a list (e.g. List<T>, Array etc.) into a XElement using options.
        /// </summary>
        /// <param name="value">The list to serialize.</param>
        /// <param name="name">The name of the list to serialize.</param>
        /// <param name="elementNames">The custom name of collection elements.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        public static XElement Serialize(object value, string name, string elementNames, XmlConvertOptions options)
        {
            if (name == null) { return null; }
            if (value == null) { return null; }

            var parentElement = new XElement(name);

            var list = (ICollection)value;
            foreach (var childValue in list)
            {
                var childElement = ObjectSerializer.Serialize(childValue, elementNames, null, null, null, options);
                Utilities.AddChildElement(childElement, parentElement);
            }

            return parentElement;
        }

        /// <summary>
        /// Deserializes the XElement to the list (e.g. List<T>, Array of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the list to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the list.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized list from the XElement.</returns>
        public static object Deserialize(Type type, XElement parent, XmlConvertOptions options)
        {
            if (type == null) { return null; }
            if (parent == null) { return null; }

            var list = (IList)Activator.CreateInstance(type);

            var elements = parent.Elements();
            if (elements == null) { return list; }

            foreach (var element in elements)
            {
                var elementType = Utilities.GetElementType(element, type, 0);

                if (elementType != null)
                {
                    var obj = ObjectSerializer.Deserialize(elementType, element, options);

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
    }
}
