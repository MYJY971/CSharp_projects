using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MY_Shape
{
    public abstract class Shape
    {
        protected List<Vector3> _vertices = new List<Vector3>();
        protected List<Vector3> _normales = new List<Vector3>();
        protected List<Vector2> _uv = new List<Vector2>();

        protected Vector3 _position;

        public abstract void SetPosition(Vector3 pos);
        public abstract Vector3 GetPosition();

        public abstract void Scale(float s);

        public abstract void Draw();

        
    }
}
