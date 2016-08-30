using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MY_Mesh
{
    class Mesh
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

        public Mesh()
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
        }

        public Mesh(String filename)
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

                if (line.StartsWith("v "))
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
                    //normale
                    if (line.StartsWith("vn "))
                    {
                        String sNormal = line.Substring(3);

                        String[] sValn = sNormal.Split(' ');
                        double n1 = Convert.ToDouble(sValn[0].Replace(".", ","));
                        double n2 = Convert.ToDouble(sValn[1].Replace(".", ","));
                        double n3 = Convert.ToDouble(sValn[2].Replace(".", ","));

                        _normales.Add(new Vector3d(n1, n2, n3));

                        if (!_haveNormale)
                            _haveNormale = true;
                    }

                    //texture
                    if (line.StartsWith("vt "))
                    {
                        String sTexture = line.Substring(3);

                        String[] sValt = sTexture.Split(' ');
                        double t1 = Convert.ToDouble(sValt[0].Replace(".", ","));
                        double t2 = Convert.ToDouble(sValt[1].Replace(".", ","));


                        _textures.Add(new Vector2d(t1, t2));

                        if (!_haveTexture)
                            _haveTexture = true;
                    }

                    //face
                    if (line.StartsWith("f"))
                    {
                        String sFace = line.Substring(2);


                        String[] sValf = sFace.Split(' ');
                        //v: indice vertex, n:indice normal, t:indice texture
                        //f v//n v//n v//n ou f v// v// v//
                        String[] param = new String[1] { "//" };

                        int[] idV = new int[3];
                        int[] idN = new int[3];
                        int[] idT = new int[3];

                        for (int i = 0; i < 3; i++)
                        {


                            String[] tmp1 = sValf[i].Split(param, StringSplitOptions.RemoveEmptyEntries);


                            if (tmp1.Count() > 1) //f v//n v//n v//n
                            {
                                idV[i] = (int)Convert.ToDouble(tmp1[0]) - 1;
                                idN[i] = (int)Convert.ToDouble(tmp1[1]) - 1;
                            }
                            else
                            {
                                String[] tmp2 = sValf[i].Split('/');

                                if (tmp2.Count() == 1) //f v v v
                                {
                                    idV[i] = (int)Convert.ToDouble(tmp1[0]) - 1;
                                }
                                else if (tmp2.Count() == 2) //f v/t v/t v/t
                                {
                                    idV[i] = (int)Convert.ToDouble(tmp2[0]) - 1;
                                    idT[i] = (int)Convert.ToDouble(tmp2[1]) - 1;
                                }
                                else //f v/t/n v/t/n v/t/n
                                {
                                    idV[i] = (int)Convert.ToDouble(tmp2[0]) - 1;
                                    idT[i] = (int)Convert.ToDouble(tmp2[1]) - 1;
                                    idN[i] = (int)Convert.ToDouble(tmp2[2]) - 1;
                                }


                            }


                        }

                        _faces.Add(new Vector3(idV[0], idV[1], idV[2]));
                        _normalID.Add(new Vector3(idN[0], idN[1], idN[2]));
                        _textureID.Add(new Vector3(idT[0], idT[1], idT[2]));

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

        public void Draw()
        {

            GL.Begin(BeginMode.Triangles);

            //foreach(Vector3 face in _mesh._faces)
            for (int i = 0; i < _faces.Count(); i++)
            {
                //if (_haveNormale)
                //{
                //    GL.Color3((Vector3)_normales[(int)_normalID[i].X]);
                //}

                //else
                //    GL.Color3((Vector3)_vertices[(int)_faces[i].X]);

                if (_haveNormale)
                    GL.Normal3(_normales[(int)_normalID[i].X]);
                if (_haveTexture)
                    GL.TexCoord2(_textures[(int)_textureID[i].X]);
                GL.Vertex3(_vertices[(int)_faces[i].X]);

                if (_haveNormale)
                    GL.Normal3(_normales[(int)_normalID[i].Y]);
                if (_haveTexture)
                    GL.TexCoord2(_textures[(int)_textureID[i].Y]);
                GL.Vertex3(_vertices[(int)_faces[i].Y]);

                if (_haveNormale)
                    GL.Normal3(_normales[(int)_normalID[i].Y]);
                if (_haveTexture)
                    GL.TexCoord2(_textures[(int)_textureID[i].Z]);
                GL.Vertex3(_vertices[(int)_faces[i].Z]);
            }

            GL.End();
        }

    }
}

