using System;

namespace Xml.Net
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertDictionaryElementNamesAttribute : Attribute
    {
        public XmlConvertDictionaryElementNamesAttribute(string keyName, string valueName)
        {
            if (keyName == null) { throw new ArgumentNullException(nameof(valueName)); }
            if (keyName.Length == 0) { throw new ArgumentException("The dictionary key element name cannot be empty"); }

            if (valueName == null) { throw new ArgumentNullException(nameof(valueName)); }
            if (valueName.Length == 0) { throw new ArgumentException("The dictionary value element name cannot be empty"); }

            KeyName = keyName;
            ValueName = valueName;
        }

        public string KeyName { get; }
        public string ValueName { get; }
    }
}
