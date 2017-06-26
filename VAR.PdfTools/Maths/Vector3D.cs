namespace VAR.PdfTools.Maths
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
}
