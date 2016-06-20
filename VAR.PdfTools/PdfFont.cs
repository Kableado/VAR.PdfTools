using System;
using System.Collections.Generic;

namespace VAR.PdfTools
{
    public class PdfFont
    {
        #region Declarations

        private PdfDictionary _baseData = null;

        private Dictionary<char, string> _toUnicode = null;

        private Dictionary<char, double> _widths = null;

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

            if (BaseData.Values.ContainsKey("FirstChar") && baseData.Values.ContainsKey("LastChar") && baseData.Values.ContainsKey("Widths"))
            {
                double glyphSpaceToTextSpace = 1000.0; // FIXME: SubType:Type3 Uses a FontMatrix that may not correspond to 1/1000th
                _widths = new Dictionary<char, double>();
                char firstChar = (char)baseData.GetParamAsInt("FirstChar");
                char lastChar = (char)baseData.GetParamAsInt("LastChar");
                PdfArray widths = baseData.Values["Widths"] as PdfArray;
                char actualChar = firstChar;
                foreach (IPdfElement elem in widths.Values)
                {
                    PdfReal widthReal = elem as PdfReal;
                    if (widthReal != null)
                    {
                        _widths.Add(actualChar, widthReal.Value / glyphSpaceToTextSpace);
                        actualChar++;
                        continue;
                    }
                    PdfInteger widthInt = elem as PdfInteger;
                    if (widthInt != null)
                    {
                        _widths.Add(actualChar, widthInt.Value / glyphSpaceToTextSpace);
                        actualChar++;
                        continue;
                    }
                }
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

        public double GetCharWidth(char character)
        {
            if (_widths == null)
            {
                return 0;
            }
            if (_widths.ContainsKey(character))
            {
                return _widths[character];
            }
            return 0;
        }

        #endregion
    }
}
