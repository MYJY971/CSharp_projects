using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;

namespace MY_Aruco_v2
{
    class Mesh
    {
        public List<Vector3d> _vertices;
        private List<Vector3> _faces;

        public int _nbVertices;
        public int _nbFaces;

        public Mesh()
        {
            _vertices = new List<Vector3d>();
            _faces = new List<Vector3>();
            _nbVertices = GetNbVertices();
            _nbFaces = GetNbFaces();
        }

        public void Load(String filename)
        {
            StreamReader file = new StreamReader(filename);
            string line;

            while (!file.EndOfStream)
            {
                line = file.ReadLine();

                if (line.StartsWith("v"))
                {
                    String sVertex = line.Substring(2);

                    String[] sVal = sVertex.Split(' ');
                    double x = Convert.ToDouble(sVal[0].Replace(".", ","));
                    double y = Convert.ToDouble(sVal[1].Replace(".", ","));
                    double z = Convert.ToDouble(sVal[2].Replace(".", ","));

                    _vertices.Add(new Vector3d(x, y, z));
                }
                else
                {
                    if (line.StartsWith("f"))
                    {
                        String sFace = line.Substring(2);

                        String[] sVal = sFace.Split(' ');
                        int f1 = (int)Convert.ToDouble(sVal[0].Replace(".", ","))-1;
                        int f2 = (int)Convert.ToDouble(sVal[1].Replace(".", ","))-1;
                        int f3 = (int)Convert.ToDouble(sVal[2].Replace(".", ","))-1;

                        _faces.Add(new Vector3(f1, f2, f3));
                    }
                }
            }

            file.Close();

            _nbVertices = GetNbVertices();
            _nbFaces = GetNbFaces();
        }

        private int GetNbVertices()
        {
            return _vertices.Count();
        }

        private int GetNbFaces()
        {
            return _faces.Count();
        }


        public List<Vector3d> GetVertices()
        {
            return _vertices;
        }

        public int NbVertices()
        {
            return _vertices.Count();
        }

    }
}

