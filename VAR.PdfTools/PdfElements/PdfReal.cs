namespace VAR.PdfTools.PdfElements
{
    public class PdfReal : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Real; } }
        public double Value { get; set; }
    }
}
