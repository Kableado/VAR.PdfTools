using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAR.PdfTools
{
    public class PdfFont
    {
        #region Declarations

        private PdfDictionary _baseData = null;

        private Dictionary<char, string> _toUnicode = null;

        #endregion

        #region Properties

        public PdfDictionary BaseData { get { return _baseData; } }
        
        #endregion

        #region Life cycle

        public PdfFont(PdfDictionary baseData)
        {
            _baseData = baseData;
            string type = baseData.GetParamAsString("Type");
            if (type != "Font")
            {
                throw new Exception(string.Format("PdfFont: Expected dictionary of type:\"Font\". Found: {0}", type));
            }

            if (baseData.Values.ContainsKey("ToUnicode"))
            {
                byte[] toUnicodeStream = ((PdfStream)baseData.Values["ToUnicode"]).Data;
                PdfParser parser = new PdfParser(toUnicodeStream);
                _toUnicode = parser.ParseToUnicode();
            }

        }

        #endregion

        #region Public methods

        public string ToUnicode(char character)
        {
            if (_toUnicode == null)
            {
                // FIXME: use standar tables
                return new string(character, 1);
            }

            if (_toUnicode.ContainsKey(character))
            {
                return _toUnicode[character];
            }

            return new string(character, 1);
        }

        #endregion
    }
}
