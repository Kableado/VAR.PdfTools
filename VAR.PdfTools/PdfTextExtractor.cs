﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAR.PdfTools.Maths;
using VAR.PdfTools.PdfElements;

namespace VAR.PdfTools
{
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
        private double _charSpacing = 0;
        private double _wordSpacing = 0;
        private double _textLeading = 0;

        // Text object state
        private bool inText = false;
        private Matrix3x3 _textMatrix = new Matrix3x3();
        private Matrix3x3 _textMatrixCurrent = new Matrix3x3();
        private StringBuilder _sbText = new StringBuilder();
        private double _textWidth = 0;
        private List<PdfCharElement> _listCharacters = new List<PdfCharElement>();

        #endregion

        #region Properties

        public PdfDocumentPage Page { get { return _page; } }

        public List<PdfTextElement> Elements { get { return _textElements; } }

        #endregion

        #region lifecycle 

        public PdfTextExtractor(PdfDocumentPage page)
        {
            _page = page;
            ProcessPageContent();
            JoinTextElements();
            SplitTextElements();
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
            textElem.Matrix = _textMatrixCurrent.Multiply(_graphicsMatrix);
            textElem.RawText = _sbText.ToString();
            textElem.VisibleText = PdfString_ToUnicode(textElem.RawText, _font);
            PdfCharElement lastChar = _listCharacters[_listCharacters.Count - 1];
            textElem.VisibleWidth = (lastChar.Displacement + lastChar.Width) * textElem.Matrix.Matrix[0, 0];
            textElem.VisibleHeight = (_font.Height * _fontSize) * textElem.Matrix.Matrix[1, 1];
            textElem.Characters = new List<PdfCharElement>();
            foreach (PdfCharElement c in _listCharacters)
            {
                textElem.Characters.Add(new PdfCharElement
                {
                    Char = c.Char,
                    Displacement = (c.Displacement * textElem.Matrix.Matrix[0, 0]),
                    Width = (c.Width * textElem.Matrix.Matrix[0, 0]),
                });
            }
            textElem.Childs = new List<PdfTextElement>();
            return textElem;
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
                _textWidth = 0;
                return;
            }
            PdfTextElement textElem = BuildTextElement();
            AddTextElement(textElem);

            _textMatrixCurrent.Matrix[0, 2] += _textWidth;

            _sbText = new StringBuilder();
            _listCharacters.Clear();
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

        private List<PdfTextElement> FindElementsContainingText(string text, bool fuzzy)
        {
            List<PdfTextElement> list = new List<PdfTextElement>();
            string matchingText = fuzzy ? SimplifyText(text) : text;
            foreach (PdfTextElement elem in _textElements)
            {
                string elemText = fuzzy ? SimplifyText(elem.VisibleText) : elem.VisibleText;
                if (elemText.Contains(matchingText))
                {
                    list.Add(elem);
                }
            }
            return list;
        }

        private bool TextElementVerticalIntersection(PdfTextElement elem1, double elem2X1, double elem2X2)
        {
            double elem1X1 = elem1.GetX();
            double elem1X2 = elem1.GetX() + elem1.VisibleWidth;

            return elem1X2 >= elem2X1 && elem2X2 >= elem1X1;
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

        private void OpSetGraphMatrix(double a, double b, double c, double d, double e, double f)
        {
            _graphicsMatrix.Set(a, b, c, d, e, f);
        }

        private void OpPopGraphState()
        {
            _graphicsMatrix = _graphicsMatrixStack[_graphicsMatrixStack.Count - 1];
            _graphicsMatrixStack.RemoveAt(_graphicsMatrixStack.Count - 1);
        }

        private void OpBeginText()
        {
            _textMatrix.Idenity();
            _textMatrixCurrent.Idenity();
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

        private void OpTextCharSpacing(double charSpacing)
        {
            _charSpacing = charSpacing;
        }

        private void OpTextWordSpacing(double wordSpacing)
        {
            _wordSpacing = wordSpacing;
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
            _textMatrixCurrent = _textMatrix.Copy();
        }

        private void OpTextLineFeed()
        {
            OpTextDisplace(0, -_textLeading);
        }

        private void OpSetTextMatrix(double a, double b, double c, double d, double e, double f)
        {
            Matrix3x3 newMatrix = new Matrix3x3(a, b, c, d, e, f);
            FlushTextElement();
            _textMatrix = newMatrix;
            _textMatrixCurrent = _textMatrix.Copy();
        }

        private void OpTextPut(string text)
        {
            if (inText == false) { return; }
            _sbText.Append(text);
            if (_font != null)
            {
                foreach (char c in text)
                {
                    string realChar = _font.ToUnicode(c);
                    if (realChar == "\0") { continue; }
                    double charWidth = _font.GetCharWidth(c) * _fontSize;
                    _listCharacters.Add(new PdfCharElement { Char = _font.ToUnicode(c), Displacement = _textWidth, Width = charWidth });
                    _textWidth += charWidth;
                    _textWidth += ((c == 0x20) ? _wordSpacing : _charSpacing);
                }
            }
        }

        private void OpTextPutMultiple(PdfArray array)
        {
            if (inText == false) { return; }
            foreach (IPdfElement elem in array.Values)
            {
                if (elem is PdfString)
                {
                    OpTextPut(((PdfString)elem).Value);
                }
                else if (elem is PdfInteger || elem is PdfReal)
                {
                    double spacing = PdfElementUtils.GetReal(elem, 0);
                    _textWidth -= (spacing / 1000) * _fontSize;
                }
                else if (elem is PdfArray)
                {
                    OpTextPutMultiple(((PdfArray)elem));
                }
            }
        }

        #endregion

        #region Private methods

        private void ProcessPageContent()
        {
            int unknowCount = 0;
            int lineCount = 0;
            int strokeCount = 0;
            int pathCount = 0;
            for (int i = 0; i < _page.ContentActions.Count; i++)
            {
                PdfContentAction action = _page.ContentActions[i];

                // Special graphics state
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
                    double charSpacing = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    OpTextCharSpacing(charSpacing);
                }
                else if (action.Token == "Tw")
                {
                    double wordSpacing = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    OpTextWordSpacing(wordSpacing);
                }
                else if (action.Token == "Tz")
                {
                    // TODO: PdfTextExtractor: Horizontal Scale
                }
                else if (action.Token == "Tf")
                {
                    string fontName = PdfElementUtils.GetString(action.Parameters[0], string.Empty);
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
                    // TODO: PdfTextExtractor: Rendering mode
                }
                else if (action.Token == "Ts")
                {
                    // TODO: PdfTextExtractor: Text rise
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
                    string text = PdfElementUtils.GetString(action.Parameters[0], string.Empty);
                    OpTextPut(text);
                }
                else if (action.Token == "'")
                {
                    string text = PdfElementUtils.GetString(action.Parameters[0], string.Empty);
                    OpTextLineFeed();
                    OpTextPut(text);
                }
                else if (action.Token == "\"")
                {
                    double wordSpacing = PdfElementUtils.GetReal(action.Parameters[0], 0);
                    double charSpacing = PdfElementUtils.GetReal(action.Parameters[1], 0);
                    string text = PdfElementUtils.GetString(action.Parameters[0], string.Empty);
                    OpTextCharSpacing(charSpacing);
                    OpTextWordSpacing(wordSpacing);
                    OpTextPut(text);
                }
                else if (action.Token == "TJ")
                {
                    OpTextPutMultiple(((PdfArray)action.Parameters[0]));
                }
                else if (action.Token == "re")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "f")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "g")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "rg")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "BI")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "ID")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "EI")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "W")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "n")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "Do")
                {
                    // TODO: PdfTextExtractor: Interpret this
                }
                else if (action.Token == "m")
                {
                    // TODO: PdfTextExtractor: Interpret this "moveto: Begin new subpath"
                }
                else if (action.Token == "l")
                {
                    // TODO: PdfTextExtractor: Interpret this "lineto: Append straight line segment to path"
                    lineCount++;
                }
                else if (action.Token == "h")
                {
                    // TODO: PdfTextExtractor: Interpret this "closepath: Close subpath"
                    pathCount++;
                }
                else if (action.Token == "W")
                {
                    // TODO: PdfTextExtractor: Interpret this "clip: Set clipping path using nonzero winding number rule"
                }
                else if (action.Token == "W*")
                {
                    // TODO: PdfTextExtractor: Interpret this "eoclip: Set clipping path using even-odd rule"
                }
                else if (action.Token == "w")
                {
                    // TODO: PdfTextExtractor: Interpret this "setlinewidth: Set line width"
                }
                else if (action.Token == "G")
                {
                    // TODO: PdfTextExtractor: Interpret this "setgray: Set gray level for stroking operations"
                }
                else if (action.Token == "S")
                {
                    // TODO: PdfTextExtractor: Interpret this "stroke: Stroke path"
                    strokeCount++;
                }
                else if (action.Token == "M")
                {
                    // TODO: PdfTextExtractor: Interpret this "setmiterlimit: Set miter limit"
                }
                else
                {
                    unknowCount++;
                }
            }
            FlushTextElement();
        }

        private void JoinTextElements()
        {
            var textElementsCondensed = new List<PdfTextElement>();
            while (_textElements.Count > 0)
            {
                PdfTextElement elem = _textElements[0];
                _textElements.Remove(elem);
                double blockY = elem.GetY();
                double blockXMin = elem.GetX();
                double blockXMax = blockXMin + elem.VisibleWidth;

                // Prepare first neighbour
                var textElementNeighbours = new List<PdfTextElement>();
                textElementNeighbours.Add(elem);

                // Search Neighbours
                int i = 0;
                while (i < _textElements.Count)
                {
                    PdfTextElement neighbour = _textElements[i];

                    if (neighbour.Font != elem.Font || neighbour.FontSize != elem.FontSize)
                    {
                        i++;
                        continue;
                    }

                    double neighbourY = neighbour.GetY();
                    if (Math.Abs(neighbourY - blockY) > 0.001) { i++; continue; }

                    double maxWidth = neighbour.MaxWidth();

                    double neighbourXMin = neighbour.GetX();
                    double neighbourXMax = neighbourXMin + neighbour.VisibleWidth;
                    double auxBlockXMin = blockXMin - maxWidth;
                    double auxBlockXMax = blockXMax + maxWidth;
                    if (auxBlockXMax >= neighbourXMin && neighbourXMax >= auxBlockXMin)
                    {
                        _textElements.Remove(neighbour);
                        textElementNeighbours.Add(neighbour);
                        if (blockXMax < neighbourXMax) { blockXMax = neighbourXMax; }
                        if (blockXMin > neighbourXMin) { blockXMin = neighbourXMin; }
                        i = 0;
                        continue;
                    }
                    i++;
                }

                if (textElementNeighbours.Count == 1)
                {
                    textElementsCondensed.Add(elem);
                    continue;
                }

                // Join neighbours
                var chars = new List<PdfCharElement>();
                foreach (PdfTextElement neighbour in textElementNeighbours)
                {
                    double neighbourXMin = neighbour.GetX();
                    foreach (PdfCharElement c in neighbour.Characters)
                    {
                        chars.Add(new PdfCharElement
                        {
                            Char = c.Char,
                            Displacement = (c.Displacement + neighbourXMin) - blockXMin,
                            Width = c.Width,
                        });
                    }
                }
                chars = chars.OrderBy(c => c.Displacement).ToList();
                var sbText = new StringBuilder();
                foreach (PdfCharElement c in chars)
                {
                    sbText.Append(c.Char);
                }
                PdfTextElement blockElem = new PdfTextElement
                {
                    Font = null,
                    FontSize = elem.FontSize,
                    Matrix = elem.Matrix.Copy(),
                    RawText = sbText.ToString(),
                    VisibleText = sbText.ToString(),
                    VisibleWidth = blockXMax - blockXMin,
                    VisibleHeight = elem.VisibleHeight,
                    Characters = chars,
                    Childs = textElementNeighbours,
                };
                blockElem.Matrix.Matrix[0, 2] = blockXMin;
                textElementsCondensed.Add(blockElem);
            }
            _textElements = textElementsCondensed;
        }

        private void SplitTextElements()
        {
            var textElementsSplitted = new List<PdfTextElement>();
            while (_textElements.Count > 0)
            {
                PdfTextElement elem = _textElements[0];
                _textElements.Remove(elem);

                double maxWidth = elem.MaxWidth();

                int prevBreak = 0;
                for (int i = 1; i < elem.Characters.Count; i++)
                {
                    double prevCharEnd = elem.Characters[i - 1].Displacement + elem.Characters[i - 1].Width;
                    double charSeparation = elem.Characters[i].Displacement - prevCharEnd;
                    if (charSeparation > maxWidth)
                    {
                        PdfTextElement partElem = elem.SubPart(prevBreak, i);
                        textElementsSplitted.Add(partElem);
                        prevBreak = i;
                    }
                }

                if (prevBreak == 0)
                {
                    textElementsSplitted.Add(elem);
                    continue;
                }
                PdfTextElement lastElem = elem.SubPart(prevBreak, elem.Characters.Count);
                textElementsSplitted.Add(lastElem);
            }
            _textElements = textElementsSplitted;
        }

        #endregion

        #region Public methods

        public Rect GetRect()
        {
            Rect rect = null;
            foreach (PdfTextElement textElement in _textElements)
            {
                Rect elementRect = textElement.GetRect();
                if (rect == null) { rect = elementRect; }
                rect.Add(elementRect);
            }
            return rect;
        }

        public PdfTextElementColumn GetColumn(string column, bool fuzzy = true)
        {
            PdfTextElement columnHead = FindElementByText(column, fuzzy);
            if (columnHead == null)
            {
                return PdfTextElementColumn.Empty;
            }
            double headY = columnHead.GetY() - columnHead.VisibleHeight;
            double headX1 = columnHead.GetX();
            double headX2 = headX1 + columnHead.VisibleWidth;

            // Determine horizontal extent
            double extentX1 = double.MinValue;
            double extentX2 = double.MaxValue;
            foreach (PdfTextElement elem in _textElements)
            {
                if (elem == columnHead) { continue; }
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

            PdfTextElementColumn columnData = GetColumn(columnHead, headY, headX1, headX2, extentX1, extentX2);

            return columnData;
        }

        public PdfTextElementColumn GetColumn(PdfTextElement columnHead, double headY, double headX1, double headX2, double extentX1, double extentX2)
        {
            // Get all the elements that intersects vertically, are down and sort results
            var columnDataRaw = new List<PdfTextElement>();
            foreach (PdfTextElement elem in _textElements)
            {
                if (TextElementVerticalIntersection(elem, headX1, headX2) == false) { continue; }

                // Only intems down the column
                double elemY = elem.GetY();
                if (elemY >= headY) { continue; }

                columnDataRaw.Add(elem);
            }
            columnDataRaw = columnDataRaw.OrderByDescending(elem => elem.GetY()).ToList();

            // Only items completelly inside extents, try spliting big elements and break on big elements that can't be splitted
            var columnElements = new List<PdfTextElement>();
            foreach (PdfTextElement elem in columnDataRaw)
            {
                double elemX1 = elem.GetX();
                double elemX2 = elemX1 + elem.VisibleWidth;

                // Add elements completely inside
                if (elemX1 > extentX1 && elemX2 < extentX2)
                {
                    columnElements.Add(elem);
                    continue;
                }

                // Try to split elements intersecting extents of the column
                double maxSpacing = elem.Characters.Average(c => c.Width) / 10;
                int indexStart = 0;
                int indexEnd = elem.Characters.Count - 1;
                bool indexStartValid = true;
                bool indexEndValid = true;
                if (elemX1 < extentX1)
                {
                    // Search best start
                    int index = 0;
                    double characterPosition = elemX1 + elem.Characters[index].Displacement;
                    while (characterPosition < extentX1 && index < (elem.Characters.Count - 1))
                    {
                        index++;
                        characterPosition = elemX1 + elem.Characters[index].Displacement;
                    }
                    double spacing = elem.GetCharacterPreviousSpacing(index);
                    while (spacing < maxSpacing && index < (elem.Characters.Count - 1))
                    {
                        index++;
                        spacing = elem.GetCharacterPreviousSpacing(index);
                    }
                    if (spacing < maxSpacing) { indexStartValid = false; }
                    indexStart = index;
                }

                if (elemX2 > extentX2)
                {
                    // Search best end
                    int index = elem.Characters.Count - 1;
                    double characterPosition = elemX1 + elem.Characters[index].Displacement + elem.Characters[index].Width;
                    while (characterPosition > extentX2 && index > 0)
                    {
                        index--;
                        characterPosition = elemX1 + elem.Characters[index].Displacement + elem.Characters[index].Width;
                    }
                    double spacing = elem.GetCharacterPrecedingSpacing(index);
                    while (spacing < maxSpacing && index > 0)
                    {
                        index--;
                        spacing = elem.GetCharacterPrecedingSpacing(index);
                    }
                    if (spacing < maxSpacing) { indexEndValid = false; }
                    indexEnd = index;
                }

                // Break when there is no good split, spaning all extent
                if (indexStartValid == false && indexEndValid == false) { break; }

                // Continue when only one of the sides is invalid. (outside elements intersecting extents of the column)
                if (indexStartValid == false || indexEndValid == false) { continue; }

                // Add splitted element
                columnElements.Add(elem.SubPart(indexStart, indexEnd + 1));
            }

            var columnData = new PdfTextElementColumn(columnHead, columnElements, headY, extentX1, extentX2);
            return columnData;
        }

        public List<string> GetColumnAsStrings(string column, bool fuzzy = true)
        {
            PdfTextElementColumn columnData = GetColumn(column, fuzzy);

            // Emit result
            var result = new List<string>();
            foreach (PdfTextElement elem in columnData.Elements)
            {
                result.Add(elem.VisibleText);
            }
            return result;
        }

        public string GetFieldAsString(string field, bool fuzzy = true)
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

            if (fieldData.Count == 0)
            {
                return null;
            }

            return fieldData.OrderBy(elem => elem.GetX()).FirstOrDefault().VisibleText;
        }

        public bool HasText(string text, bool fuzzy = true)
        {
            List<PdfTextElement> list = FindElementsContainingText(text, fuzzy);
            return (list.Count > 0);
        }

        #endregion
    }
}
