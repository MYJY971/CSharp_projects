using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace MY_Shape
{
    class Quad : Shape
    {
        //private float size;
        private float _width;
        private float _height;

        #region constructeur
        public Quad()
            :this(new Vector3(0.0f,0.0f,0.0f))
        {
           
        }

        public Quad(Vector3 pos)
            :this(pos,1,1)
        {
            
        }

        public Quad(Vector3 pos, float width, float height)
        {
            this._width = width;
            this._height = height;

            SetPosition(pos);

            _normales.Add(new Vector3(0.0f, 0.0f, 1.0f));

            initUV(1);
        }

        public Quad(int size)
            :this()
        {
            Scale(size);
        }

        #endregion

        private void updateVertices()
        {
            _vertices = new List<Vector3>();
            _vertices.Add(new Vector3(_position.X - this._width / 2, _position.Y - this._height, 0.0f)); //p1 
            _vertices.Add(new Vector3(_position.X - this._width / 2, _position.Y + this._height, 0.0f)); //p2
            _vertices.Add(new Vector3(_position.X + this._width / 2, _position.Y + this._height, 0.0f)); //p3
            _vertices.Add(new Vector3(_position.X + this._width / 2, _position.Y - this._height, 0.0f));//p4
        }

        public override void SetPosition(Vector3 pos)
        {
            this._position = pos;
            updateVertices();
        }

        public override Vector3 GetPosition()
        {
            return this._position;
        }

        public override void Scale(float s)
        {
            this._width = _width * s;
            this._height = _height * s;
            updateVertices();
        }


        public override void GiveTexture(int idTexture)
        {
            this._idTexture = idTexture;
            this._haveTexture = true;
        }

        public override void GiveColor(Color color)
        {
            this._color = color;
            this._haveColor = true;
        }

        public override void Draw()
        {

            GL.Begin(BeginMode.Quads);

            GL.Normal3(_normales[0]);
            for (int i = 0; i < 4; i++)
            {
                GL.TexCoord2(_uv[i]);
                GL.Vertex3(_vertices[i]);
            }

            GL.End();

            GL.Color3(1.0f, 1.0f, 1.0f);
        }


        public override void initUV(float lenght)
        {
            _uv = new List<Vector2>();

            _uv.Add(new Vector2(0, lenght));
            _uv.Add(new Vector2(0, 0));
            _uv.Add(new Vector2(lenght, 0));
            _uv.Add(new Vector2(lenght, lenght));
        }

        public override void ScaleUV(int s)
        {
            initUV(s);
        }

        public override void Draw(bool textured)
        {
            if (textured && _haveTexture)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, this._idTexture);

                Draw();

                GL.Disable(EnableCap.Texture2D);
            }

            else
            {
                GL.Disable(EnableCap.Texture2D);
                if (_haveColor)
                    GL.Color3(this._color);
                Draw();
                
            }
        }
    }
}
