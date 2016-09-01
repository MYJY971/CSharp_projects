using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MY_Shape
{
    class Quad : Shape
    {
        //private float size;
        private float _width;
        private float _height;
        

        public Quad()
        {
            this._width = 1;
            this._height = 1;

            SetPosition(new Vector3(0.0f, 0.0f, 0.0f));

            updateVertices(_width, _height);
        //_vertices.Add(new Vector3(-width/2, -height, 0.0f)); //p1 
        //_vertices.Add(new Vector3(-width/2, height, 0.0f)); //p2
        //_vertices.Add(new Vector3(width/2, height, 0.0f)); //p3
        //_vertices.Add(new Vector3(width/2, -height, 0.0f));//p4

        _normales.Add(new Vector3(0.0f, 0.0f, 1.0f));

            _uv.Add(new Vector2(0, 1));
            _uv.Add(new Vector2(0, 0));
            _uv.Add(new Vector2(1, 0));
            _uv.Add(new Vector2(1, 1));

            
        }

        public Quad(Vector3 pos)
        {
            
        }


        private void updateVertices(float w, float h)
        {
            _vertices = new List<Vector3>();
            _vertices.Add(new Vector3(_position.X - w / 2, _position.Y - h, 0.0f)); //p1 
            _vertices.Add(new Vector3(_position.X - w / 2, _position.Y + h, 0.0f)); //p2
            _vertices.Add(new Vector3(_position.X + w / 2, _position.Y + h, 0.0f)); //p3
            _vertices.Add(new Vector3(_position.X + w / 2, _position.Y - h, 0.0f));//p4
        }

        public override void SetPosition(Vector3 pos)
        {
            this._position=pos;
        }

        public override Vector3 GetPosition()
        {
            return this._position;
        }

        public override void Scale(float s)
        {
            this._width = _width * s;
            this._height = _height * s;
            updateVertices(_width, _height);
        }

        public override void Draw()
        {
            GL.Begin(BeginMode.Quads);

            GL.Normal3(_normales[0]);
            for(int i = 0; i<4; i++)
            {
                GL.TexCoord2(_uv[i]);
                GL.Vertex3(_vertices[i]);
            }

            GL.End();
        }
    }
}
