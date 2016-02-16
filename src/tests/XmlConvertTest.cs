using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using Xunit;

namespace Xml.Net.Tests
{
    public static class XmlConvertTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("PRIMITIVE:");
            SerializeDeserialize_PrimitiveObject_Success();

            Console.WriteLine();
            Console.WriteLine("ADVANCED:");
            SerializeDeserialize_AdvancedObject_Success();

            Console.ReadLine();
        }

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
            }, new List<string>
            {
                "d",
                "e",
                "f"
            }, new Dictionary<string, string>
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
            }, new Hashtable
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
            }, new Hashtable());
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
            //Test with a simple object
            BasicObject bo = new BasicObject(text);
            SerializeDeserializeObject_Equal_Success(bo, XmlConvertOptions.None);
        }
        
        public static void SerializeDeserialize_EmbeddedObject_Success(string text)
        {
            //Test with an object containing another object
            EmbeddedObject eo = CreateEmbeddedObject();
            SerializeDeserializeObject_Equal_Success(eo, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_PrimitiveObject_Success()
        {
            //Test with primitive objects
            PrimitiveObject po = CreatePrimitiveObject();
            SerializeDeserializeObject_Equal_Success(po, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_CollectionObject_Success()
        {
            //Test with a collection, list and dictionary
            CollectionObject co = CreateCollectionObject();
            SerializeDeserializeObject_Equal_Success(co, XmlConvertOptions.None);
        }

        [Fact]
        public static void SerializeDeserialize_NonGenericCollectionObjectWithTypeInformation_Success()
        {
            //Test with an IDictionary
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
            //Test with XML from a serialized dictionary that I have made invalid (no key and/or value element).
            string xml = @"<CollectionObject>
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
            //Test with XML with an empty element.
            string xml = @"<CollectionObject>
    <DictionaryValue>
    </DictionaryValue>
</CollectionObject>";

            CollectionObject co = XmlConvert.DeserializeObject<CollectionObject>(xml);
            Assert.Equal(0, co.DictionaryValue.Count);
        }

        [Fact]
        public static void Deserialize_EmptyCollectionXml_CreatesEmptyCollection()
        {
            //Test with XML with an empty element.
            string xml = @"<CollectionObject>
    <CollectionValue>
    </CollectionValue>
</CollectionObject>";

            CollectionObject co = XmlConvert.DeserializeObject<CollectionObject>(xml);
            Assert.Equal(0, co.CollectionValue.Count);
        }

        [Fact]
        public static void Deserialize_NoXmlElement_NullProperty()
        {
            //Test with XML with no element.
            string xml = @"<BasicObject>
</BasicObject>";

            BasicObject co = XmlConvert.DeserializeObject<BasicObject>(xml);
            Assert.Null(co.StringValue);
        }

        [Fact]
        public static void SerializeDeserialize_AdvancedObject_Success()
        { 
            //Test with an object with embedded objects (which also have embedded objects)
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

            //Test serialize deserialize string
            Console.WriteLine("Xml.Net String");

            if (options == XmlConvertOptions.None)
            {
                TimeAction(() => xml = XmlConvert.SerializeObject(obj1));
                TimeAction(() => obj2 = XmlConvert.DeserializeObject<T>(xml));
                TimeAction(() => obj3 = (T)XmlConvert.DeserializeObject(obj1.GetType(), xml));
            }
            else
            {
                TimeAction(() => xml = XmlConvert.SerializeObject(obj1, options));
                TimeAction(() => obj2 = XmlConvert.DeserializeObject<T>(xml, options));
                TimeAction(() => obj3 = (T)XmlConvert.DeserializeObject(obj1.GetType(), xml, options));
            }

            Assert.Equal(obj1, obj2);
            Assert.Equal(obj1, obj3);

            //Test serialize deserialize XElement
            Console.WriteLine("Xml.Net XElement");
            XElement element = null;

            if (options == XmlConvertOptions.None)
            {
                TimeAction(() => element = XmlConvert.SerializeXElement(obj1));
                TimeAction(() => obj2 = XmlConvert.DeserializeXElement<T>(element));
                TimeAction(() => obj3 = (T)XmlConvert.DeserializeXElement(obj1.GetType(), element));
            }
            else
            {
                TimeAction(() => element = XmlConvert.SerializeXElement(obj1, options));
                TimeAction(() => obj2 = XmlConvert.DeserializeXElement<T>(element, options));
                TimeAction(() => obj3 = (T)XmlConvert.DeserializeXElement(obj1.GetType(), element, options));
            }

            Assert.Equal(obj1, obj2);
            Assert.Equal(obj1, obj3);

            try
            {
                //Test XmlSerializer
                Console.WriteLine("XmlSerializer");
                TimeAction(() =>
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(obj1.GetType());

                    using (StringWriter textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, obj1);
                        xml = textWriter.ToString();
                    }
                });

                TimeAction(() =>
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(obj1.GetType());

                    using (StringReader textReader = new StringReader(xml))
                    {
                        obj2 = (T)xmlSerializer.Deserialize(textReader);
                    }
                });

                Assert.Equal(obj1, obj2);
            }
            catch (InvalidOperationException)
            {
                //Error using XmlSerializer
                Console.WriteLine("Could not serialize or deserialize.");
            }
        }

        private static void TimeAction(Action action)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            action();

            s.Stop();
            Console.WriteLine("Action took " + s.Elapsed.ToString() + " ms to run");
        }
    }
}
