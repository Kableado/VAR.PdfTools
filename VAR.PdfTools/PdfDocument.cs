using System.Collections.Generic;
using System.IO;

namespace VAR.PdfTools
{
    public class PdfDocument
    {
        #region Declarations

        private List<PdfObject> _objects = new List<PdfObject>();

        #endregion

        #region Properties

        public List<PdfObject> Objects { get { return _objects; } }

        #endregion

        #region Life cycle

        private PdfDocument() { }

        #endregion

        #region Public methods

        public static PdfDocument Load(string filename)
        {
            byte[] fileBytes = File.ReadAllBytes(filename);
            return Load(fileBytes);
        }

        public static PdfDocument Load(byte[] data)
        {
            var parser = new PdfParser(data);
            var doc = new PdfDocument();
            do
            {
                PdfObject obj = parser.ParseObject();
                if (obj != null)
                {
                    doc.Objects.Add(obj);
                }
            } while (parser.IsEndOfStream() == false);
            return doc;
        }

        #endregion

    }
}
