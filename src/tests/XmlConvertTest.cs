using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using Xunit;

namespace Xml.Net.Tests
{
    public static class XmlConvertTest
    {
        public static void Main(string[] args)
        {
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

        private static void SerializeDeserializeObject_Equal_Success<T>(T obj1) where T : new()
        {
            string xml = default(string);
            T obj2 = default(T);

            Console.WriteLine("Xml.Net");
            TimeAction(() => xml = XmlConvert.SerializeObject(obj1));
            TimeAction(() => obj2 = XmlConvert.DeserializeObject<T>(xml));

            Assert.True(obj1.Equals(obj2));

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

        [Fact]
        public static void SerializeObject_Invalid()
        {
            Assert.Throws<ArgumentNullException>("value", () => XmlConvert.SerializeObject(null)); //Object is null
            Assert.Throws<ArgumentException>("value", () => XmlConvert.SerializeObject("string")); //Object is a fundamental primitive
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
