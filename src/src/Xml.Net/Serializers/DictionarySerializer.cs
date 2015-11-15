using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Xml.Net.Serializers
{
    public static class DictionarySerializer
    {
        /// <summary>
        /// Serializes a dictionary (e.g. List<TKey, TValue>, HashTable etc.) into a XElement using options.
        /// </summary>
        /// <param name="value">The dictionary to serialize.</param>
        /// <param name="name">The name of the dictionary to serialize.</param>
        /// <param name="elementNames">The custom name of collection elements.</param>
        /// <param name="keyNames">The optional custom name of dictionary key elements.</param>
        /// <param name="valueNames">The optional custom name of dictionary value elements.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        public static XElement Serialize(object value, string name, string elementNames, string keyNames, string valueNames, XmlConvertOptions options)
        {
            if (name == null) { return null; }
            if (value == null) { return null; }
            if (elementNames == null) { return null; }

            var element = new XElement(name);

            var dictionary = (IDictionary)value;
            foreach (DictionaryEntry dictionaryEntry in dictionary)
            {
                var keyValueElement = new XElement(elementNames);
                
                var keyElement = ObjectSerializer.Serialize(dictionaryEntry.Key, keyNames, null, null, null, options);
                var valueElement = ObjectSerializer.Serialize(dictionaryEntry.Value, valueNames, null, null, null, options);

                Utilities.AddChildElement(keyElement, keyValueElement);
                Utilities.AddChildElement(valueElement, keyValueElement);

                element.Add(keyValueElement);
            }

            return element;
        }


        /// <summary>
        /// Deserializes the XElement to the dictionary (e.g. Dictionary<TKey, TValue>, HashTable of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the dictionary to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the dictionary.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized dictionary from the XElement.</returns>
        public static object Deserialize(Type type, XElement parentElement, XmlConvertOptions options)
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

                var keyType = Utilities.GetElementType(keyElement, type, 0);
                var valueType = Utilities.GetElementType(valueElement, type, 1);


                if (keyType != null && valueType != null)
                {
                    var key = ObjectSerializer.Deserialize(keyType, keyElement, options);
                    var value = ObjectSerializer.Deserialize(valueType, valueElement, options);

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
    }
}
