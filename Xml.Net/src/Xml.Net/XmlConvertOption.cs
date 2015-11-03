using System;

namespace Xml.Net
{
    /// <summary>
    /// Indicates how the objects are formatted, serialized or deserialized.
    /// </summary>
    [Flags]
    public enum XmlConvertOptions
    {
        None = 0,
        ExcludeTypes = 1 << 0
    }
}
