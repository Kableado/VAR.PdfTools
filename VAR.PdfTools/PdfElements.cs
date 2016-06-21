﻿using System.Collections.Generic;
using System.IO;

namespace VAR.PdfTools
{
    public enum PdfElementTypes
    {
        Undefined,
        Boolean,
        Integer,
        Real,
        String,
        Name,
        Array,
        Dictionary,
        Null,
        ObjectReference,
        Object,
        Stream,
    };

    public interface IPdfElement
    {
        PdfElementTypes Type { get; }
    }

    public class PdfBoolean : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Boolean;
        public bool Value { get; set; }
    }

    public class PdfInteger : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Integer;
        public long Value { get; set; }
    }

    public class PdfReal : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Real;
        public double Value { get; set; }
    }

    public class PdfString : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.String;
        public string Value { get; set; }
    }

    public class PdfName : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Name;
        public string Value { get; set; }
    }

    public class PdfArray : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Array;
        private List<IPdfElement> _values = new List<IPdfElement>();
        public List<IPdfElement> Values { get { return _values; } }
    }

    public class PdfDictionary : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Dictionary;
        private Dictionary<string, IPdfElement> _values = new Dictionary<string, IPdfElement>();
        public Dictionary<string, IPdfElement> Values { get { return _values; } }

        public string GetParamAsString(string name)
        {
            if (Values.ContainsKey(name) == false) { return null; }

            IPdfElement value = Values[name];
            if (value is PdfArray)
            {
                value = ((PdfArray)value).Values[0];
            }
            if (value is PdfName)
            {
                return ((PdfName)value).Value;
            }
            if (value is PdfString)
            {
                return ((PdfString)value).Value;
            }
            return null;
        }

        public long? GetParamAsInt(string name)
        {
            if (Values.ContainsKey(name) == false) { return null; }

            IPdfElement value = Values[name];
            if (value is PdfArray)
            {
                value = ((PdfArray)value).Values[0];
            }
            if (value is PdfInteger)
            {
                return ((PdfInteger)value).Value;
            }
            return null;
        }

        public byte[] GetParamAsStream(string name)
        {
            if (Values.ContainsKey(name) == false) { return null; }

            IPdfElement value = Values[name];
            if (value is PdfArray)
            {
                PdfArray array = value as PdfArray;
                MemoryStream memStream = new MemoryStream();
                foreach(IPdfElement elem in array.Values)
                {
                    PdfStream stream = elem as PdfStream;
                    if (stream == null) { continue; }
                    memStream.Write(stream.Data, 0, stream.Data.Length);
                }
                if (memStream.Length > 0)
                {
                    return memStream.ToArray();
                }
                return null;
            }
            if (value is PdfStream)
            {
                return ((PdfStream)value).Data;
            }
            return null;
        }

    }

    public class PdfNull : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Null;
    }

    public class PdfObjectReference : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.ObjectReference;
        public int ObjectID { get; set; }
        public int ObjectGeneration { get; set; }
    }

    public class PdfStream : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Stream;
        public PdfDictionary Dictionary { get; set; }
        public byte[] Data { get; set; }

        public byte[] OriginalData { get; set; }
        public IPdfElement OriginalFilter { get; set; }
    }

    public class PdfObject : IPdfElement
    {
        public PdfElementTypes Type { get; private set; } = PdfElementTypes.Object;
        public int ObjectID { get; set; }
        public int ObjectGeneration { get; set; }
        public IPdfElement Data { get; set; }
        public int UsageCount { get; set; } = 0;
    }

    public static class PdfElementUtils
    {
        public static double GetReal(IPdfElement elem, double defaultValue)
        {
            if(elem == null)
            {
                return defaultValue;
            }
            if (elem is PdfInteger)
            {
                return ((PdfInteger)elem).Value;
            }
            if (elem is PdfReal)
            {
                return ((PdfReal)elem).Value;
            }
            return defaultValue;
        }
        public static long GetInt(IPdfElement elem, long defaultValue)
        {
            if (elem == null)
            {
                return defaultValue;
            }
            if (elem is PdfInteger)
            {
                return ((PdfInteger)elem).Value;
            }
            if (elem is PdfReal)
            {
                return (long)((PdfReal)elem).Value;
            }
            return defaultValue;
        }
    }
}
