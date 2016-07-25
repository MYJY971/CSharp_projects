using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using MY_Aruco;

namespace TestLoadOBJ
{
    class Program
    {

        
        static void Main(string[] args)
        {
            StreamReader file;
            List<Vector3d> vertices = new List<Vector3d>();

            file = new StreamReader("sphere.obj");

            Mesh mesh1 = new Mesh();
            mesh1.Load("sphere.obj");

            vertices = mesh1.GetVertices();


        }
    }
}
