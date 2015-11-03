using System;

namespace Xml.Net
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertIgnoredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertCustomNameAttribute : Attribute
    {
        public XmlConvertCustomNameAttribute(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (name.Length == 0) { throw new ArgumentException("The xml object name cannot be empty"); }

            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertCustomColectionElementNameAttribute : Attribute
    {
        public XmlConvertCustomColectionElementNameAttribute(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (name.Length == 0) { throw new ArgumentException("The xml custom collection name cannot be empty"); }

            Name = name;
        }

        public string Name { get; }
    }
}
