using System;
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
            CustomNameTest.SerializeDeserialize_CustomNamedCollection_Success();
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
                { "c", "3" },
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
            SerializeDeserializeObject_Equal_Success(bo);
        }
        
        public static void SerializeDeserialize_EmbeddedObject_Success(string text)
        {
            EmbeddedObject eo = CreateEmbeddedObject();
            SerializeDeserializeObject_Equal_Success(eo);
        }

        [Fact]
        public static void SerializeDeserialize_PrimitiveObject_Success()
        {
            PrimitiveObject po = CreatePrimitiveObject();
            SerializeDeserializeObject_Equal_Success(po);
        }

        [Fact]
        public static void SerializeDeserialize_CollectionObject_Success()
        {
            CollectionObject co = CreateCollectionObject();
            SerializeDeserializeObject_Equal_Success(co);
        }

        [Fact]
        public static void SerializeDeserialize_AdvancedObject_Success()
        {
            AdvancedObject ao = CreateAdvancedObject();
            SerializeDeserializeObject_Equal_Success(ao);
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

        public static void SerializeDeserializeObject_Equal_Success<T>(T obj1) where T : new()
        {
            T obj2 = default(T);
            string xml = null;

            //Test serialize deserialize string
            Console.WriteLine("Xml.Net String");
            TimeAction(() => xml = XmlConvert.SerializeObject(obj1));
            TimeAction(() => obj2 = XmlConvert.DeserializeObject<T>(xml));

            Assert.Equal(obj1, obj2);

            //Test serialize deserialize XElement
            Console.WriteLine("Xml.Net XElement");
            XElement element = null;

            TimeAction(() => element = XmlConvert.SerializeXElement(obj1));
            TimeAction(() => obj2 = XmlConvert.DeserializeXElement<T>(element));

            Assert.Equal(obj1, obj2);

            try {
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
