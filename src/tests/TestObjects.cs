using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xml.Net.Tests
{
    public class BasicObject
    {
        public BasicObject() { }

        public BasicObject(string s)
        {
            StringValue = s;
        }

        public string StringValue { get; set; }

        public override bool Equals(object obj)
        {
            BasicObject bo = (BasicObject)obj;

            if (bo == null)
            {
                return false;
            }

            if (StringValue == null && bo.StringValue == null)
            {
                return true;
            }

            return StringValue != null && StringValue.Equals(bo.StringValue);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
        
    public class EmbeddedObject
    {
        public EmbeddedObject() { }

        public EmbeddedObject(BasicObject bo)
        {
            BasicObjectValue = bo;
        }

        public BasicObject BasicObjectValue { get; set; }

        public override bool Equals(object obj)
        {
            EmbeddedObject eo = (EmbeddedObject)obj;

            if (eo == null)
            {
                return false;
            }

            if (BasicObjectValue == null && eo.BasicObjectValue == null)
            {
                return true;
            }

            return BasicObjectValue != null && BasicObjectValue.Equals(eo.BasicObjectValue);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AdvancedObject
    {
        public AdvancedObject() { }

        public AdvancedObject(PrimitiveObject po, CollectionObject co, EmbeddedObject eo)
        {
            PrimitiveObjectValue = po;
            CollectionObjectValue = co;
            EmbeddedObjectValue = eo;
        }

        public PrimitiveObject PrimitiveObjectValue { get; set; }
        public CollectionObject CollectionObjectValue { get; set; }
        public EmbeddedObject EmbeddedObjectValue { get; set; }

        public override bool Equals(object obj)
        {
            AdvancedObject ao = (AdvancedObject)obj;

            if (ao == null)
            {
                return false;
            }

            if (PrimitiveObjectValue == null && ao.PrimitiveObjectValue == null &&
                CollectionObjectValue == null && ao.CollectionObjectValue == null &&
                EmbeddedObjectValue == null && ao.EmbeddedObjectValue == null)
            {
                return true;
            }
            
            return 
                PrimitiveObjectValue != null && PrimitiveObjectValue.Equals(ao.PrimitiveObjectValue) &&
                CollectionObjectValue != null && CollectionObjectValue.Equals(ao.CollectionObjectValue) &&
                EmbeddedObjectValue != null && EmbeddedObjectValue.Equals(ao.EmbeddedObjectValue);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class PrimitiveObject
    {
        public PrimitiveObject() { }

        public PrimitiveObject(string s, char c, sbyte i8, short i16, int i32, long i64, byte u8, ushort u16, uint u32, ulong u64, float f, double d, decimal dec, bool b, DateTime dt)
        {
            StringValue = s;
            CharValue = c;

            SByteValue = i8;
            ShortValue = i16;
            IntValue = i32;
            LongValue = i64;

            UByteValue = u8;
            UShortValue = u16;
            UIntValue = u32;
            ULongValue = u64;

            FloatValue = f;
            DoubleValue = d;
            DecimalValue = dec;

            BoolValue = b;

            DateTimeValue = dt;
        }

        public string StringValue { get; set; }
        public char CharValue { get; set; }

        public sbyte SByteValue { get; set; }
        public short ShortValue { get; set; }
        public int IntValue { get; set; }
        public long LongValue { get; set; }

        public byte UByteValue { get; set; }
        public ushort UShortValue { get; set; }
        public uint UIntValue { get; set; }
        public ulong ULongValue { get; set; }

        public float FloatValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }

        public bool BoolValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public override bool Equals(object obj)
        {
            PrimitiveObject po = (PrimitiveObject)obj;

            if (po == null)
            {
                return false;
            }

            if (StringValue == null && po.StringValue == null)
            {
                return true;
            }

            return 
                StringValue != null && StringValue == po.StringValue &&
                CharValue == po.CharValue &&
                SByteValue == po.SByteValue &&
                ShortValue == po.ShortValue &&
                IntValue == po.IntValue &&
                LongValue == po.LongValue &&
                UByteValue == po.UByteValue &&
                UShortValue == po.UShortValue &&
                UIntValue == po.UIntValue &&
                ULongValue == po.ULongValue &&
                FloatValue == po.FloatValue &&
                DoubleValue == po.DoubleValue &&
                DecimalValue == po.DecimalValue &&
                BoolValue == po.BoolValue &&
                DateTimeValue == DateTimeValue;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CollectionObject
    {
        public CollectionObject() { }

        public CollectionObject(Collection<string> c, List<string> l, Dictionary<string, string> d)
        {
            CollectionValue = c;
            ListValue = l;
            DictionaryValue = d;
        }

        public Collection<string> CollectionValue { get; set; }
        public List<string> ListValue { get; set; }
        public Dictionary<string, string> DictionaryValue { get; set; }

        public override bool Equals(object obj)
        {
            CollectionObject co = (CollectionObject)obj;

            if (co == null)
            {
                return false;
            }

            if (CollectionValue == null && co.CollectionValue == null &&
                ListValue == null && co.ListValue == null &&
                DictionaryValue == null && co.DictionaryValue == null)
            {
                return true;
            }

            return
                CollectionValue != null && CollectionValue.Count.Equals(co.CollectionValue.Count) &&
                ListValue != null && ListValue.Count.Equals(co.ListValue.Count) &&
                DictionaryValue != null && DictionaryValue.Count.Equals(co.DictionaryValue.Count);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [XmlConvertCustomElement("AttributeIdentifier")]
    public class AttributeNamedObject
    {
        public AttributeNamedObject()
        {
        }

        public AttributeNamedObject(string stringValue)
        {
            StringValue = stringValue;
        }

        [XmlConvertCustomElement("CustomElementIdentifier")]
        public string StringValue { get; set; }

        public override bool Equals(object obj)
        {
            AttributeNamedObject anc = (AttributeNamedObject)obj;

            if (anc == null)
            {
                return false;
            }

            if (StringValue == null && anc.StringValue == null)
            {
                return true;
            }

            return StringValue != null && StringValue.Equals(anc.StringValue);
        }

        public override int GetHashCode()
        {
            return StringValue.GetHashCode();
        }
    }

    public class InterfaceNamedObject : IXmlConvertible
    {
        public string XmlIdentifier => "InterfaceIdentifier";
    }

    [XmlConvertCustomElement("AttributeIdentifier")]
    public class AttributeInterfaceNamedObject : IXmlConvertible
    {
        public string XmlIdentifier => "InterfaceIdentifier";
    }

    public class CustomNameCollectionObject
    {
        public CustomNameCollectionObject() { }

        public CustomNameCollectionObject(Collection<string> c, Dictionary<string, string> d)
        {
            CollectionValue = c;
            DictionaryValue = d;
        }

        [XmlConvertElementsName("CollectionElement")]
        public Collection<string> CollectionValue { get; set; }

        [XmlConvertElementsName("DictionaryElement")]
        [XmlConvertKeyValueElement("DictionaryKey", "DictionaryValue")]
        public Dictionary<string, string> DictionaryValue { get; set; }

        public override bool Equals(object obj)
        {
            CustomNameCollectionObject co = (CustomNameCollectionObject)obj;

            if (co == null)
            {
                return false;
            }

            if (CollectionValue == null && co.CollectionValue == null &&
                DictionaryValue == null && co.DictionaryValue == null)
            {
                return true;
            }

            return
                CollectionValue != null && CollectionValue.Count.Equals(co.CollectionValue.Count) &&
                DictionaryValue != null && DictionaryValue.Count.Equals(co.DictionaryValue.Count);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class IgnoredPropertyObject
    {
        public IgnoredPropertyObject()
        {
        }

        [XmlConvertIgnored]
        public string Value { get; set; }
    }
}
