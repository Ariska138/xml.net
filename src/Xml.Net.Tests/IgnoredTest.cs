using System.Xml.Linq;
using Xunit;

namespace Xml.Net.Tests
{
    public static class IgnoredTest
    {
        public static void Ctor_Empty_Success()
        {
            XmlConvertIgnoredAttribute attribute = new XmlConvertIgnoredAttribute();
        }

        [Fact]
        public static void SerializeDeserialize_Ignored_Success()
        {
            IgnoredPropertyObject ipo = new IgnoredPropertyObject();
            XElement element = XmlConvert.SerializeXElement(ipo);

			Assert.Null(element.Element("IgnoredValue"));

            Assert.Null(element.Element("NoGetterValue"));
            Assert.Null(element.Element("NoSetterValue"));
        }
    }
}
