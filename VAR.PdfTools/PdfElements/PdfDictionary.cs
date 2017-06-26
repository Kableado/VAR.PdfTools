using System.Collections.Generic;
using System.IO;

namespace VAR.PdfTools.PdfElements
{
    public class PdfDictionary : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Dictionary; } }
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
                foreach (IPdfElement elem in array.Values)
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
}
