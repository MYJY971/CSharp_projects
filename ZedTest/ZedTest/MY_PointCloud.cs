using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MY_PC
{
    class MY_PointCloud
    {
        public List<Vector3d> _vertices;
        public List<Vector3> _faces;
        public List<Vector3d> _normales;
        public List<Vector3> _normalID;
        public List<Vector2d> _textures;
        public List<Vector3> _textureID;

        private bool _haveTexture;
        private bool _haveNormale;

        public int _nbVertices;
        public int _nbFaces;

        public MY_PointCloud(String filename)
        {
            _vertices = new List<Vector3d>();
            _faces = new List<Vector3>();
            _normales = new List<Vector3d>();
            _normalID = new List<Vector3>();
            _textures = new List<Vector2d>();
            _textureID = new List<Vector3>();
            _nbVertices = GetNbVertices();
            _nbFaces = GetNbFaces();
            _haveTexture = false;
            _haveNormale = false;

            Load(filename);
        }

        public void Load(String filename)
        {
            StreamReader file = new StreamReader(filename);
            string line;

            while (!file.EndOfStream)
            {
                line = file.ReadLine();

                if (!line.StartsWith("#"))
                {
                    String sVertex = line;

                    String[] sVal = sVertex.Split(' ');
                    double x = Convert.ToDouble(sVal[0].Replace(".", ","));
                    double y = Convert.ToDouble(sVal[1].Replace(".", ","));
                    double z = Convert.ToDouble(sVal[2].Replace(".", ","));

                    _vertices.Add(new Vector3d(x, y, z));
                }

            }

            file.Close();

            _nbVertices = GetNbVertices();
            
        }

        private int GetNbVertices()
        {
            return _vertices.Count();
        }

        private int GetNbFaces()
        {
            return _faces.Count();
        }

        public void Draw()
        {

            GL.Begin(BeginMode.Points);

            //foreach(Vector3 face in _mesh._faces)
            for (int i = 0; i < _vertices.Count(); i++)
            {
                GL.Vertex3(_vertices[i]);
                
            }

            GL.End();
        }
    }
}
