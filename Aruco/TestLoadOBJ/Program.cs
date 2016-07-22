using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace TestLoadOBJ
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader file;

            file = new StreamReader("sphere.obj");

            string line;

            while(/*!file.EndOfStream*/(line = file.ReadLine()) != null)
            {
                if(line.StartsWith("v"))
                { 

                }
                else
                    Console.WriteLine(line.First());
            }

            file.Close();
        }
    }
}
