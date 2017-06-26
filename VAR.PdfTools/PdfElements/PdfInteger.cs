namespace VAR.PdfTools.PdfElements
{
    public class PdfInteger : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Integer; } }
        public long Value { get; set; }
    }
}
