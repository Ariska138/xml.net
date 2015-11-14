using System;
using System.Xml.Linq;
using Xunit;

namespace Xml.Net.Tests
{
    public static class CustomNameTest
    {
        public static void Ctor_Name_Success()
        {
            XmlConvertCustomElementAttribute element = new XmlConvertCustomElementAttribute("hello");
            Assert.Equal("hello", element.Name);
        }

        public static void Ctor_Name_Invalid()
        {
            Assert.Throws<ArgumentNullException>("name", () => new XmlConvertCustomElementAttribute(null)); //Name is null

            Assert.Throws<ArgumentException>("name", () => new XmlConvertCustomElementAttribute("")); //Name is empty
            Assert.Throws<ArgumentException>("name", () => new XmlConvertCustomElementAttribute("  ")); //Name is whitespace
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
    }
}
