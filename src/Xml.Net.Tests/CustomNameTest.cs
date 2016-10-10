using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
		public static void Ctor_NullName_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("name", () => new XmlConvertCustomElementAttribute(null));
			Assert.Throws<ArgumentNullException>("name", () => new XmlConvertElementsNameAttribute(null));
			Assert.Throws<ArgumentNullException>("keyName", () => new XmlConvertKeyValueElementAttribute(null, "Valid"));
			Assert.Throws<ArgumentNullException>("valueName", () => new XmlConvertKeyValueElementAttribute("Valid", null));
		}

		[Theory]
		[InlineData("")]
		[InlineData(" \r \t \n")]
		public static void Ctor_InvalidName_ThrowsArgumentException(string name)
		{
			Assert.Throws<ArgumentException>("name", () => new XmlConvertCustomElementAttribute(name));
			Assert.Throws<ArgumentException>("name", () => new XmlConvertElementsNameAttribute(name));
			Assert.Throws<ArgumentException>("keyName", () => new XmlConvertKeyValueElementAttribute(name, "Valid"));
			Assert.Throws<ArgumentException>("valueName", () => new XmlConvertKeyValueElementAttribute("Valid", name));
		}

        [Fact]
        public static void Ctor_ElementsName_Success()
        {
            XmlConvertElementsNameAttribute attribute = new XmlConvertElementsNameAttribute("hello");
            Assert.Equal("hello", attribute.Name);
        }

        [Fact]
        public static void Ctor_KeyNameValueName_Success()
        {
            XmlConvertKeyValueElementAttribute attribute = new XmlConvertKeyValueElementAttribute("hello", "goodbye");
            Assert.Equal("hello", attribute.KeyName);
            Assert.Equal("goodbye", attribute.ValueName);
        }

        [Fact]
        public static void SerializeDeserialize_AttributeName_Success()
        {
            AttributeNamedObject ano = new AttributeNamedObject("name");
            XElement element = XmlConvert.SerializeXElement(ano);

            Assert.Equal("AttributeIdentifier", element.Name);
            Assert.Equal("name", element.Element("CustomElementIdentifier").Value);

			XmlConvertTests.SerializeDeserializeObject_Equal_Success(ano, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_InterfaceName_Success()
        {
            InterfaceNamedObject ino = new InterfaceNamedObject();
            XElement element = XmlConvert.SerializeXElement(ino);
            Assert.Equal("InterfaceIdentifier", element.Name);
        }
        
        [Fact]
        public static void SerializeDeserialize_BothAttributeNameAndInterface_PrefersInterface()
        {
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
            },
			new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" },
            });

            XElement element = XmlConvert.SerializeXElement(cnco);

            XElement collectionElement = element.Element("CollectionValue");
            Assert.Equal(3, collectionElement.Elements("CollectionElement").Count());

            XElement dictionaryElement = element.Element("DictionaryValue");
            List<XElement> dictionaryElements = new List<XElement>(dictionaryElement.Elements("DictionaryElement"));
            Assert.Equal(3, dictionaryElements.Count);
			
            Assert.Equal(3, dictionaryElements.Elements("DictionaryKey").Count());
            Assert.Equal(3, dictionaryElements.Elements("DictionaryKey").Count());

            XmlConvertTests.SerializeDeserializeObject_Equal_Success(cnco, XmlConvertOptions.None);
        }
    }
}
