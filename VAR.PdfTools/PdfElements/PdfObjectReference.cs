namespace VAR.PdfTools.PdfElements
{
    public class PdfObjectReference : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.ObjectReference; } }
        public int ObjectID { get; set; }
        public int ObjectGeneration { get; set; }
    }
}
