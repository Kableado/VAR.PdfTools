namespace VAR.PdfTools.PdfElements
{
    public class PdfName : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Name; } }
        public string Value { get; set; }
    }
}
