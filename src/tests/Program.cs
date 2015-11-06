using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xml.Net;

namespace Tests
{
    public class Test
    {
        public string TestString { get; set; }
        public char TestChar { get; set; }

        public sbyte TestSByte { get; set; }
        public short TestShort { get; set; }
        public int TestInt { get; set; }
        public long TestLong { get; set; }

        public byte TestUByte { get; set; }
        public ushort TestUShort { get; set; }
        public uint TestUInt { get; set; }
        public ulong TestULong { get; set; }

        public float TestFloat { get; set; }
        public double TestDouble { get; set; }
        public decimal TestDecimal { get; set; }

        public bool TestBool { get; set; }

        public DateTime TestDateTime { get; set; }

        [XmlConvertElementsName("Hi")]
        public Collection<string> TestCollection { get; set; }

        [XmlConvertElementsName("DictionaryItem")]
        [XmlConvertKeyValueElement("AKey", "AValue")]
        public Dictionary<string, string> TestDictionary { get; set; }

        [XmlConvertCustomElement("AString")]
        public string CustomNameString { get; set; }

        [XmlConvertIgnored]
        public string IgnoredString { get; set; } = "Ignored";
    }

    class Program
    {
        static Test CreateTestObject()
        {
            var test = new Test();

            test.TestString = "Test";
            test.TestChar = 'h';

            test.TestUByte = 1;
            test.TestUShort = 2;
            test.TestUInt = 3;
            test.TestULong = 4;

            test.TestSByte = 5;
            test.TestShort = 6;
            test.TestInt = 7;
            test.TestLong = 8;

            test.TestFloat = 1.022F;
            test.TestDouble = 1.2;
            test.TestDecimal = 1.6M;

            test.TestBool = true;

            test.TestDateTime = DateTime.Now;

            test.TestCollection = new Collection<string>
            {
                "Obj1",
                "Obj2",
                "Obj3"
            };

            test.TestDictionary = new Dictionary<string, string>
            {
                { "1", "Value1" },
                { "2", "Value2" },
                { "3", "Value3" }
            };

            test.CustomNameString = "Custom Name!";

            return test;
        }
        
        static void Main(string[] args)
        {
            TestSerialization();

            TestCollectionDeserialization();

            Console.ReadLine();
        }

        public static void TestSerialization()
        {
            var testInput = CreateTestObject();

            var xml = XmlConvert.SerializeObject(testInput, XmlConvertOptions.ExcludeTypes);

            var testOutput = XmlConvert.DeserializeObject<Test>(xml);

            Console.WriteLine(xml);

            Console.WriteLine("************");

            Console.WriteLine(testOutput.TestString);
            Console.WriteLine(testOutput.TestChar);

            Console.WriteLine("************");

            Console.WriteLine(testOutput.TestUByte);
            Console.WriteLine(testOutput.TestUShort);
            Console.WriteLine(testOutput.TestUInt);
            Console.WriteLine(testOutput.TestULong);

            Console.WriteLine("************");

            Console.WriteLine(testOutput.TestSByte);
            Console.WriteLine(testOutput.TestShort);
            Console.WriteLine(testOutput.TestInt);
            Console.WriteLine(testOutput.TestLong);

            Console.WriteLine("************");

            Console.WriteLine(testOutput.TestFloat);
            Console.WriteLine(testOutput.TestDouble);
            Console.WriteLine(testOutput.TestDecimal);

            Console.WriteLine("************");

            Console.WriteLine(testOutput.TestBool);

            Console.WriteLine("************");

            Console.WriteLine(testOutput.TestDateTime);

            Console.WriteLine("************");

            Console.WriteLine("Collection:");
            foreach (var obj in testOutput.TestCollection)
            {
                Console.WriteLine(obj);
            }

            Console.WriteLine("************");

            Console.WriteLine("Dictionary:");
            foreach (var obj in testOutput.TestDictionary)
            {
                Console.WriteLine(obj.Key + " : " + obj.Value);
            }
        }

        public class CollectionTestObject 
        {
            public string Obj1 { get; set; }
            public string Obj2 { get; set; }
        }

        public class CollectionTests
        {
            public Collection<CollectionTestObject> TestObjects { get; set; }
        }

        public static void TestCollectionDeserialization()
        {
            var deserializationXml =
@"<CollectionTests>
  <TestObjects>
    <Element>
      <Obj1>Hi</Obj1>
      <Obj2>Yo</Obj2>
    </Element>
  </TestObjects>
</CollectionTests>";
            var deserializationTest = XmlConvert.DeserializeObject<CollectionTests>(deserializationXml);

            Console.WriteLine("************");
            
            Console.WriteLine("Deserialized Collection:");
            foreach (var obj in deserializationTest.TestObjects)
            {
                Console.WriteLine(obj.Obj1.ToString() + ", " + obj.Obj2.ToString());
            }
        }
    }
}
