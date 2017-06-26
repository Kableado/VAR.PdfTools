namespace VAR.PdfTools.PdfElements
{
    public class PdfObject : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Object; } }
        public int ObjectID { get; set; }
        public int ObjectGeneration { get; set; }
        public IPdfElement Data { get; set; }
        public int UsageCount { get; set; }
    }
}
