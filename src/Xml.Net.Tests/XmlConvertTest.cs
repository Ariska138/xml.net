using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace Xml.Net.Tests
{
	public static class XmlConvertTests
    {
        private static BasicObject CreateBasicObject()
        {
            return new BasicObject("test");
        }

        private static EmbeddedObject CreateEmbeddedObject()
        {
            return new EmbeddedObject(CreateBasicObject());
        }
            
        private static PrimitiveObject CreatePrimitiveObject()
        {
            return new PrimitiveObject("Test", 'h', 1, 2, 3, 4, 5, 6, 7, 8, 1.022F, 1.2, 1.6M, true, new DateTime(2012, 12, 21));
        }

        private static CollectionObject CreateCollectionObject()
        {
            return new CollectionObject(new Collection<string>
            {
                "a",
                "b",
                "c"
            },
			new List<string>
            {
                "d",
                "e",
                "f"
            },
			new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            });
        }

        private static ICollectionObject CreateICollectionObjectFull()
        {
            return new ICollectionObject(new ArrayList
            {
                "a",
                "b",
                "c"
            },
			new Hashtable
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            });
        }

        private static ICollectionObject CreateICollectionObjectIList()
        {
            return new ICollectionObject(new ArrayList
            {
                "a",
                "b",
                "c"
            },
			new Hashtable());
        }

        private static ICollectionObject CreateICollectionObjectIDictionary()
        {
            return new ICollectionObject(new ArrayList(), new Hashtable
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            });
        }

        private static AdvancedObject CreateAdvancedObject()
        {
            return new AdvancedObject(CreatePrimitiveObject(), CreateCollectionObject(), CreateEmbeddedObject());
        }

        [Theory]
        [InlineData("test")]
        [InlineData("")]
        [InlineData(null)]
        public static void SerializeDeserialize_BasicObject_Success(string text)
        {
            BasicObject bo = new BasicObject(text);
            SerializeDeserializeObject_Equal_Success(bo, XmlConvertOptions.None);
        }
        
		[Fact]
        public static void SerializeDeserialize_EmbeddedObject_Success()
        {
            EmbeddedObject eo = CreateEmbeddedObject();
            SerializeDeserializeObject_Equal_Success(eo, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_PrimitiveObject_Success()
        {
            PrimitiveObject po = CreatePrimitiveObject();
            SerializeDeserializeObject_Equal_Success(po, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_CollectionObject_Success()
        {
            CollectionObject co = CreateCollectionObject();
            SerializeDeserializeObject_Equal_Success(co, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_NonGenericCollectionObjectWithTypeInformation_Success()
        {
            ICollectionObject ico = CreateICollectionObjectFull();
            SerializeDeserializeObject_Equal_Success(ico, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_NonGenericCollectionObjectWithoutTypeInformation_ThrowsException()
        {
            ICollectionObject ico = CreateICollectionObjectIList();
            Assert.Throws<InvalidOperationException>(() => SerializeDeserializeObject_Equal_Success(ico, XmlConvertOptions.ExcludeTypes)); //A non-generic IList without type annotations (can't be resolved)

            ico = CreateICollectionObjectIDictionary();
            Assert.Throws<InvalidOperationException>(() => SerializeDeserializeObject_Equal_Success(ico, XmlConvertOptions.ExcludeTypes)); //A non-generic IDictionary without type annotations (can't be resolved)
        }

        [Fact]
        public static void Deserialize_InvalidDictionaryXml_IgnoresInvalid()
        {
            string xml = @"
<CollectionObject>
    <DictionaryValue>
        <Element>
            <Key>a</Key>
            <Value>1</Value>
        </Element>
        <Element>
            <Key>b</Key>
        </Element>
        <Element>
            <Value>3</Value>
        </Element>
        <Element>
        </Element>
        <Element>
            <Key>e</Key>
            <Value>5</Value>
        </Element>
    </DictionaryValue>
</CollectionObject>";

            CollectionObject co = XmlConvert.DeserializeObject<CollectionObject>(xml);
            Assert.Equal(2, co.DictionaryValue.Count);
        }

        [Fact]
        public static void Deserialize_EmptyDictionaryXml_CreatesEmptyDictionary()
        {
            string xml = @"
<CollectionObject>
    <DictionaryValue>
    </DictionaryValue>
</CollectionObject>";

            CollectionObject co = XmlConvert.DeserializeObject<CollectionObject>(xml);
            Assert.Equal(0, co.DictionaryValue.Count);
        }

        [Fact]
        public static void Deserialize_EmptyCollectionXml_CreatesEmptyCollection()
        {
            string xml = @"
<CollectionObject>
    <CollectionValue>
    </CollectionValue>
</CollectionObject>";

            CollectionObject co = XmlConvert.DeserializeObject<CollectionObject>(xml);
            Assert.Equal(0, co.CollectionValue.Count);
        }

        [Fact]
        public static void Deserialize_NoXmlElement_NullProperty()
        {
            string xml = @"<BasicObject></BasicObject>";

            BasicObject co = XmlConvert.DeserializeObject<BasicObject>(xml);
            Assert.Null(co.StringValue);
        }

        [Fact]
        public static void SerializeDeserialize_AdvancedObject_Success()
        {
            AdvancedObject ao = CreateAdvancedObject();
            SerializeDeserializeObject_Equal_Success(ao, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeObject_Invalid()
        {
            Assert.Throws<ArgumentNullException>("value", () => XmlConvert.SerializeObject(null)); //Object is null
        }

        [Fact]
        public static void DeserializeObject_Invalid()
        {
            Assert.Throws<ArgumentNullException>("type", () => XmlConvert.DeserializeObject(null, "<xml></xml>")); //Type is null
            Assert.Throws<ArgumentNullException>("xml", () => XmlConvert.DeserializeObject(typeof(string), null)); //Xml is null

            Assert.Throws<XmlException>(() => XmlConvert.DeserializeObject(typeof(string), "hello")); //Xml is invalid
        }

        [Fact]
        public static void Deserialize_XElement_Invalid()
        {
            Assert.Throws<ArgumentNullException>("type", () => XmlConvert.DeserializeXElement(null, new XElement("element"))); //Type is null
            Assert.Throws<ArgumentNullException>("element", () => XmlConvert.DeserializeXElement(typeof(string), null)); //Element is null
        }

        public static void SerializeDeserializeObject_Equal_Success<T>(T obj1, XmlConvertOptions options) where T : new()
        {
            T obj2 = default(T);
            T obj3 = default(T);
            string xml = null;

            // Serializing and deserializing strings
            if (options == XmlConvertOptions.None)
            {
                xml = XmlConvert.SerializeObject(obj1);
                obj2 = XmlConvert.DeserializeObject<T>(xml);
                obj3 = (T)XmlConvert.DeserializeObject(obj1.GetType(), xml);
            }
            else
            {
                xml = XmlConvert.SerializeObject(obj1, options);
                obj2 = XmlConvert.DeserializeObject<T>(xml, options);
                obj3 = (T)XmlConvert.DeserializeObject(obj1.GetType(), xml, options);
            }

            Assert.Equal(obj1, obj2);
            Assert.Equal(obj1, obj3);

            // Serializing and deserializing XElements
            XElement element = null;

            if (options == XmlConvertOptions.None)
            {
                element = XmlConvert.SerializeXElement(obj1);
                obj2 = XmlConvert.DeserializeXElement<T>(element);
                obj3 = (T)XmlConvert.DeserializeXElement(obj1.GetType(), element);
            }
            else
            {
                element = XmlConvert.SerializeXElement(obj1, options);
                obj2 = XmlConvert.DeserializeXElement<T>(element, options);
                obj3 = (T)XmlConvert.DeserializeXElement(obj1.GetType(), element, options);
            }

            Assert.Equal(obj1, obj2);
            Assert.Equal(obj1, obj3);
        }
    }
}
