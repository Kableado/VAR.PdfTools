using System.Collections.Generic;

namespace VAR.PdfTools
{
    public class PdfContentAction
    {
        #region Declarations

        string _token = null;

        private List<IPdfElement> _parameters = null;

        #endregion

        #region Properties

        public string Token { get { return _token; } }

        public List<IPdfElement> Parameters { get { return _parameters; } }

        #endregion

        #region Life cycle

        public PdfContentAction(string token, List<IPdfElement> parameters)
        {
            _token = token;
            _parameters = parameters;
        }

        #endregion
    }
}
