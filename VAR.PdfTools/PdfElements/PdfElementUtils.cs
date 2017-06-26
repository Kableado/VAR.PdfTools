namespace VAR.PdfTools.PdfElements
{
    public static class PdfElementUtils
    {
        public static double GetReal(IPdfElement elem, double defaultValue)
        {
            if (elem == null)
            {
                return defaultValue;
            }
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

        public static long GetInt(IPdfElement elem, long defaultValue)
        {
            if (elem == null)
            {
                return defaultValue;
            }
            if (elem is PdfInteger)
            {
                return ((PdfInteger)elem).Value;
            }
            if (elem is PdfReal)
            {
                return (long)((PdfReal)elem).Value;
            }
            return defaultValue;
        }

        public static string GetString(IPdfElement elem, string defaultValue)
        {
            if (elem == null)
            {
                return defaultValue;
            }
            if (elem is PdfString)
            {
                return ((PdfString)elem).Value;
            }
            if (elem is PdfName)
            {
                return ((PdfName)elem).Value;
            }
            return defaultValue;
        }
    }
}
