using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

using Xunit;

namespace Xml.Net.Tests
{
    public static class CustomNameTest
    {
        [Fact]
        public static void Ctor_ElementName_Success()
        {
            XmlConvertCustomElementAttribute attribute = new XmlConvertCustomElementAttribute("hello");
            Assert.Equal("hello", attribute.Name);
        }

        [Fact]
        public static void Ctor_ElementName_Invalid()
        {
            Assert.Throws<ArgumentNullException>("name", () => new XmlConvertCustomElementAttribute(null)); //Name is null

            Assert.Throws<ArgumentException>("name", () => new XmlConvertCustomElementAttribute("")); //Name is empty
            Assert.Throws<ArgumentException>("name", () => new XmlConvertCustomElementAttribute("  ")); //Name is whitespace
        }

        [Fact]
        public static void Ctor_ElementsName_Success()
        {
            XmlConvertElementsNameAttribute attribute = new XmlConvertElementsNameAttribute("hello");
            Assert.Equal("hello", attribute.Name);
        }

        [Fact]
        public static void Ctor_ElementsName_Invalid()
        {
            Assert.Throws<ArgumentNullException>("name", () => new XmlConvertElementsNameAttribute(null)); //Name is null

            Assert.Throws<ArgumentException>("name", () => new XmlConvertElementsNameAttribute("")); //Name is empty
            Assert.Throws<ArgumentException>("name", () => new XmlConvertElementsNameAttribute("  ")); //Name is whitespace
        }

        [Fact]
        public static void Ctor_KeyNameValueName_Success()
        {
            XmlConvertKeyValueElementAttribute attribute = new XmlConvertKeyValueElementAttribute("hello", "goodbye");
            Assert.Equal("hello", attribute.KeyName);
            Assert.Equal("goodbye", attribute.ValueName);
        }

        [Fact]
        public static void Ctor_KeyNameValueName_Invalid()
        {
            Assert.Throws<ArgumentNullException>("keyName", () => new XmlConvertKeyValueElementAttribute(null, "hello")); //Key name is null
            Assert.Throws<ArgumentNullException>("valueName", () => new XmlConvertKeyValueElementAttribute("hello", null)); //Value name is null

            Assert.Throws<ArgumentException>("keyName", () => new XmlConvertKeyValueElementAttribute("", "hello")); //Key name is empty
            Assert.Throws<ArgumentException>("keyName", () => new XmlConvertKeyValueElementAttribute("  ", "hello")); //Key name is whitespace
            Assert.Throws<ArgumentException>("valueName", () => new XmlConvertKeyValueElementAttribute("hello", "")); //Value name is empty
            Assert.Throws<ArgumentException>("valueName", () => new XmlConvertKeyValueElementAttribute("hello", "  ")); //Value name is whitespace
        }

        [Fact]
        public static void SerializeDeserialize_AttributeName_Success()
        {
            AttributeNamedObject ano = new AttributeNamedObject("name");

            XElement element = XmlConvert.SerializeXElement(ano);

            Assert.Equal("AttributeIdentifier", element.Name);
            Assert.Equal("name", element.Element("CustomElementIdentifier").Value);

            XmlConvertTest.SerializeDeserializeObject_Equal_Success(ano); //Make sure we can deserialize custom parameters
        }

        [Fact]
        public static void SerializeDeserialize_InterfaceName_Success()
        {
            InterfaceNamedObject ino = new InterfaceNamedObject();

            XElement element = XmlConvert.SerializeXElement(ino);

            Assert.Equal("InterfaceIdentifier", element.Name);
        }
        
        [Fact]
        public static void SerializeDeserialize_AttributeNameInterface_UseInterface()
        {
            //If both an attribute and interface are specified for a class, prefer the interface (it can be more specific and runtime defined)
            AttributeInterfaceNamedObject aino = new AttributeInterfaceNamedObject();

            XElement element = XmlConvert.SerializeXElement(aino);

            Assert.Equal("InterfaceIdentifier", element.Name);
        }

        [Fact]
        public static void SerializeDeserialize_CustomNamedCollection_Success()
        {
            CustomNameCollectionObject cnco = new CustomNameCollectionObject(new Collection<string>
            {
                "1",
                "2",
                "3"
            }, new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" },
            });

            XElement element = XmlConvert.SerializeXElement(cnco);

            XElement collectionElement = element.Element("CollectionValue");
            List<XElement> collectionElements = new List<XElement>(collectionElement.Elements("CollectionElement"));

            Assert.Equal(3, collectionElements.Count);

            XElement dictionaryElement = element.Element("DictionaryValue");
            List<XElement> dictionaryElements = new List<XElement>(dictionaryElement.Elements("DictionaryElement"));

            Assert.Equal(3, dictionaryElements.Count);

            List<XElement> keyElements = new List<XElement>(dictionaryElements.Elements("DictionaryKey"));
            List<XElement> valueElements = new List<XElement>(dictionaryElements.Elements("DictionaryKey"));

            Assert.Equal(3, keyElements.Count);
            Assert.Equal(3, valueElements.Count);

            XmlConvertTest.SerializeDeserializeObject_Equal_Success(cnco);
        }
    }
}
