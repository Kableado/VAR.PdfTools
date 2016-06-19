using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace VAR.PdfTools
{
    public class PdfDocument
    {
        #region Declarations

        private List<PdfObject> _objects = new List<PdfObject>();

        private PdfDictionary _catalog = null;

        private List<PdfDocumentPage> _pages = new List<PdfDocumentPage>();

        #endregion

        #region Properties

        public List<PdfObject> Objects { get { return _objects; } }

        public PdfDictionary Catalog { get { return _catalog; } }

        public List<PdfDocumentPage> Pages { get { return _pages; } }

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

        private static void ApplyFiltersToStreams(PdfStream stream)
        {
            string filter = stream.Dictionary.GetParamAsString("Filter");
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

        private static IPdfElement ResolveIndirectReferences(IPdfElement elem, Dictionary<int, PdfObject> dictReferences)
        {
            if (elem is PdfObjectReference)
            {
                int objectId = ((PdfObjectReference)elem).ObjectID;
                if (dictReferences.ContainsKey(objectId))
                {
                    PdfObject referencedObject = dictReferences[objectId];
                    referencedObject.UsageCount++;
                    return referencedObject.Data;
                }
                else
                {
                    return new PdfNull();
                }
            }

            PdfObject obj = elem as PdfObject;
            if (obj != null)
            {
                IPdfElement result = ResolveIndirectReferences(obj.Data, dictReferences);
                if (result != obj.Data)
                {
                    obj.Data = result;
                }
                return elem;
            }

            PdfArray array = elem as PdfArray;
            if (array != null)
            {
                for (int i = 0; i < array.Values.Count; i++)
                {
                    IPdfElement result = ResolveIndirectReferences(array.Values[i], dictReferences);
                    if(result != array.Values[i])
                    {
                        array.Values[i] = result;
                    }
                }
                return elem;
            }
            
            PdfDictionary dict = elem as PdfDictionary;
            if (dict != null)
            {
                List<string> keys = dict.Values.Keys.ToList();
                foreach (string key in keys)
                {
                    IPdfElement value = dict.Values[key];
                    IPdfElement result = ResolveIndirectReferences(value, dictReferences);
                    if (result != value)
                    {
                        dict.Values[key] = result;
                    }
                }
                return elem;
            }
            
            return elem;
        }

        private static void ExtractPages(PdfDictionary page, PdfDocument doc)
        {
            string type = page.GetParamAsString("Type");
            if (type == "Page")
            {
                PdfDocumentPage prevDocPage = null;
                if (doc._pages.Count > 0)
                {
                    prevDocPage = doc._pages.Last();
                }
                PdfDocumentPage docPage = new PdfDocumentPage(page, prevDocPage);
                doc._pages.Add(docPage);
                return;
            }
            else if (type == "Pages")
            {
                if (page.Values.ContainsKey("Kids") == false || (page.Values["Kids"] is PdfArray) == false)
                {
                    throw new Exception("PdfDocument: Pages \"Kids\" not found");
                }
                PdfArray kids = page.Values["Kids"] as PdfArray;
                foreach (IPdfElement elem in kids.Values)
                {
                    PdfDictionary childPage = elem as PdfDictionary;
                    if (page == null) { continue; }
                    ExtractPages(childPage, doc);
                }
            }
            else
            {
                throw new Exception(string.Format("PdfDocument: Unexpected page type, found: {0}", type));
            }
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
            var doc = new PdfDocument();

            // Parse data
            var parser = new PdfParser(data);
            do
            {
                PdfObject obj = parser.ParseObject();
                if (obj != null)
                {
                    if (obj.Data is PdfStream)
                    {
                        ApplyFiltersToStreams((PdfStream)obj.Data);
                    }
                    doc.Objects.Add(obj);
                }
            } while (parser.IsEndOfStream() == false);

            // Expand Object Streams
            List<PdfObject> streamObjects = new List<PdfObject>();
            foreach (PdfObject obj in doc.Objects)
            {
                if (obj.Data.Type != PdfElementTypes.Stream) { continue; }
                PdfStream stream = obj.Data as PdfStream;

                string type = stream.Dictionary.GetParamAsString("Type");
                long? number = stream.Dictionary.GetParamAsInt("N");
                long? first = stream.Dictionary.GetParamAsInt("First");
                if (type == "ObjStm" && number != null && first != null)
                {
                    obj.UsageCount++;
                    PdfParser parserAux = new PdfParser(stream.Data);
                    streamObjects.AddRange(parserAux.ParseObjectStream((int)number, (long)first));
                }
            }
            foreach (PdfObject obj in streamObjects)
            {
                doc.Objects.Add(obj);
            }

            // Build cross reference table
            Dictionary<int, PdfObject> dictObjects = new Dictionary<int, PdfObject>();
            foreach (PdfObject obj in doc.Objects)
            {
                if (dictObjects.ContainsKey(obj.ObjectID))
                {
                    if (dictObjects[obj.ObjectID].ObjectGeneration < obj.ObjectGeneration)
                    {
                        dictObjects[obj.ObjectID] = obj;
                    }
                }
                else
                {
                    dictObjects.Add(obj.ObjectID, obj);
                }
            }

            // Iterate full document to resolve all indirect references
            foreach(PdfObject obj in doc.Objects)
            {
                ResolveIndirectReferences(obj, dictObjects);
            }

            // Search Catalog
            foreach(PdfObject obj in doc.Objects)
            {
                if ((obj.Data is PdfDictionary) == false) { continue; }
                string type = ((PdfDictionary)obj.Data).GetParamAsString("Type");
                if(type == "Catalog")
                {
                    doc._catalog = (PdfDictionary)obj.Data;
                    break;
                }

            }
            if(doc._catalog == null)
            {
                throw new Exception("PdfDocument: Catalog not found");
            }

            // Search pages
            if(doc.Catalog.Values.ContainsKey("Pages") == false || 
                (doc.Catalog.Values["Pages"] is PdfDictionary) == false)
            {
                throw new Exception("PdfDocument: Pages not found");
            }
            PdfDictionary pages = (PdfDictionary)doc.Catalog.Values["Pages"];
            ExtractPages(pages, doc);

            return doc;
        }

        #endregion

    }
}
