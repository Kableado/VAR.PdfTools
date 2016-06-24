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

        private Dictionary<string, PdfFont> _fonts = new Dictionary<string, PdfFont>();

        private List<PdfContentAction> _contentActions = null;

        #endregion

        #region Properties
        
        public PdfDictionary BaseData { get { return _baseData; } }

        public byte[] Content { get { return _content; } }
        
        public Dictionary<string, PdfFont> Fonts { get { return _fonts; } }

        public List<PdfContentAction> ContentActions { get { return _contentActions; } }

        #endregion

        #region Life cycle

        public PdfDocumentPage(PdfDictionary baseData, PdfDictionary resources)
        {
            _baseData = baseData;
            string type = baseData.GetParamAsString("Type");
            if (type != "Page")
            {
                throw new Exception(string.Format("PdfDocumentPage: Expected dictionary of type:\"Page\". Found: {0}", type));
            }

            // Get content, resources and fonts
            _content = _baseData.GetParamAsStream("Contents");
            if (_baseData.Values.ContainsKey("Resources") == false)
            {
                _resources = resources;
            }
            else
            {
                _resources = _baseData.Values["Resources"] as PdfDictionary;
            }
            if (_resources != null && _resources.Values.ContainsKey("Font"))
            {
                PdfDictionary fonts = _resources.Values["Font"] as PdfDictionary;
                foreach (KeyValuePair<string, IPdfElement> pair in fonts.Values)
                {
                    var font = new PdfFont(pair.Value as PdfDictionary);
                    font.Name = pair.Key;
                    _fonts.Add(pair.Key, font);
                }
            }

            // Parse content
            if (_content != null)
            {
                PdfParser parser = new PdfParser(_content);
                _contentActions = parser.ParseContent();
            }else
            {
                _contentActions = new List<PdfContentAction>();
            }
        }

        #endregion
    }
}
