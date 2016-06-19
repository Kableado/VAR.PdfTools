using System;
using System.Collections.Generic;

namespace VAR.PdfTools
{
    public class PdfDocumentPage
    {
        #region Declarations

        private PdfDictionary _baseData = null;

        private byte[] _content = null;

        private PdfDictionary _resources = null;

        private Dictionary<string, PdfDictionary> _fonts = new Dictionary<string, PdfDictionary>();

        #endregion

        #region Properties
        
        public PdfDictionary BaseData { get { return _baseData; } }

        public byte[] Content { get { return _content; } }
        
        public Dictionary<string, PdfDictionary> Fonts { get { return _fonts; } }

        #endregion

        #region Life cycle

        public PdfDocumentPage(PdfDictionary baseData, PdfDocumentPage prevDocPage)
        {
            _baseData = baseData;
            string type = baseData.GetParamAsString("Type");
            if (type != "Page")
            {
                throw new Exception(string.Format("PdfDocumentPage: Expected dictionary of type:\"Page\". Found: {0}", type));
            }

            _content = _baseData.GetParamAsStream("Contents");

            if (_baseData.Values.ContainsKey("Resources") == false)
            {
                _resources = prevDocPage._resources;
            }
            else
            {
                _resources = _baseData.Values["Resources"] as PdfDictionary;
            }
            if (_resources.Values.ContainsKey("Font"))
            {
                PdfDictionary fonts = _resources.Values["Font"] as PdfDictionary;
                foreach (KeyValuePair<string, IPdfElement> pair in fonts.Values)
                {
                    _fonts.Add(pair.Key, pair.Value as PdfDictionary);
                }
            }
        }

        #endregion
    }
}
