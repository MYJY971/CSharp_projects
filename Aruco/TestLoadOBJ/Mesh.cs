using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;

namespace MY_Aruco
{
    class Mesh
    {
        public List<Vector3d> _vertices;
        private List<Vector3d> _faces;

        public int _nbVertices;
        public int _nbFaces;

        public Mesh()
        {
            _vertices = new List<Vector3d>();
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
            }

            file.Close();

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

