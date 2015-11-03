using System;

namespace Xml.Net
{
    [Flags]
    public enum XmlConvertOptions
    {
        None = 0,
        ExcludeTypes = 1 << 0
    }
}
