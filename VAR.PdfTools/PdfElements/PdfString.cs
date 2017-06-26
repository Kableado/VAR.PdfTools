namespace VAR.PdfTools.PdfElements
{
    public class PdfString : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.String; } }
        public string Value { get; set; }
    }
}
