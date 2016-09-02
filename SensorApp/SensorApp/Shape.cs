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
    public abstract class Shape
    {
        protected List<Vector3> _vertices;
        protected List<Vector3> _normales = new List<Vector3>();
        protected List<Vector2> _uv;
        

        protected int _idTexture;
        protected bool _haveTexture;

        protected Color _color;
        protected bool _haveColor;

        protected Vector3 _position;

        public abstract void SetPosition(Vector3 pos);
        public abstract Vector3 GetPosition();

        public abstract void Scale(float s);

        public abstract void initUV(float lenght);
        public abstract void GiveTexture(int idTexture);
        public abstract void ScaleUV(int s);

        public abstract void GiveColor(Color color);

        public abstract void Draw();
        public abstract void Draw(bool textured);

        
    }
}
