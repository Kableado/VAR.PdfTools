namespace VAR.PdfTools.PdfElements
{
    public class PdfStream : IPdfElement
    {
        public PdfElementTypes Type { get { return PdfElementTypes.Stream; } }
        public PdfDictionary Dictionary { get; set; }
        public byte[] Data { get; set; }

        public byte[] OriginalData { get; set; }
        public IPdfElement OriginalFilter { get; set; }
    }
}
