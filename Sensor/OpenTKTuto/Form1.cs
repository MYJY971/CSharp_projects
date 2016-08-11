using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.IO;

namespace OpenTKTuto
{
    public partial class Form1 : Form
    {
        bool _loaded = false;
        private bool _initContextGLisOk;

        //glsl
        int _pgmID;
        int _vsID;
        int _fsID;

        int _attribute_vcol;
        int _attribute_vpos;
        int _uniform_mview;

        int _vbo_position;
        int _vbo_color;
        int _vbo_mview;

        Vector3[] _vertdata;
        Vector3[] _coldata;
        Matrix4[] _mviewdata;

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            _loaded = true;

            initProgram();

            _vertdata = new Vector3[]   { new Vector3(-0.8f, -0.8f, 0f),
                                          new Vector3( 0.8f, -0.8f, 0f),
                                          new Vector3( 0f,  0.8f, 0f)
                                        };

            _coldata = new Vector3[]   { new Vector3(1f, 0f, 0f),
                                          new Vector3( 0f, 1f, 0f),
                                          new Vector3( 0f,  0f, 1f)
                                        };

            _mviewdata = new Matrix4[] { Matrix4.Identity };
            
            

            GL.ClearColor(Color.CornflowerBlue);
            SetupViewport();

            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while(glControl1.IsIdle)
            {
                //OnUpdate();
                RenderShader();
            }
        }

        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;

            if (h == 0)                                                  // Prevent A Divide By Zero...
                h = 1;                                                   // By Making Height Equal To One

            GL.Viewport(0, 0, w, h);              // Use all of the glControl painting area

            GL.MatrixMode(MatrixMode.Projection);                               // Select The Projection Matrix
            GL.LoadIdentity();                                                  // Reset The Projection Matrix

            GL.Ortho(0, w, 0, h, -1, 1);
            //GL.Viewport(0, 0, w, h);

        }



        private void glControl1_Resize(object sender, EventArgs e)
        {
            base.OnSizeChanged(e);
            SetupViewport();
        }

        private void InitGLContext()
        {
            GL.Enable(EnableCap.Texture2D);                       // Enable Texture Mapping
            GL.ShadeModel(ShadingModel.Smooth);                   // Enable Smooth Shading
            GL.ClearColor(Color.SkyBlue);                           // Clear the Color

            // Clear the Color and Depth Buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.ClearDepth(1.0f);                                             // Depth Buffer Setup
            GL.Enable(EnableCap.DepthTest);                                  // Enables Depth Testing
            GL.DepthFunc(DepthFunction.Lequal);                              // The Type Of Depth Testing To Do
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);  // Really Nice Perspective Calculations


            // Lumiere

            float[] lightPos = { 10.0f, 10.0f, 100.0f, 1.0f };
            GL.Light(LightName.Light0, LightParameter.Position, lightPos);
            GL.Light(LightName.Light0, LightParameter.SpotDirection, new float[] { 0, 0, -1 });

            GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(0.6f, 0.6f, 0.6f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Specular, new Color4(0.6f, 0.6f, 0.6f, 1.0f));

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            //GL.Enable(EnableCap.CullFace);

            _initContextGLisOk = true;
        }

        private void initProgram()
        {
            _pgmID = GL.CreateProgram();

            loadShader("vs.glsl", ShaderType.VertexShader, _pgmID, out _vsID);
            loadShader("fs.glsl", ShaderType.FragmentShader, _pgmID, out _fsID);

            GL.LinkProgram(_pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(_pgmID));

            _attribute_vpos = GL.GetAttribLocation(_pgmID, "vPosition");
            _attribute_vcol = GL.GetAttribLocation(_pgmID, "vColor");
            _uniform_mview = GL.GetUniformLocation(_pgmID, "modelview");

            if (_attribute_vpos == -1 || _attribute_vcol == -1 || _uniform_mview == -1)
            {
                Console.WriteLine("Error binding attributes");
            }

            GL.GenBuffers(1, out _vbo_position);
            GL.GenBuffers(1, out _vbo_color);
            GL.GenBuffers(1, out _vbo_mview);
            
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Projection
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            //ModelView
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            GL.End();
            DrawScene();


            glControl1.SwapBuffers();
        }

        //private void OnUpdate()
        //{
        //    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_position);
        //    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(_vertdata.Length * Vector3.SizeInBytes), _vertdata, BufferUsageHint.StaticDraw);
        //    GL.VertexAttribPointer(_attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

        //    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_color);
        //    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(_coldata.Length * Vector3.SizeInBytes), _coldata, BufferUsageHint.StaticDraw);
        //    GL.VertexAttribPointer(_attribute_vcol, 3, VertexAttribPointerType.Float, true, 0, 0);

        //    GL.UniformMatrix4(_uniform_mview, false, ref _mviewdata[0]);

        //    GL.UseProgram(_pgmID);

        //    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        //}

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {

            RenderShader();
        }

        private void RenderShader()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(_vertdata.Length * Vector3.SizeInBytes), _vertdata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(_attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_color);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(_coldata.Length * Vector3.SizeInBytes), _coldata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(_attribute_vcol, 3, VertexAttribPointerType.Float, true, 0, 0);

            GL.UniformMatrix4(_uniform_mview, false, ref _mviewdata[0]);

            GL.UseProgram(_pgmID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);



            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Projection
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            //ModelView
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            GL.End();
            DrawSceneShader();


            glControl1.SwapBuffers();
        }

        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }


        #region Draw Methods
        private void DrawScene()
        {
            DrawTriangle1();
        }

        private void DrawSceneShader()
        {
            DrawTriangle2();
        }

        private void DrawTriangle1()
        {
            GL.Begin(BeginMode.Triangles);

            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(-1.0f, -1.0f, 4.0f);

            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(1.0f, -1.0f, 4.0f);

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 4.0f);

            GL.End();
        }

        private void DrawTriangle2()
         {

            GL.EnableVertexAttribArray(_attribute_vpos);
            GL.EnableVertexAttribArray(_attribute_vcol);

            GL.DrawArrays(BeginMode.Triangles, 0, 3);

            GL.DisableVertexAttribArray(_attribute_vpos);
            GL.DisableVertexAttribArray(_attribute_vcol);

            GL.Flush();
        }
        #endregion
    }
}
