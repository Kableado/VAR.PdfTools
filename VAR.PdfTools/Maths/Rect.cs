using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAR.PdfTools.Maths
{
    public class Rect
    {
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }

        public void Add(Rect rect)
        {
            if (rect.XMax > XMax) { XMax = rect.XMax; }
            if (rect.YMax > YMax) { YMax = rect.YMax; }
            if (rect.XMin < XMin) { XMin = rect.XMin; }
            if (rect.YMin < YMin) { YMin = rect.YMin; }
        }

    }
}
