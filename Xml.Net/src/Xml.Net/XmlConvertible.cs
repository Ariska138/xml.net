namespace Xml.Net
{
    /// <summary>
    /// The interface that specifies a custom XML identifier for serialized elements - this overrides any attributes on properties of the class.
    /// </summary>
    public interface XmlConvertible
    {
        string XmlIdentifier { get; }
    }
}
