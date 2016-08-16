using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYTestOpenGl
{
    public class Vector : Matrix
    {
        private float _x, _y, _z, _w;
        private int _size;

        //Constructeur 
        public Vector(float x, float y, float z, float w)
            : base(new float[4, 1] { { x }, { y }, { z }, { w } })
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
            this._size = 4;
        }

        public Vector(float x, float y, float z)
            : base(new float[3, 1] { { x }, { y }, { z } })
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = 0;
            this._size = 3;
        }

        public Vector(Matrix mat)
            : base(mat)
        {

            try
            {
                if (mat.GetRow() <= 4)
                {
                    //this._values = mat.GetValues();

                    this._x = (float)mat.GetValue(0, 0);
                    this._y = (float)mat.GetValue(1, 0);
                    this._z = (float)mat.GetValue(2, 0);



                    if (mat.GetRow() == 4)
                    {
                        this._w = (float)mat.GetValue(3, 0);
                    }
                    else
                    {
                        this._w = 0;
                    }

                    this._size = mat.GetRow();

                    Update();
                }
                else
                    throw new System.IndexOutOfRangeException();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }


        }


        //Méthodes

        /// <summary>
        /// Met à jour les valeurs d'un vecteur
        /// </summary>
        private void Update()
        {
            if (this._size == 3)
            {
                this.SetValues(new float[3, 1] { { this._x }, { this._y }, { this._z } });
            }
            else
            {
                this.SetValues(new float[4, 1] { { this._x }, { this._y }, { this._z }, { this._w } });
            }
        }

        /// <summary>
        /// Multiplie le vecteur courant par une matrice de rotation
        /// </summary>
        /// <param name="axe">axe de rotation: 0=x; 1=y; 2=z</param>
        /// <param name="angle">angle de rotation</param>
        /// <returns></returns>
        public new Vector Rotate(int axe, float angle)
        {
            return new Vector(ProductMat(new RotationMatrix(axe, angle)));
        }

        //Récupérer les coordonnées d'un vecteur
        public float GetX()
        {
            return this._x;
        }

        public float GetY()
        {
            return this._y;
        }

        public float GetZ()
        {
            return this._z;
        }

        public float GetW()
        {
            return this._w;
        }


        /*//Pour les couleurs
        public float GetR()
        {
            return this._x;
        }

        public float GetG()
        {
            return this._y;
        }

        public float GetB()
        {
            return this._z;
        }

        public float GetA()
        {
            return this._w;
        }*/

        /// <summary>
        /// Translation
        /// </summary>
        /// <param name="v"></param>
        public Vector Translate(int coeff, Vector v)
        {
            float x, y, z, w;
            x = this._x + coeff * v.GetX();
            y = this._y + coeff * v.GetY();
            z = this._z + coeff * v.GetZ();

            if (v.GetRow() == 4)
            {
                w = this._w + coeff * v.GetW();
                return new Vector(x, y, z, w);
            }  
            else
            {
                w = 0;
                return new Vector(x, y, z);
            }
                

                       
        }

        public Vector Translate(Vector v)
        {
            return Translate(1, v);
        }


    }
}
