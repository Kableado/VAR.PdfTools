using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

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

        #region Private methods

        private static byte[] DecodeFlateStreamData(byte[] streamData)
        {
            MemoryStream msInput = new MemoryStream(streamData);
            MemoryStream msOutput = new MemoryStream();

            // It seems to work when skipping the first two bytes.
            byte header;   // 0x30 0x59
            header = (byte)msInput.ReadByte();
            //Debug.Assert(header == 48);
            header = (byte)msInput.ReadByte();
            //Debug.Assert(header == 89);
            DeflateStream zip = new DeflateStream(msInput, CompressionMode.Decompress, true);
            int cbRead;
            byte[] abResult = new byte[1024];
            do
            {
                cbRead = zip.Read(abResult, 0, abResult.Length);
                if (cbRead > 0)
                    msOutput.Write(abResult, 0, cbRead);
            }
            while (cbRead > 0);
            zip.Close();
            msOutput.Flush();
            if (msOutput.Length >= 0)
            {
                msOutput.Capacity = (int)msOutput.Length;
                return msOutput.GetBuffer();
            }
            return null;
        }

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

            // Apply filters to streams
            foreach(PdfObject obj in doc.Objects)
            {
                if(obj.Data.Type != PdfElementTypes.Stream) { continue; }
                PdfStream stream = obj.Data as PdfStream;

                string filter = stream.GetParamAsString("Filter");
                if (filter == "FlateDecode")
                {
                    stream.OriginalData = stream.Data;
                    stream.OriginalFilter = stream.Dictionary.Values["Filter"];
                    byte[] decodedStreamData = DecodeFlateStreamData(stream.Data);
                    stream.Data = decodedStreamData;
                    stream.Dictionary.Values["Length"] = new PdfInteger { Value = decodedStreamData.Length };
                    stream.Dictionary.Values.Remove("Filter");
                }
            }

            return doc;
        }

        #endregion

    }
}
