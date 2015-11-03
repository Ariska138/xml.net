using System;

namespace Xml.Net
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertElementNameAttribute : Attribute
    {
        public XmlConvertElementNameAttribute(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (name.Length == 0) { throw new ArgumentException("The xml object name cannot be empty"); }

            Name = name;
        }

        public string Name { get; }
    }
}
