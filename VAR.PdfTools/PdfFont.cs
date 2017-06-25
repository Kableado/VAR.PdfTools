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

        private double _height = 1.0;

        private string _name = string.Empty;

        private bool _tainted = false;

        #endregion

        #region Properties

        public PdfDictionary BaseData { get { return _baseData; } }
        
        public double Height { get { return _height; } }

        public string Name { get { return _name; } set { _name = value; } }

        public bool Tainted { get { return _tainted; } }

        #endregion

        #region Life cycle

        public PdfFont(PdfDictionary baseData)
        {
            _baseData = baseData;
            string type = baseData.GetParamAsString("Type");
            if (type != "Font")
            {
                // NOTE: Type="Font" is Required by the standard, continuing anyway
                _tainted = true;
            }

            PrepareSizes(baseData);
        }

        #endregion

        #region Private methods

        private void PrepareSizes(PdfDictionary baseData)
        {
            // Set "Times-Roman" as default basefont sizes
            _widths = PdfStandar14FontMetrics.Times_Roman.Widths;
            _height = PdfStandar14FontMetrics.Times_Roman.ApproxHeight;

            if (baseData.Values.ContainsKey("ToUnicode"))
            {
                byte[] toUnicodeStream = ((PdfStream)baseData.Values["ToUnicode"]).Data;
                PdfParser parser = new PdfParser(toUnicodeStream);
                _toUnicode = parser.ParseToUnicode();
            }

            string baseFont = _baseData.GetParamAsString("BaseFont");
            if (string.IsNullOrEmpty(baseFont))
            {
                SetBaseFontSizes(baseFont);
            }

            if (_baseData.Values.ContainsKey("FirstChar") && _baseData.Values.ContainsKey("LastChar") && _baseData.Values.ContainsKey("Widths"))
            {
                ParseSizes();
            }
        }

        private void ParseSizes()
        {
            double glyphSpaceToTextSpace = 1000.0; // FIXME: SubType:Type3 Uses a FontMatrix that may not correspond to 1/1000th
            _widths = new Dictionary<char, double>();
            char firstChar = (char)_baseData.GetParamAsInt("FirstChar");
            char lastChar = (char)_baseData.GetParamAsInt("LastChar");
            PdfArray widths = _baseData.Values["Widths"] as PdfArray;
            char actualChar = firstChar;
            foreach (IPdfElement elem in widths.Values)
            {
                if (elem is PdfReal widthReal)
                {
                    double charWidth = widthReal.Value / glyphSpaceToTextSpace;
                    _widths.Add(actualChar, charWidth);
                    actualChar++;
                    continue;
                }
                if (elem is PdfInteger widthInt)
                {
                    double charWidth = widthInt.Value / glyphSpaceToTextSpace;
                    _widths.Add(actualChar, charWidth);
                    actualChar++;
                    continue;
                }
            }
            // FIMXE: Calculate real height
        }

        private void SetBaseFontSizes(string baseFont)
        {
            if (baseFont == "Times-Roman")
            {
                _widths = PdfStandar14FontMetrics.Times_Roman.Widths;
                _height = PdfStandar14FontMetrics.Times_Roman.ApproxHeight;
            }
            if (baseFont == "Times-Bold")
            {
                _widths = PdfStandar14FontMetrics.Times_Bold.Widths;
                _height = PdfStandar14FontMetrics.Times_Bold.ApproxHeight;
            }
            if (baseFont == "Times-Italic")
            {
                _widths = PdfStandar14FontMetrics.Times_Italic.Widths;
                _height = PdfStandar14FontMetrics.Times_Italic.ApproxHeight;
            }
            if (baseFont == "Times-BoldItalic")
            {
                _widths = PdfStandar14FontMetrics.Times_BoldItalic.Widths;
                _height = PdfStandar14FontMetrics.Times_BoldItalic.ApproxHeight;
            }
            if (baseFont == "Helvetica")
            {
                _widths = PdfStandar14FontMetrics.Helvetica.Widths;
                _height = PdfStandar14FontMetrics.Helvetica.ApproxHeight;
            }
            if (baseFont == "Helvetica-Bold")
            {
                _widths = PdfStandar14FontMetrics.Helvetica_Bold.Widths;
                _height = PdfStandar14FontMetrics.Helvetica_Bold.ApproxHeight;
            }
            if (baseFont == "Helvetica-Oblique")
            {
                _widths = PdfStandar14FontMetrics.Helvetica_Oblique.Widths;
                _height = PdfStandar14FontMetrics.Helvetica_Oblique.ApproxHeight;
            }
            if (baseFont == "Helvetica-BoldOblique")
            {
                _widths = PdfStandar14FontMetrics.Helvetica_BoldOblique.Widths;
                _height = PdfStandar14FontMetrics.Helvetica_BoldOblique.ApproxHeight;
            }
            if (baseFont == "Courier")
            {
                _widths = PdfStandar14FontMetrics.Courier.Widths;
                _height = PdfStandar14FontMetrics.Courier.ApproxHeight;
            }
            if (baseFont == "Courier-Bold")
            {
                _widths = PdfStandar14FontMetrics.Courier_Bold.Widths;
                _height = PdfStandar14FontMetrics.Courier_Bold.ApproxHeight;
            }
            if (baseFont == "Courier-Oblique")
            {
                _widths = PdfStandar14FontMetrics.Courier_Oblique.Widths;
                _height = PdfStandar14FontMetrics.Courier_Oblique.ApproxHeight;
            }
            if (baseFont == "Courier-BoldOblique")
            {
                _widths = PdfStandar14FontMetrics.Courier_BoldOblique.Widths;
                _height = PdfStandar14FontMetrics.Courier_BoldOblique.ApproxHeight;
            }
            if (baseFont == "Symbol")
            {
                _widths = PdfStandar14FontMetrics.Symbol.Widths;
                _height = PdfStandar14FontMetrics.Symbol.ApproxHeight;
            }
            if (baseFont == "ZapfDingbats")
            {
                _widths = PdfStandar14FontMetrics.ZapfDingbats.Widths;
                _height = PdfStandar14FontMetrics.ZapfDingbats.ApproxHeight;
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
