using System;
using System.Collections.Generic;
using System.IO;
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
        
        private static void ApplyFilterToStream(PdfStream stream, string filter)
        {
            if (filter == "FlateDecode")
            {
                byte[] decodedStreamData = PdfFilters.FlateDecode.Decode(stream.Data);
                stream.Data = decodedStreamData;
            }
            else if (filter == "ASCII85Decode" || filter == "A85")
            {
                // FIXME: Implement this filter
            }
            else if (filter == "CCITTFaxDecode")
            {
                // FIXME: Implement this filter
            }
            else if (filter == "DCTDecode")
            {
                // FIXME: Implement this filter
            }
            else
            {
                // FIXME: Implement the rest of filters
            }
        }

        private static void ApplyFiltersToStreams(PdfStream stream)
        {
            if (stream.Dictionary.Values.ContainsKey("Filter") == false) { return; }
            IPdfElement elemFilter = stream.Dictionary.Values["Filter"];

            stream.OriginalData = stream.Data;
            stream.OriginalFilter = stream.Dictionary.Values["Filter"];

            if (elemFilter is PdfString)
            {
                ApplyFilterToStream(stream, ((PdfString)elemFilter).Value);
            }
            else if (elemFilter is PdfName)
            {
                ApplyFilterToStream(stream, ((PdfName)elemFilter).Value);
            }
            else if(elemFilter is PdfArray)
            {
                foreach(IPdfElement elemSubFilter in ((PdfArray)elemFilter).Values)
                {
                    if (elemSubFilter is PdfString)
                    {
                        ApplyFilterToStream(stream, ((PdfString)elemSubFilter).Value);
                    }
                    else if (elemSubFilter is PdfName)
                    {
                        ApplyFilterToStream(stream, ((PdfName)elemSubFilter).Value);
                    }
                    else
                    {
                        throw new Exception("PdfFilter not correctly specified");
                    }
                }
            }
            else
            {
                throw new Exception("PdfFilter not correctly specified");
            }

            stream.Dictionary.Values["Length"] = new PdfInteger { Value = stream.Data.Length };
            stream.Dictionary.Values.Remove("Filter");
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

        private static void ExtractPages(PdfDictionary page, PdfDocument doc, PdfDictionary resources)
        {
            string type = page.GetParamAsString("Type");
            if (type == "Page")
            {
                PdfDocumentPage docPage = new PdfDocumentPage(page, resources);
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
                    PdfDictionary resourcesAux = null;
                    if (page.Values.ContainsKey("Resources"))
                    {
                        resourcesAux = page.Values["Resources"] as PdfDictionary;
                    }
                    ExtractPages(childPage, doc, resourcesAux);
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
                PdfObject obj = parser.ParseObject(doc.Objects);
                if (obj != null && obj.Data != null)
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
            PdfDictionary resources = null;
            if (doc.Catalog.Values.ContainsKey("Resources"))
            {
                resources = doc.Catalog.Values["Resources"] as PdfDictionary;
            }
            ExtractPages(pages, doc, resources);

            return doc;
        }

        #endregion

    }
}
