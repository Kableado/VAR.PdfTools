using System.Collections.Generic;

namespace VAR.PdfTools.PdfElements
{
    public class PdfArray : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Array; } }
        private List<IPdfElement> _values = new List<IPdfElement>();
        public List<IPdfElement> Values { get { return _values; } }
    }
}
