using System;

namespace Xml.Net
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class XmlConvertCustomElementAttribute : Attribute
    {
        public XmlConvertCustomElementAttribute(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (name.Length == 0) { throw new ArgumentException("The xml object name cannot be empty"); }

            Name = name;
        }

        public string Name { get; }
    }
}
