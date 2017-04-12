using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAR.PdfTools
{
    public class Vector3D
    {
        #region Declarations

        public double[] _vector = new double[3];

        #endregion

        #region Properties

        public double[] Vector { get { return _vector; } }

        #endregion

        #region Creator

        public Vector3D()
        {
            Init();
        }
        
        public void Init()
        {
            _vector[0] = 0.0;
            _vector[1] = 0.0;
            _vector[2] = 1.0;
        }

        #endregion
    }
    
    public class Matrix3x3
    {
        #region Declarations

        public double[,] _matrix = new double[3, 3];

        #endregion

        #region Properties

        public double[,] Matrix { get { return _matrix; } }

        #endregion

        #region Creator

        public Matrix3x3()
        {
            Idenity();
        }

        public Matrix3x3(double a, double b, double c, double d, double e, double f)
        {
            Set(a, b, c, d, e, f);
        }

        #endregion

        #region Public methods

        public void Idenity()
        {
            _matrix[0, 0] = 1.0;
            _matrix[0, 1] = 0.0;
            _matrix[0, 2] = 0.0;
            _matrix[1, 0] = 0.0;
            _matrix[1, 1] = 1.0;
            _matrix[1, 2] = 0.0;
            _matrix[2, 0] = 0.0;
            _matrix[2, 1] = 0.0;
            _matrix[2, 2] = 1.0;
        }

        public void Set(double a, double b, double c, double d, double e, double f)
        {
            _matrix[0, 0] = a;
            _matrix[1, 0] = b;
            _matrix[2, 0] = 0;
            _matrix[0, 1] = c;
            _matrix[1, 1] = d;
            _matrix[2, 1] = 0;
            _matrix[0, 2] = e;
            _matrix[1, 2] = f;
            _matrix[2, 2] = 1;
        }

        public Vector3D Multiply(Vector3D vect)
        {
            Vector3D vectResult = new Vector3D();

            vectResult.Vector[0] = (vect.Vector[0] * _matrix[0, 0]) + (vect.Vector[1] * _matrix[0, 1]) + (vect.Vector[2] * _matrix[0, 2]);
            vectResult.Vector[1] = (vect.Vector[0] * _matrix[1, 0]) + (vect.Vector[1] * _matrix[1, 1]) + (vect.Vector[2] * _matrix[1, 2]);
            vectResult.Vector[2] = (vect.Vector[0] * _matrix[2, 0]) + (vect.Vector[1] * _matrix[2, 1]) + (vect.Vector[2] * _matrix[2, 2]);

            return vectResult;
        }

        public Matrix3x3 Multiply(Matrix3x3 matrix)
        {
            Matrix3x3 newMatrix = new Matrix3x3();

            newMatrix._matrix[0, 0] = (_matrix[0, 0] * matrix._matrix[0, 0]) + (_matrix[1, 0] * matrix._matrix[0, 1]) + (_matrix[2, 0] * matrix._matrix[0, 2]);
            newMatrix._matrix[0, 1] = (_matrix[0, 1] * matrix._matrix[0, 0]) + (_matrix[1, 1] * matrix._matrix[0, 1]) + (_matrix[2, 1] * matrix._matrix[0, 2]);
            newMatrix._matrix[0, 2] = (_matrix[0, 2] * matrix._matrix[0, 0]) + (_matrix[1, 2] * matrix._matrix[0, 1]) + (_matrix[2, 2] * matrix._matrix[0, 2]);
            newMatrix._matrix[1, 0] = (_matrix[0, 0] * matrix._matrix[1, 0]) + (_matrix[1, 0] * matrix._matrix[1, 1]) + (_matrix[2, 0] * matrix._matrix[1, 2]);
            newMatrix._matrix[1, 1] = (_matrix[0, 1] * matrix._matrix[1, 0]) + (_matrix[1, 1] * matrix._matrix[1, 1]) + (_matrix[2, 1] * matrix._matrix[1, 2]);
            newMatrix._matrix[1, 2] = (_matrix[0, 2] * matrix._matrix[1, 0]) + (_matrix[1, 2] * matrix._matrix[1, 1]) + (_matrix[2, 2] * matrix._matrix[1, 2]);
            newMatrix._matrix[2, 0] = (_matrix[0, 0] * matrix._matrix[2, 0]) + (_matrix[1, 0] * matrix._matrix[2, 1]) + (_matrix[2, 0] * matrix._matrix[2, 2]);
            newMatrix._matrix[2, 1] = (_matrix[0, 1] * matrix._matrix[2, 0]) + (_matrix[1, 1] * matrix._matrix[2, 1]) + (_matrix[2, 1] * matrix._matrix[2, 2]);
            newMatrix._matrix[2, 2] = (_matrix[0, 2] * matrix._matrix[2, 0]) + (_matrix[1, 2] * matrix._matrix[2, 1]) + (_matrix[2, 2] * matrix._matrix[2, 2]);
            
            return newMatrix;
        }

        public Matrix3x3 Copy()
        {
            Matrix3x3 newMatrix = new Matrix3x3();

            newMatrix._matrix[0, 0] = _matrix[0, 0];
            newMatrix._matrix[0, 1] = _matrix[0, 1];
            newMatrix._matrix[0, 2] = _matrix[0, 2];
            newMatrix._matrix[1, 0] = _matrix[1, 0];
            newMatrix._matrix[1, 1] = _matrix[1, 1];
            newMatrix._matrix[1, 2] = _matrix[1, 2];
            newMatrix._matrix[2, 0] = _matrix[2, 0];
            newMatrix._matrix[2, 1] = _matrix[2, 1];
            newMatrix._matrix[2, 2] = _matrix[2, 2];

            return newMatrix;
        }

        public bool IsCollinear(Matrix3x3 otherMatrix, double horizontalDelta = 0.00001, double verticalDelta = 0.00001)
        {
            double epsilon = 0.00001;
            return (
                Math.Abs(_matrix[0, 0] - otherMatrix.Matrix[0, 0]) <= epsilon &&
                Math.Abs(_matrix[1, 0] - otherMatrix.Matrix[1, 0]) <= epsilon &&
                Math.Abs(_matrix[0, 1] - otherMatrix.Matrix[0, 1]) <= epsilon &&
                Math.Abs(_matrix[1, 1] - otherMatrix.Matrix[1, 1]) <= epsilon &&
                Math.Abs(_matrix[0, 2] - otherMatrix.Matrix[0, 2]) <= horizontalDelta &&
                Math.Abs(_matrix[1, 2] - otherMatrix.Matrix[1, 2]) <= verticalDelta &&
                true);
        }

        #endregion
    }

    public class PdfTextElement
    {
        #region Properties

        public PdfFont Font { get; set; }

        public double FontSize { get; set; }

        public Matrix3x3 Matrix { get; set; }

        public string RawText { get; set; }

        public string VisibleText { get; set; }

        public double VisibleWidth { get; set; }

        public double VisibleHeight { get; set; }

        private List<PdfTextElement> _childs = new List<PdfTextElement>();
        public List<PdfTextElement> Childs { get { return _childs; } }

        #endregion

        #region Public methods

        public double GetX()
        {
            return Matrix.Matrix[0, 2];
        }

        public double GetY()
        {
            return Matrix.Matrix[1, 2];
        }

        #endregion
    }

    public class PdfTextExtractor
    {
        #region Declarations

        private PdfDocumentPage _page = null;

        private List<PdfTextElement> _textElements = new List<PdfTextElement>();

        // Graphics state
        private List<Matrix3x3> _graphicsMatrixStack = new List<Matrix3x3>();
        private Matrix3x3 _graphicsMatrix = new Matrix3x3();

        // Text state
        private PdfFont _font = null;
        private double _fontSize = 1;
        private double _textLeading = 0;

        // Text object state
        private bool inText = false;
        private Matrix3x3 _textMatrix = new Matrix3x3();
        private StringBuilder _sbText = new StringBuilder();
        private double _textWidth = 0;

        PdfTextElement _currentTextElement = null;

        #endregion

        #region Properties

        public PdfDocumentPage Page { get { return _page; } }

        public List<PdfTextElement> Elements { get { return _textElements; } }

        #endregion

        #region lifecycle 

        public PdfTextExtractor(PdfDocumentPage page)
        {
            _page = page;
            ProcessPage();
        }

        #endregion

        #region Utility methods

        private string PdfString_ToUnicode(string text, PdfFont font)
        {
            if (font == null)
            {
                return text;
            }

            StringBuilder sbText = new StringBuilder();
            foreach (char c in text)
            {
                sbText.Append(font.ToUnicode(c).Replace("\0", ""));
            }
            return sbText.ToString();
        }

        private PdfTextElement BuildTextElement()
        {
            PdfTextElement textElem = new PdfTextElement();
            textElem.Font = _font;
            textElem.FontSize = _fontSize;
            textElem.Matrix = _textMatrix.Multiply(_graphicsMatrix);
            textElem.RawText = _sbText.ToString();
            textElem.VisibleText = PdfString_ToUnicode(textElem.RawText, _font);
            textElem.VisibleWidth = _textWidth * textElem.Matrix.Matrix[0, 0];
            textElem.VisibleHeight = (_font.Height * _fontSize) * textElem.Matrix.Matrix[1, 1];
            return textElem;
        }

        private void FlushTextElementSoft()
        {
            if (_sbText.Length == 0)
            {
                return;
            }

            PdfTextElement textElem = BuildTextElement();
            if (_currentTextElement == null)
            {
                _currentTextElement = new PdfTextElement();
                _currentTextElement.Font = null;
                _currentTextElement.FontSize = -1;
                _currentTextElement.Matrix = textElem.Matrix.Copy();
                _currentTextElement.RawText = string.Empty;
                _currentTextElement.VisibleText = string.Empty;
                _currentTextElement.VisibleWidth = 0;
                _currentTextElement.VisibleHeight = 0;
            }
            _currentTextElement.VisibleText += textElem.VisibleText;
            _currentTextElement.VisibleWidth += textElem.VisibleWidth;
            _currentTextElement.VisibleHeight = System.Math.Max(_currentTextElement.VisibleHeight, textElem.VisibleHeight);
            _currentTextElement.Childs.Add(textElem);

            _sbText = new StringBuilder();
            _textWidth = 0;
        }

        private void AddTextElement(PdfTextElement textElement)
        {
            if (string.IsNullOrEmpty(textElement.VisibleText.Trim()))
            {
                return;
            }
            _textElements.Add(textElement);
        }

        private void FlushTextElement()
        {
            if (_sbText.Length == 0)
            {
                if (_currentTextElement != null)
                {
                    AddTextElement(_currentTextElement);
                    _currentTextElement = null;
                }
                return;
            }

            if (_currentTextElement != null)
            {
                FlushTextElementSoft();
                AddTextElement(_currentTextElement);
                _currentTextElement = null;
            }
            else
            {
                PdfTextElement textElem = BuildTextElement();
                AddTextElement(textElem);
            }

            _sbText = new StringBuilder();
            _textWidth = 0;
        }

        private string SimplifyText(string text)
        {
            StringBuilder sbResult = new StringBuilder();
            foreach (char c in text)
            {
                if (c == '.' || c == ',' || 
                    c == ':' || c == ';' || 
                    c == '-' || c == '_' || 
                    c == ' ' || c == '\t')
                {
                    continue;
                }
                sbResult.Append(char.ToUpper(c));
            }
            return sbResult.ToString();
        }

        private PdfTextElement FindElementByText(string text, bool fuzzy)
        {
            string matchingText = fuzzy ? SimplifyText(text) : text;
            foreach (PdfTextElement elem in _textElements)
            {
                string elemText = fuzzy ? SimplifyText(elem.VisibleText) : elem.VisibleText;
                if (elemText == matchingText)
                {
                    return elem;
                }
            }
            return null;
        }

        private bool TextElementVerticalIntersection(PdfTextElement elem1, PdfTextElement elem2)
        {
            double elem1X1 = elem1.GetX();
            double elem1X2 = elem1.GetX() + elem1.VisibleWidth;
            double elem2X1 = elem2.GetX();
            double elem2X2 = elem2.GetX() + elem2.VisibleWidth;

            return elem1X2 >= elem2X1 && elem2X2 >= elem1X1;
        }

        private bool TextElementHorizontalIntersection(PdfTextElement elem1, PdfTextElement elem2)
        {
            double elem1Y1 = elem1.GetY();
            double elem1Y2 = elem1.GetY() + elem1.VisibleHeight;
            double elem2Y1 = elem2.GetY();
            double elem2Y2 = elem2.GetY() + elem2.VisibleHeight;

            return elem1Y2 >= elem2Y1 && elem2Y2 >= elem1Y1;
        }

        #endregion

        #region Operations

        private void OpPushGraphState()
        {
            _graphicsMatrixStack.Add(_graphicsMatrix.Copy());
        }

        private void OpPopGraphState()
        {
            _graphicsMatrix = _graphicsMatrixStack[_graphicsMatrixStack.Count - 1];
            _graphicsMatrixStack.RemoveAt(_graphicsMatrixStack.Count - 1);
        }

        private void OpSetGraphMatrix(double a, double b, double c, double d, double e, double f)
        {
            _graphicsMatrix.Set(a, b, c, d, e, f);
        }

        private void OpBeginText()
        {
            _textMatrix.Idenity();
            inText = true;
        }

        private void OpEndText()
        {
            FlushTextElementSoft();
            inText = false;
        }

        private void OpTextFont(string fontName, double size)
        {
            FlushTextElementSoft();
            _font = _page.Fonts[fontName];
            _fontSize = size;
        }

        private void OpTextLeading(double textLeading)
        {
            _textLeading = textLeading;
        }

        private void OpTextDisplace(double x, double y)
        {
            FlushTextElement();
            var newMatrix = new Matrix3x3();
            newMatrix.Matrix[0, 2] = x;
            newMatrix.Matrix[1, 2] = y;
            _textMatrix = newMatrix.Multiply(_textMatrix);
        }

        private void OpTextLineFeed()
        {
            OpTextDisplace(0, -_textLeading);
        }

        private void OpSetTextMatrix(double a, double b, double c, double d, double e, double f)
        {
            double halfSpaceWidth = 0;
            double horizontalDelta = 0;
            Matrix3x3 newMatrix = new Matrix3x3(a, b, c, d, e, f);

            if (_font != null)
            {
                halfSpaceWidth = _font.GetCharWidth(' ') * _fontSize;
            }
            horizontalDelta = (_textWidth + halfSpaceWidth);
            if (_textMatrix.IsCollinear(newMatrix, horizontalDelta: horizontalDelta))
            {
                return;
            }
            if (_currentTextElement != null)
            {
                if (_currentTextElement.Font != null)
                {
                    halfSpaceWidth = _currentTextElement.Font.GetCharWidth(' ') * _currentTextElement.FontSize;
                }
                horizontalDelta = (_currentTextElement.VisibleWidth + halfSpaceWidth);
                if (_currentTextElement.Matrix.IsCollinear(newMatrix, horizontalDelta: horizontalDelta))
                {
                    FlushTextElementSoft();
                    _textMatrix = newMatrix;
                    return;
                }
            }
            FlushTextElement();
            _textMatrix = newMatrix;
        }
        
        private void OpTextPut(string text)
        {
            if (inText == false) { return; }
            _sbText.Append(text);
            if (_font != null)
            {
                foreach (char c in text)
                {
                    _textWidth += _font.GetCharWidth(c) * _fontSize;
                }
            }
        }

        private void OpTextPutMultiple(PdfArray array)
        {
            if (inText == false) { return; }
            foreach (IPdfElement elem in array.Values)
            {
                if(elem is PdfString)
                {
                    OpTextPut(((PdfString)elem).Value);
                }
                else if(elem is PdfInteger || elem is PdfReal)
                {
                    double spacing = PdfElementUtils.GetReal(elem, 0);
                    // FIXME: Apply correctly spacing
                    //_textWidth += spacing; 
                }
                else if(elem is PdfArray)
                {
                    OpTextPutMultiple(((PdfArray)elem));
                }
            }
        }

        #endregion

        #region Private methods

        private void ProcessPage()
        {
            int unknowCount = 0;
            for (int i = 0; i < _page.ContentActions.Count; i++)
            {
                PdfContentAction action = _page.ContentActions[i];
                // Graphics Operations
                if (action.Token == "q")
                {
                    OpPushGraphState();
                }
                else if (action.Token == "Q")
                {
                    OpPopGraphState();
                }
                else if (action.Token == "cm")
                {
                    double a = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    double b = PdfElementUtils.GetReal(action.Parameters[1], 0);
                    double c = PdfElementUtils.GetReal(action.Parameters[2], 0);
                    double d = PdfElementUtils.GetReal(action.Parameters[3], 0);
                    double e = PdfElementUtils.GetReal(action.Parameters[4], 0);
                    double f = PdfElementUtils.GetReal(action.Parameters[5], 0);
                    OpSetGraphMatrix(a, b, c, d, e, f);
                }

                // Text Operations
                else if (action.Token == "BT")
                {
                    OpBeginText();
                }
                else if (action.Token == "ET")
                {
                    OpEndText();
                }
                else if (action.Token == "Tc")
                {
                    // FIXME: Char spacing
                }
                else if (action.Token == "Tw")
                {
                    // FIXME: Word spacing
                }
                else if (action.Token == "Tz")
                {
                    // FIXME: Horizontal Scale
                }
                else if (action.Token == "Tf")
                {
                    string fontName = ((PdfName)action.Parameters[0]).Value;
                    double fontSize = PdfElementUtils.GetReal(action.Parameters[1], 0);
                    OpTextFont(fontName, fontSize);
                }
                else if (action.Token == "TL")
                {
                    double leading = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    OpTextLeading(leading);
                }
                else if (action.Token == "Tr")
                {
                    // FIXME: Rendering mode
                }
                else if (action.Token == "Ts")
                {
                    // FIXME: Text rise
                }
                else if (action.Token == "Td")
                {
                    double x = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    double y = PdfElementUtils.GetReal(action.Parameters[1], 0);
                    OpTextDisplace(x, y);
                }
                else if (action.Token == "TD")
                {
                    double x = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    double y = PdfElementUtils.GetReal(action.Parameters[1], 0);
                    OpTextLeading(-y);
                    OpTextDisplace(x, y);
                }
                else if (action.Token == "Tm")
                {
                    double a = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    double b = PdfElementUtils.GetReal(action.Parameters[1], 0);
                    double c = PdfElementUtils.GetReal(action.Parameters[2], 0);
                    double d = PdfElementUtils.GetReal(action.Parameters[3], 0);
                    double e = PdfElementUtils.GetReal(action.Parameters[4], 0);
                    double f = PdfElementUtils.GetReal(action.Parameters[5], 0);
                    OpSetTextMatrix(a, b, c, d, e, f);
                }
                else if (action.Token == "T*")
                {
                    OpTextLineFeed();
                }
                else if (action.Token == "Tj")
                {
                    OpTextPut(((PdfString)action.Parameters[0]).Value);
                }
                else if (action.Token == "'")
                {
                    OpTextLineFeed();
                    OpTextPut(((PdfString)action.Parameters[0]).Value);
                }
                else if (action.Token == "\"")
                {
                    double wordSpacing = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    double charSpacing = PdfElementUtils.GetReal(action.Parameters[1], 0);
                    OpTextPut(((PdfString)action.Parameters[2]).Value);
                }
                else if (action.Token == "TJ")
                {
                    OpTextPutMultiple(((PdfArray)action.Parameters[0]));
                }
                else if (action.Token == "re")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "f")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "g")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "rg")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "BI")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "ID")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "EI")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "W")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "n")
                {
                    // FIXME: Interpret this
                }
                else if (action.Token == "Do")
                {
                    // FIXME: Interpret this
                }
                else
                {
                    unknowCount++;
                }
            }
            FlushTextElement();
        }

        #endregion

        #region Public methods

        public List<string> GetColumn(string column)
        {
            return GetColumn(column, true);
        }

        public List<string> GetColumn(string column, bool fuzzy)
        {
            PdfTextElement columnHead = FindElementByText(column, fuzzy);
            if(columnHead == null)
            {
                return new List<string>();
            }
            double headY = columnHead.GetY();
            double headX1 = columnHead.GetX();
            double headX2 = headX1 + columnHead.VisibleWidth;

            // Determine horizontal extent
            double extentX1 = double.MinValue;
            double extentX2 = double.MaxValue;
            foreach (PdfTextElement elem in _textElements)
            {
                if(elem == columnHead){continue;}
                if (TextElementHorizontalIntersection(columnHead, elem) == false) { continue; }
                double elemX1 = elem.GetX();
                double elemX2 = elemX1 + elem.VisibleWidth;

                if (elemX2 < headX1)
                {
                    if (elemX2 > extentX1)
                    {
                        extentX1 = elemX2;
                    }
                }
                if (elemX1 > headX2)
                {
                    if (elemX1 < extentX2)
                    {
                        extentX2 = elemX1;
                    }
                }

            }

            // Get all the elements that intersects vertically, are down and sort results
            var columnDataRaw = new List<PdfTextElement>();
            foreach (PdfTextElement elem in _textElements)
            {
                if (TextElementVerticalIntersection(columnHead, elem) == false) { continue; }

                // Only intems down the column
                double elemY = elem.GetY();
                if (elemY >= headY) { continue; }

                columnDataRaw.Add(elem);
            }
            columnDataRaw = columnDataRaw.OrderByDescending(elem => elem.GetY()).ToList();

            // Only items completelly inside extents, amd break on the first element outside
            var columnData = new List<PdfTextElement>();
            foreach (PdfTextElement elem in columnDataRaw)
            {
                double elemX1 = elem.GetX();
                double elemX2 = elemX1 + elem.VisibleWidth;
                if (elemX1 < extentX1 || elemX2 > extentX2) { break; }

                columnData.Add(elem);
            }

            // Emit result
            var result = new List<string>();
            foreach (PdfTextElement elem in columnData)
            {
                result.Add(elem.VisibleText);
            }
            return result;
        }

        public string GetField(string field)
        {
            return GetField(field, true);
        }

        public string GetField(string field, bool fuzzy)
        {
            PdfTextElement fieldTitle = FindElementByText(field, fuzzy);
            if (fieldTitle == null)
            {
                return null;
            }
            double titleX = fieldTitle.GetX();
            var fieldData = new List<PdfTextElement>();


            foreach (PdfTextElement elem in _textElements)
            {
                if (TextElementHorizontalIntersection(fieldTitle, elem) == false) { continue; }
                double elemX = elem.GetX();
                if (elemX <= titleX) { continue; }

                fieldData.Add(elem);
            }

            if(fieldData.Count == 0)
            {
                return null;
            }

            return fieldData.OrderBy(elem => elem.GetX()).FirstOrDefault().VisibleText;
        }

        public bool HasText(string text)
        {
            return HasText(text, true);
        }

        public bool HasText(string text, bool fuzzy)
        {
            PdfTextElement fieldTitle = FindElementByText(text, fuzzy);
            if (fieldTitle == null)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
