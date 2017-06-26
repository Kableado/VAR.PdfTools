namespace VAR.PdfTools.PdfElements
{
    public class PdfBoolean : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Boolean; } }
        public bool Value { get; set; }
    }
}
