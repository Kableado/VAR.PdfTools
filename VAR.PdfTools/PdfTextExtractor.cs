using System.Collections.Generic;
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

        #endregion
    }

    public class PdfTextElement
    {
        public PdfFont Font { get; set; }

        public double TextSize { get; set; }

        public Matrix3x3 Matrix { get; set; }

        public string RawText { get; set; }

        public string VisibleText { get; set; }

        public double VisibleWidth { get; set; }
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

        private string PdfElement_GetOnlyStrings(IPdfElement elem)
        {
            if (elem is PdfString)
            {
                return ((PdfString)elem).Value;
            }
            if (elem is PdfArray)
            {
                var sbText = new StringBuilder();
                PdfArray array = elem as PdfArray;
                foreach (IPdfElement subElem in array.Values)
                {
                    sbText.Append(PdfElement_GetOnlyStrings(subElem));
                }
                return sbText.ToString();
            }
            return string.Empty;
        }

        private double PdfElement_GetReal(IPdfElement elem, double defaultValue)
        {
            if (elem is PdfInteger)
            {
                return ((PdfInteger)elem).Value;
            }
            if (elem is PdfReal)
            {
                return ((PdfReal)elem).Value;
            }
            return defaultValue;
        }

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

        private void FlushTextElement()
        {
            if (_sbText.Length == 0)
            {
                return;
            }

            PdfTextElement textElem = new PdfTextElement();
            textElem.Matrix = _textMatrix.Multiply(_graphicsMatrix);
            textElem.Font = _font;
            textElem.RawText = _sbText.ToString();
            textElem.VisibleText = PdfString_ToUnicode(textElem.RawText, _font);
            textElem.VisibleWidth = _textWidth * textElem.Matrix.Matrix[0, 0];
            _textElements.Add(textElem);

            _sbText = new StringBuilder();
            _textWidth = 0;
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
            _graphicsMatrix.Matrix[0, 0] = a;
            _graphicsMatrix.Matrix[1, 0] = b;
            _graphicsMatrix.Matrix[2, 0] = 0;
            _graphicsMatrix.Matrix[0, 1] = c;
            _graphicsMatrix.Matrix[1, 1] = d;
            _graphicsMatrix.Matrix[2, 1] = 0;
            _graphicsMatrix.Matrix[0, 2] = e;
            _graphicsMatrix.Matrix[1, 2] = f;
            _graphicsMatrix.Matrix[2, 2] = 1;
        }

        private void OpBeginText()
        {
            _textMatrix.Idenity();
            inText = true;
        }

        private void OpEndText()
        {
            FlushTextElement();
            inText = false;
        }

        private void OpTextFont(string fontName, double size)
        {
            FlushTextElement();
            _font = _page.Fonts[fontName];
            _fontSize = size;
        }

        private void OpTextLeading(double textLeading)
        {
            _textLeading = textLeading;
        }

        private void OpTesDisplace(double x, double y)
        {
            FlushTextElement();
            var newMatrix = new Matrix3x3();
            newMatrix.Matrix[0, 2] = x;
            newMatrix.Matrix[1, 2] = y;
            _textMatrix = newMatrix.Multiply(_textMatrix);
        }

        private void OpTextLineFeed()
        {
            OpTesDisplace(0, -_textLeading);
        }

        private void OpSetTextMatrix(double a, double b, double c, double d, double e, double f)
        {
            FlushTextElement();
            _textMatrix.Matrix[0, 0] = a;
            _textMatrix.Matrix[1, 0] = b;
            _textMatrix.Matrix[2, 0] = 0;
            _textMatrix.Matrix[0, 1] = c;
            _textMatrix.Matrix[1, 1] = d;
            _textMatrix.Matrix[2, 1] = 0;
            _textMatrix.Matrix[0, 2] = e;
            _textMatrix.Matrix[1, 2] = f;
            _textMatrix.Matrix[2, 2] = 1;
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
                    double spacing = PdfElement_GetReal(elem, 0);
                    _textWidth += spacing; 
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
            foreach (PdfContentAction action in _page.ContentActions)
            {
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
                    double a = PdfElement_GetReal(action.Parameters[0], 0);
                    double b = PdfElement_GetReal(action.Parameters[1], 0);
                    double c = PdfElement_GetReal(action.Parameters[2], 0);
                    double d = PdfElement_GetReal(action.Parameters[3], 0);
                    double e = PdfElement_GetReal(action.Parameters[4], 0);
                    double f = PdfElement_GetReal(action.Parameters[5], 0);
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
                    double fontSize = PdfElement_GetReal(action.Parameters[1], 0);
                    OpTextFont(fontName, fontSize);
                }
                else if (action.Token == "TL")
                {
                    double leading = PdfElement_GetReal(action.Parameters[0], 0);
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
                    double x = PdfElement_GetReal(action.Parameters[0], 0);
                    double y = PdfElement_GetReal(action.Parameters[1], 0);
                    OpTesDisplace(x, y);
                }
                else if (action.Token == "TD")
                {
                    double x = PdfElement_GetReal(action.Parameters[0], 0);
                    double y = PdfElement_GetReal(action.Parameters[1], 0);
                    OpTextLeading(-y);
                    OpTesDisplace(x, y);
                }
                else if (action.Token == "Tm")
                {
                    double a = PdfElement_GetReal(action.Parameters[0], 0);
                    double b = PdfElement_GetReal(action.Parameters[1], 0);
                    double c = PdfElement_GetReal(action.Parameters[2], 0);
                    double d = PdfElement_GetReal(action.Parameters[3], 0);
                    double e = PdfElement_GetReal(action.Parameters[4], 0);
                    double f = PdfElement_GetReal(action.Parameters[5], 0);
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
                    double wordSpacing = PdfElement_GetReal(action.Parameters[0], 0);
                    double charSpacing = PdfElement_GetReal(action.Parameters[1], 0);
                    OpTextPut(((PdfString)action.Parameters[2]).Value);
                }
                else if (action.Token == "TJ")
                {
                    OpTextPutMultiple(((PdfArray)action.Parameters[0]));
                }
            }
            FlushTextElement();
        }

        #endregion
    }
}
