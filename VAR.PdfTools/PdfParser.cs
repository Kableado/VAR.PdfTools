using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace VAR.PdfTools
{
    public class PdfParser
    {
        #region Declarations

        private byte[] _stream = null;
        private long _streamPosition = 0;

        #endregion

        #region Creator

        public PdfParser(byte[] stream)
        {
            _stream = stream;
        }

        #endregion

        #region Utility methods

        private int ByteHexValue(byte character)
        {
            if (character >= 0x30 && character <= 0x39)
            {
                return (character - 0x30);
            }
            if (character >= 0x41 && character <= 0x46)
            {
                return 10 + (character - 0x41);
            }
            if (character >= 0x61 && character <= 0x66)
            {
                return 10 + (character - 0x61);
            }
            return 0;
        }

        private byte[] GetRawData(long length)
        {
            var memStream = new MemoryStream();
            var byteArray = new byte[1];

            for (long i = 0; i < length; i++)
            {
                byteArray[0] = PeekChar();
                NextChar();
                memStream.Write(byteArray, 0, 1);
            }

            return memStream.ToArray();
        }

        private bool TestMarker(long position, char[] marker)
        {
            for(int i = 0; i < marker.Length; i++)
            {
                if ((position + i) >= _stream.Length) { return false; }
                if(_stream[position+i] != marker[i]) { return false; }
            }
            return true;
        }

        private long MeasureToMarkers(char[][] markers)
        {
            long position = _streamPosition;
            do
            {
                foreach(char[] marker in markers)
                {
                    if (TestMarker(position, marker))
                    {
                        return (position - _streamPosition);
                    }
                }
                position++;
            } while (position < _stream.Length);
            return 0;
        }

        private byte PeekChar()
        {
            if (_streamPosition >= _stream.Length)
            {
                return 0;
            }
            return _stream[_streamPosition];
        }

        private byte PeekNextChar()
        {
            return PeekNextChar(1);
        }

        private byte PeekNextChar(int offset)
        {
            if ((_streamPosition + offset) >= _stream.Length)
            {
                return 0;
            }
            return _stream[_streamPosition + offset];
        }

        private bool NextChar()
        {
            _streamPosition++;
            if (_streamPosition >= _stream.Length)
            {
                return false;
            }
            return true;
        }

        private bool IsWhitespace(byte character)
        {
            if (
                character == 0x00 || // NULL
                character == 0x09 || // Horizontal Tab (HT)
                character == 0x0A || // Line Feed (LF)
                character == 0x0C || // Form Feed (FF)
                character == 0x0D || // Carriage Return (CR)
                character == 0x20 || // Space (SP)
                false)
            {
                return true;
            }
            return false;
        }

        private bool IsEndOfLine(byte character)
        {
            byte lineFeed = 0x0A;
            byte carriageReturn = 0x0D;
            if (
                character == lineFeed ||
                character == carriageReturn ||
                false)
            {
                return true;
            }
            return false;
        }

        private bool IsDelimiter(byte character)
        {
            if (
                character == '(' ||
                character == ')' ||
                character == '<' ||
                character == '>' ||
                character == '[' ||
                character == ']' ||
                character == '}' ||
                character == '{' ||
                character == '%' ||
                false)
            {
                return true;
            }
            return false;
        }

        private bool IsDigit(byte character)
        {
            if (character >= '0' && character <= '9')
            {
                return true;
            }
            return false;
        }

        private void SkipWhitespace()
        {
            while (IsWhitespace(PeekChar()))
            {
                _streamPosition++;
                if (_streamPosition >= _stream.Length)
                {
                    // EOS
                    break;
                }
            }
        }

        private void SkipEndOfLine()
        {
            byte lineFeed = 0x0A;
            byte carriageReturn = 0x0D;
            if (PeekChar() == carriageReturn)
            {
                NextChar();
                if (_streamPosition < _stream.Length)
                {
                    if (_stream[_streamPosition] == lineFeed)
                    {
                        _streamPosition++;
                    }
                }
                return;
            }
            if (PeekChar() == lineFeed)
            {
                NextChar();
                return;
            }
        }

        private void SkipToEndOfLine()
        {
            while (IsEndOfLine(PeekChar()) == false)
            {
                if (NextChar() == false)
                {
                    break;
                }
            }
        }

        private void SkipComment()
        {
            if (PeekChar() != '%') { return; }
            SkipToEndOfLine();
            SkipEndOfLine();
        }

        private string ParseComment()
        {
            if (PeekChar() != '%') { return string.Empty; }
            NextChar();
            StringBuilder sbComment = new StringBuilder();
            while (IsEndOfLine(PeekChar()) == false)
            {
                sbComment.Append((char)PeekChar());
                if (NextChar() == false)
                {
                    break;
                }
            }
            SkipEndOfLine();
            return sbComment.ToString();
        }

        private string ParseToken()
        {
            SkipWhitespace();
            StringBuilder sbToken = new StringBuilder();
            do
            {
                byte character = PeekChar();
                if (IsWhitespace(character) || IsDelimiter(character))
                {
                    break;
                }
                sbToken.Append((char)character);
                NextChar();
            } while (IsEndOfStream() == false);
            return sbToken.ToString();
        }
        
        private IPdfElement ParseElement()
        {
            IPdfElement obj = null;
            byte character = PeekChar();
            byte nextCharacter = PeekNextChar();

            if (character == 't' || character == 'f')
            {
                obj = ParseBoolean();
            }
            if (character == 'n')
            {
                obj = ParseNull();
            }
            else if (IsDigit(character) || character == '+' || character == '-' || character == '.')
            {
                obj = ParseNumberOrReference();
            }
            else if (character == '(' || (character == '<' && nextCharacter != '<'))
            {
                obj = ParseString();
            }
            else if (character == '/')
            {
                obj = ParseName();
            }
            else if (character == '[')
            {
                obj = ParseArray();
            }
            else if (character == '<' && nextCharacter == '<')
            {
                obj = ParseDictionary();
            }
            return obj;
        }

        private PdfBoolean ParseBoolean()
        {
            if (
                PeekNextChar(0) == 't' &&
                PeekNextChar(1) == 'r' &&
                PeekNextChar(2) == 'u' &&
                PeekNextChar(3) == 'e'
                )
            {
                NextChar();
                NextChar();
                NextChar();
                NextChar();
                return new PdfBoolean { Value = true };
            }

            if (
                PeekNextChar(0) == 'f' &&
                PeekNextChar(1) == 'a' &&
                PeekNextChar(2) == 'l' &&
                PeekNextChar(3) == 's' &&
                PeekNextChar(4) == 'e'
                )
            {
                NextChar();
                NextChar();
                NextChar();
                NextChar();
                NextChar();
                return new PdfBoolean { Value = false };
            }

            return null;
        }

        private PdfNull ParseNull()
        {
            if (
                PeekNextChar(0) == 'n' &&
                PeekNextChar(1) == 'u' &&
                PeekNextChar(2) == 'l' &&
                PeekNextChar(3) == 'l'
                )
            {
                NextChar();
                NextChar();
                NextChar();
                NextChar();
                return new PdfNull();
            }

            return null;
        }

        private IPdfElement ParseNumber()
        {
            long startPosition = _streamPosition;
            bool valid = false;
            int dotCount = 0;
            StringBuilder sbNumber = new StringBuilder();
            if (PeekChar() == '-')
            {
                sbNumber.Append('-');
                NextChar();
            }
            else if (PeekChar() == '+')
            {
                NextChar();
            }
            while (IsDigit(PeekChar()) || PeekChar() == '.')
            {
                if (PeekChar() == '.')
                {
                    sbNumber.Append(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                    dotCount++;
                }
                else
                {
                    sbNumber.Append((char)PeekChar());
                }
                NextChar();
                valid = true;
            }
            if (valid && dotCount <= 1)
            {
                if (dotCount == 0)
                {
                    PdfInteger obj = new PdfInteger();
                    obj.Value = Convert.ToInt32(sbNumber.ToString());
                    return obj;
                }
                if (dotCount == 1)
                {
                    PdfReal obj = new PdfReal();
                    obj.Value = Convert.ToDouble(sbNumber.ToString());
                    return obj;
                }
            }
            throw new Exception(string.Format("Expected number at {0}, found \"{1}\"", startPosition, sbNumber.ToString()));
        }

        private IPdfElement ParseNumberOrReference()
        {
            IPdfElement obj = ParseNumber();
            PdfInteger number = obj as PdfInteger;
            if (number == null)
            {
                return obj;
            }

            // Try to get an indirect object reference
            long streamPosition = _streamPosition;
            SkipWhitespace();
            if (char.IsDigit((char)PeekChar()) == false)
            {
                _streamPosition = streamPosition;
                return obj;
            }
            IPdfElement objectGeneration = ParseNumber();
            if((objectGeneration is PdfInteger) == false)
            {
                _streamPosition = streamPosition;
                return obj;
            }
            SkipWhitespace();
            if (PeekChar() != 'R')
            {
                _streamPosition = streamPosition;
                return obj;
            }
            NextChar();
            PdfObjectReference objRef = new PdfObjectReference();
            objRef.ObjectID = (int)number.Value;
            objRef.ObjectGeneration = (int)((PdfInteger)objectGeneration).Value;
            return objRef;
        }

        private PdfString ParseString()
        {
            if (PeekChar() == '(')
            {
                StringBuilder sbString = new StringBuilder();
                int depth = 1;
                NextChar();
                do
                {
                    byte character = PeekChar();
                    if (character == '(')
                    {
                        depth++;
                        sbString.Append((char)character);
                    }
                    else if (character == ')')
                    {
                        depth--;
                        if (depth == 0)
                        {
                            NextChar();
                            break;
                        }
                        else
                        {
                            sbString.Append((char)character);
                        }
                    }
                    else if (character == '\\')
                    {
                        if (NextChar() == false)
                        {
                            throw new Exception("Unexpected end of string and file");
                        }
                        character = PeekChar();
                        if (character == 'n')
                        {
                            sbString.Append('\n');
                        }
                        else if (character == 'r')
                        {
                            sbString.Append('\n');
                        }
                        else if (character == 't')
                        {
                            sbString.Append('\t');
                        }
                        else if (character == 'b')
                        {
                            sbString.Append('\b');
                        }
                        else if (character == 'f')
                        {
                            sbString.Append('\f');
                        }
                        else if (character == '(')
                        {
                            sbString.Append('(');
                        }
                        else if (character == ')')
                        {
                            sbString.Append(')');
                        }
                        else if (character == '\\')
                        {
                            sbString.Append('\\');
                        }
                        else if (IsEndOfLine(character))
                        {
                            SkipEndOfLine();
                            continue;
                        }
                        else if (IsDigit(character))
                        {
                            if (_streamPosition + 2 >= _stream.Length)
                            {
                                throw new Exception("Unexpected end of string and file");
                            }
                            StringBuilder sbOctal = new StringBuilder();
                            sbOctal.Append((char)character);
                            NextChar();
                            sbOctal.Append((char)PeekChar());
                            NextChar();
                            sbOctal.Append((char)PeekChar());
                            char newCharacter = (char)(Convert.ToInt32(sbOctal.ToString(), 8));
                        }
                    }
                    else
                    {
                        sbString.Append((Char)character);
                    }
                    if (NextChar() == false)
                    {
                        throw new Exception("Unexpected end of string and file");
                    }
                } while (IsEndOfStream() == false);

                PdfString obj = new PdfString();
                obj.Value = sbString.ToString();
                return obj;
            }
            else if (PeekChar() == '<')
            {
                StringBuilder sbString = new StringBuilder();
                NextChar();
                do
                {
                    byte character = PeekChar();
                    if (character == '>') { break; }
                    byte nextCharacter = PeekNextChar(1);
                    byte realChar = (byte)(ByteHexValue(character) * 16 + ByteHexValue(nextCharacter));
                    sbString.Append((Char)realChar);
                    NextChar();
                } while (IsEndOfStream() == false);
                NextChar();

                PdfString obj = new PdfString();
                obj.Value = sbString.ToString();
                return obj;
            }
            return null;
        }

        private PdfName ParseName()
        {
            if (PeekChar() != '/')
            {
                return null;
            }
            NextChar();
            StringBuilder sbName = new StringBuilder();
            do
            {
                byte character = PeekChar();
                if (IsDelimiter(character) || character == '/')
                {
                    break;
                }
                else if (character == '#')
                {
                    byte realChar = 0;
                    NextChar();
                    character = PeekChar();
                    realChar = (byte)(ByteHexValue(character) * 16);
                    NextChar();
                    character = PeekChar();
                    realChar += (byte)ByteHexValue(character);
                    sbName.Append((char)realChar);
                }
                else if (character > 0x20 && character < 0x7F)
                {
                    sbName.Append((char)character);
                }
                else
                {
                    break;
                }
                if (NextChar() == false)
                {
                    throw new Exception("Unexpected end of name and file");
                }
            } while (IsEndOfStream() == false);

            PdfName obj = new PdfName();
            obj.Value = sbName.ToString();
            return obj;
        }

        private PdfArray ParseArray()
        {
            if (PeekChar() != '[')
            {
                return null;
            }
            NextChar();
            SkipWhitespace();

            PdfArray array = new PdfArray();
            do
            {
                byte character = PeekChar();

                if (character == ']')
                {
                    NextChar();
                    break;
                }
                else
                {
                    IPdfElement obj = ParseElement();
                    array.Values.Add(obj);
                }
                SkipWhitespace();
            } while (IsEndOfStream() == false);
            return array;
        }

        private PdfDictionary ParseDictionary()
        {
            if (PeekChar() != '<' || PeekNextChar() != '<')
            {
                return null;
            }
            NextChar();
            NextChar();
            SkipWhitespace();

            PdfName previousName = null;
            PdfDictionary dict = new PdfDictionary();
            do
            {
                byte character = PeekChar();
                byte nextCharacter = PeekNextChar();

                if (character == '>' && nextCharacter == '>')
                {
                    NextChar();
                    NextChar();
                    break;
                }
                else if (character == '/')
                {
                    PdfName name = ParseName();
                    previousName = name;
                    SkipWhitespace();
                    IPdfElement obj = ParseElement();
                    if (obj is PdfNull)
                    {
                        dict.Values.Remove(name.Value);
                    }
                    else
                    {
                        previousName = name;
                        if (dict.Values.ContainsKey(name.Value))
                        {
                            dict.Values[name.Value] = obj;
                        }
                        else
                        {
                            dict.Values.Add(name.Value, obj);
                        }
                    }
                    SkipWhitespace();
                }
                else
                {
                    throw new Exception(string.Format("Error parsing Dictionary at: {0}", _streamPosition));
                }

            } while (IsEndOfStream() == false);
            return dict;
        }

        #endregion

        #region Public methods

        public PdfObject ParseObject()
        {
            PdfObject obj = null;
            long startPosition = _streamPosition;
            do
            {
                SkipWhitespace();
                byte character = PeekChar();

                if (character == '%')
                {
                    SkipComment();
                }
                else if (IsDigit(character))
                {
                    IPdfElement objectID = ParseNumber();
                    SkipWhitespace();
                    IPdfElement objectGeneration = ParseNumber();
                    SkipWhitespace();
                    string token = ParseToken();
                    if (token == "obj")
                    {
                        SkipWhitespace();
                        IPdfElement element = ParseElement();
                        string endToken = ParseToken();

                        // Intercept streams
                        if (endToken == "stream")
                        {
                            PdfDictionary streamDict = element as PdfDictionary;
                            if (streamDict == null)
                            {
                                throw new Exception(string.Format("Stream after a not dictionary element at: {0}", _streamPosition));
                            }
                            SkipEndOfLine();
                            long length;
                            if (streamDict.Values.ContainsKey("Length") && streamDict.Values["Length"] is PdfInteger)
                            {
                                length = ((PdfInteger)streamDict.Values["Length"]).Value;
                            }
                            else
                            {
                                byte lineFeed = 0x0A;
                                byte carriageReturn = 0x0D;
                                length = MeasureToMarkers(new char[][] {
                                    new char[] {(char)carriageReturn, (char)lineFeed, 'e', 'n', 'd', 's', 't', 'r', 'e', 'a', 'm'},
                                    new char[] {(char)lineFeed, 'e', 'n', 'd', 's', 't', 'r', 'e', 'a', 'm'},
                                    new char[] {'e', 'n', 'd', 's', 't', 'r', 'e', 'a', 'm', (char)lineFeed},
                                    new char[] {'e', 'n', 'd', 's', 't', 'r', 'e', 'a', 'm', (char)carriageReturn, (char)lineFeed},
                                });
                            }
                            byte[] streamBody = GetRawData(length);
                            SkipEndOfLine();
                            endToken = ParseToken();
                            if (endToken != "endstream")
                            {
                                throw new Exception(string.Format("Expected \"endstream\" token, \"{0}\" found at: {1}", token, _streamPosition));
                            }
                            SkipWhitespace();
                            endToken = ParseToken();
                            PdfStream stream = new PdfStream();
                            stream.Dictionary = streamDict;
                            stream.Data = streamBody;
                            element = stream;
                        }

                        if (endToken == "endobj")
                        {
                            obj = new PdfObject();
                            obj.ObjectID = (int)((PdfInteger)objectID).Value;
                            obj.ObjectGeneration = (int)((PdfInteger)objectGeneration).Value;
                            obj.Data = element;
                            break;
                        }
                    }
                }
                else
                {
                    long streamPosition = _streamPosition;
                    string token = ParseToken();
                    if (token == "startxref")
                    {
                        // FIXME: Ignoring startxref for now
                        SkipEndOfLine();
                        SkipToEndOfLine();
                        SkipEndOfLine();
                        SkipToEndOfLine();
                        SkipEndOfLine();
                        SkipWhitespace();
                        continue;
                    }
                    if (token == "xref")
                    {
                        // FIXME: Ignoring xref for now
                        SkipEndOfLine();
                        IPdfElement objNumber = ParseNumber();
                        SkipWhitespace();
                        objNumber = ParseNumber();
                        SkipEndOfLine();
                        PdfInteger refNumber = objNumber as PdfInteger;
                        for (int i = 0; i < refNumber.Value; i++)
                        {
                            SkipToEndOfLine();
                            SkipEndOfLine();
                        }
                        continue;
                    }
                    if (token == "trailer")
                    {
                        // FIXME: Ignoring trailer for now
                        SkipEndOfLine();
                        ParseElement();
                        SkipWhitespace();

                        SkipToEndOfLine();
                        SkipEndOfLine();
                        SkipToEndOfLine();
                        SkipEndOfLine();
                        SkipToEndOfLine();
                        SkipEndOfLine();
                        SkipWhitespace();
                        continue;
                    }
                    throw new Exception(string.Format("Expected objectID at {0}", startPosition));
                }
            } while (IsEndOfStream() == false);
            return obj;
        }

        public List<PdfObject> ParseObjectStream(int number, long first)
        {
            var streamObjects = new List<PdfObject>();
            var objectIds = new List<long>();
            for (int i = 0; i < number; i++)
            {
                SkipWhitespace();
                IPdfElement objectId = ParseElement();
                if (objectId is PdfInteger)
                {
                    objectIds.Add(((PdfInteger)objectId).Value);
                }
                else
                {
                    throw new System.Exception(string.Format("Unexpected element parsing ObjectStream at: {0}", _streamPosition));
                }
                SkipWhitespace();
                ParseElement();
            }
            _streamPosition = (int)first;
            for (int i = 0; i < number; i++)
            {
                SkipWhitespace();
                IPdfElement elem = ParseElement();
                if (elem == null)
                {
                    throw new System.Exception(string.Format("Unexpected error parsing ObjectStream at: {0}", _streamPosition));
                }

                PdfObject objAux = new PdfObject();
                objAux.ObjectGeneration = 0;
                objAux.ObjectID = (int)objectIds[i];
                objAux.Data = elem;
                streamObjects.Add(objAux);
            }
            return streamObjects;
        }

        public bool IsEndOfStream()
        {
            return _streamPosition >= _stream.Length;
        }

        #endregion
    }
}
