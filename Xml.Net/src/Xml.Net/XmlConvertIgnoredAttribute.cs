using System;

namespace Xml.Net
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertIgnoredAttribute : Attribute
    {
    }
}
