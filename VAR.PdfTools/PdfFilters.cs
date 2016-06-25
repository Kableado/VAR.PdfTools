using System;
using System.IO;
using System.IO.Compression;

namespace VAR.PdfTools
{
    public static class PdfFilters
    {
        public class FlateDecode
        {
            public byte[] Encode(byte[] streamData)
            {
                throw new NotImplementedException("FlateFilter.Encode: Not implemented");
            }

            public static byte[] Decode(byte[] streamData)
            {
                MemoryStream msInput = new MemoryStream(streamData);
                MemoryStream msOutput = new MemoryStream();

                // It seems to work when skipping the first two bytes.
                byte header;
                header = (byte)msInput.ReadByte();
                header = (byte)msInput.ReadByte();

                DeflateStream zip = new DeflateStream(msInput, CompressionMode.Decompress, true);
                int cbRead;
                byte[] abResult = new byte[1024];
                do
                {
                    cbRead = zip.Read(abResult, 0, abResult.Length);
                    if (cbRead > 0)
                    {
                        msOutput.Write(abResult, 0, cbRead);
                    }
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
        }

    }
}
