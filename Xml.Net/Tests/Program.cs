using System;
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

        public Collection<string> TestCollection { get; set; }

        [XmlConvertCustomName("AString")]
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
                "hi",
                "yo",
                "test"
            };

            test.CustomNameString = "Custom Name!";

            return test;
        }
        
        static void Main(string[] args)
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
            
            Console.ReadLine();
        }
    }
}
