using System;

namespace VAR.PdfTools.Maths
{
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
}
