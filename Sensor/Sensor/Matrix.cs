using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYTestOpenGl
{
    public class Matrix
    {
        protected float[,] _values;
        protected int _row, _col, _size;

        /*Constructeur*/

        /// <summary>
        /// Initialise une matrice à 0 de taille [row x col]
        /// </summary>
        /// <param name="row"> nombre de ligne </param>
        /// <param name="col"> nombre de colonne </param>
        public Matrix(int row, int col)
        {
            this._row = row;
            this._col = col;
            this._values = new float[this._row, this._col];
            ComputeSize();
        }

        /// <summary>
        /// Matrice carré identité de taille n
        /// </summary>
        /// <param name="n"></param>
        public Matrix(int n)
        {
            this._row = n;
            this._col = n;
            this._values = new float[this._row, this._col];
            ComputeSize();
            Identity();
        }

        /// <summary>
        /// Initialize la matrice avec un tableau
        /// </summary>
        /// <param name="tab"></param>
        public Matrix(float[,] tab)
        {
            this._row = tab.GetLength(0);
            this._col = tab.GetLength(1);
            this._values = tab;

        }

        /// <summary>
        /// Initialise une matrice avec à partir d'une autre matrice
        /// </summary>
        /// <param name="mat"> matrice </param>
        public Matrix(Matrix mat)
        {
            this._row = mat._row;
            this._col = mat._col;
            this._size = mat._size;
            this._values = mat._values;
        }

        /// <summary>
        /// Initialise une matrice à partir d'un vecteur
        /// </summary>
        /// <param name="vect"> vecteur </param>
        public Matrix(Vector vect)
        {
            this._row = vect._row;
            this._col = vect._col;
            this._size = vect._size;
            this._values = vect._values;
        }


        /*Méthodes*/

        /// <summary>
        /// Calcul de la taille
        /// </summary>
        private void ComputeSize()
        {
            this._size = this._row * this._col;
        }

        /// <summary>
        /// Pour la création d'une matrice identité
        /// </summary>
        private void Identity()
        {
            for (int r = 0; r < _row; ++r)
            {
                for (int c = 0; c < _col; ++c)
                {
                    if (c == r)
                    {
                        _values[r, c] = 1f;
                        c = _col;
                    }
                }
            }
        }

        /// <summary>
        /// Réinitialise les valeurs de la matrice avec un tableau
        /// </summary>
        /// <param name="values"> tableau de valeur</param>
        public void SetValues(float[,] values)
        {
            try
            {
                if (this._values.GetLength(0) == values.GetLength(0) &&
                    this._values.GetLength(1) == values.GetLength(1))
                {
                    this._values = values;
                }

                else
                    throw new Exception("Size Error");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("" + e);
            }

        }

        /// <summary>
        /// Pour se servir des méthodes de la bibliotèque CSML
        /// </summary>
        /// <returns></returns>
        private MatrixCSML ConvertCSML()
        {
            return new MatrixCSML(this._values);
        }

        /// <summary>
        /// Matrice inverse, obtenu à l'aide de la bibliothèque CSML
        /// </summary>
        /// <returns></returns>
        public Matrix Inverse()
        {
            MatrixCSML tmp = ConvertCSML();
            tmp = tmp.Inverse();
            return tmp.ConvertMYMatrix();
        }

        /// <summary>
        /// Produit matriciel : multiplication de la matrice courante par une autre matrice
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public Matrix ProductMat(Matrix mat)
        {
            float[,] newVal = new float[this._row, mat._col];
            try
            {
                if (this._col == mat._row)
                {
                    for (int r = 0; r < this._row; ++r)
                    {

                        for (int c = 0; c < mat._col; ++c)
                        {
                            int row = 0;
                            while (row < this._col)
                            //for (int row = 0; row < this._row; ++row)
                            {
                                for (int col = 0; col < mat._row; col++)
                                {
                                    newVal[r, c] = newVal[r, c] + this._values[r, row] * mat._values[col, c];
                                    row++;
                                }
                            }


                        }
                    }


                }

                else
                    throw new Exception("Size Error");


            }
            catch (Exception e)
            {

                Console.Error.WriteLine("" + e);
            }

            return new Matrix(newVal);
        }


        /// <summary>
        /// Multiplication de la matrice courante par un vecteur
        /// </summary>
        /// <param name="vect"> vecteur </param>
        /// <returns></returns>
        public Vector ProductMat(Vector vect)
        {
            Matrix res = new Matrix(vect);
            return new Vector(ProductMat(res));
        }

        /// <summary>
        /// Multiplie la matrice courante par une matrice de rotation
        /// </summary>
        /// <param name="axe">axe de rotation: 0=x; 1=y; 2=z</param>
        /// <param name="angle">angle de rotation</param>
        /// <returns></returns>
        public Matrix Rotate(int axe, float angle)
        {
            return ProductMat(new RotationMatrix(axe, angle));
        }

        /// <summary>
        /// Récupère la valeur situé à la position [r,c]
        /// </summary>
        /// <param name="r">indice des lignes (de 0 à nb lignes -1)</param>
        /// <param name="c">indice des colonnes (de 0 à nb colonnes -1)</param>
        /// <returns></returns>
        public object GetValue(int r, int c)
        {
            return this._values.GetValue(r, c);
        }

        /// <summary>
        /// Retourne le nombre de lignes
        /// </summary>
        /// <returns></returns>
        public int GetRow()
        {
            return this._row;
        }

        /// <summary>
        /// Retourne le nombre de colonnes
        /// </summary>
        /// <returns></returns>
        public int GetCol()
        {
            return this._col;
        }

        /// <summary>
        /// Retourne le tableau de valeurs de la matrice
        /// </summary>
        /// <returns></returns>
        public float[,] GetValues()
        {
            return this._values;
        }


        /// <summary>
        /// Ecrit la matrice dans un string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string stringValues = "\n";

            for (int r = 0; r < _row; ++r)
            {
                for (int c = 0; c < _col; ++c)
                {
                    stringValues = stringValues + " " + _values[r, c];
                }
                stringValues = stringValues + "\n";
            }

            return stringValues;

        }



    }
}