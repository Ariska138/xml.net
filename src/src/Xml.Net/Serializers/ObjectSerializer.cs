using System;
using System.Reflection;
using System.Xml.Linq;

namespace Xml.Net.Serializers
{
    internal class ObjectSerializer
    {
        /// <summary>
        /// Serializes the specified object to a XElement using options.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="name">The name of the object to serialize.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        /// <returns>The XElement representation of the object.</returns>
        public static XElement Serialize(object value, string name, XmlConvertOptions options)
        {
            var objectElement = new XElement(name);

            var properties = value.GetType().GetRuntimeProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyElement = Serialize(property, value, options);
                    Utilities.AddChildElement(propertyElement, objectElement);
                }
            }

            return objectElement;
        }

        /// <summary>
        /// Serializes the specified property into a XElement using options.
        /// </summary>
        /// <param name="property">The property to serialize.</param>
        /// <param name="parentObject">The object that owns the property.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static XElement Serialize(PropertyInfo property, object parentObject, XmlConvertOptions options)
        {
            if (property == null) { return null; }
            if (parentObject == null) { return null; }

            if (Utilities.ShouldIgnoreProperty(property)) //Either we ignore or can't read the property
            {
                return null;
            }

            var m = property.GetMethod;

            var propertyValue = m.Invoke(parentObject, null);
            if (propertyValue == null) //Ignore null properties
            {
                return null;
            }

            var propertyName = Utilities.GetIdentifier(property);

            string elementNames = null;
            string keyNames = null;
            string valueNames = null;

            var objectType = ObjectType.From(property.PropertyType);
            if (objectType == ObjectType.Dictionary)
            {
                elementNames = Utilities.GetCollectionElementName(property);

                var dictionaryNames = Utilities.GetDictionaryElementName(property);
                keyNames = dictionaryNames.Key;
                valueNames = dictionaryNames.Value;
            }
            else if (objectType == ObjectType.List)
            {
                elementNames = Utilities.GetCollectionElementName(property);
            }

            return Serialize(propertyValue, propertyName, elementNames, keyNames, valueNames, options);
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
        public static XElement Serialize(object value, string name, string elementNames, string keyNames, string valueNames, XmlConvertOptions options)
        {
            if (value == null) { return null; }
            if (name == null) { return null; }
            
            XElement childElement = null;

            var objectType = ObjectType.From(value);

            if (objectType == ObjectType.Primitive)
            {
                childElement = PrimitiveSerializer.Serialize(value, name, options);
            }
            else if (objectType == ObjectType.Dictionary)
            {
                if (elementNames == null) { elementNames = "Element"; }
                if (keyNames == null) { keyNames = "Key"; }
                if (valueNames == null) { valueNames = "Value"; }

                childElement = DictionarySerializer.Serialize(value, name, elementNames, keyNames, valueNames, options);
            }
            else if (objectType == ObjectType.List)
            {
                if (elementNames == null) { elementNames = "Element"; }

                childElement = ListSerializer.Serialize(value, name, elementNames, options);
            }
            else
            {
                childElement = Serialize(value, name, options); //Recurse
            }

            return childElement;
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type using options.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="element">The XElement to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static object DeserializeObject(Type type, XElement element, XmlConvertOptions options)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (element == null) { throw new ArgumentNullException(nameof(element)); }

            var value = Activator.CreateInstance(type);
            var identifier = Utilities.GetIdentifier(value);

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

            var name = Utilities.GetIdentifier(property);
            var type = property.PropertyType;

            var propertyElement = Utilities.GetChildElement(name, parentElement);

            var value = Deserialize(type, propertyElement, options);
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
        public static object Deserialize(Type type, XElement parentElement, XmlConvertOptions options)
        {
            if (type == null) { return null; }
            if (parentElement == null) { return null; }

            var value = parentElement.Value;
            if (value == null) { return null; }

            var objectType = ObjectType.From(type);

            if (objectType == ObjectType.Primitive)
            {
                return PrimitiveSerializer.Deserialize(type, parentElement, options);
            }
            else if (objectType == ObjectType.Dictionary)
            {
                return DictionarySerializer.Deserialize(type, parentElement, options);
            }
            else if (objectType == ObjectType.List)
            {
                return ListSerializer.Deserialize(type, parentElement, options);
            }
            else
            {
                return DeserializeObject(type, parentElement, options);
            }
        }
    }
}
