using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


//OpenTk
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TexLib;
using OpenTK;
using MY_Mesh;
using MY_PC;
namespace ZedTest
{
    public partial class Form1 : Form
    {
        int _idTextBackground;
        Size _backgroundSize;
        Matrix4 _proj;
        
        Matrix4 _pose;

        MY_PointCloud _pc;
        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            SetProjectionMat();
            _pc = new MY_PointCloud("DATA\\PC_894.xyz");
            _idTextBackground = TexUtil.CreateTextureFromFile("DATA\\ZED_image.png", out _backgroundSize);
            glControl1.Width = _backgroundSize.Width/2;
            glControl1.Height = _backgroundSize.Height;
            TexUtil.InitTexturing();
            InitGLContext();
            SetupViewport();
            

            Application.Idle +=Application_Idle;
            //Run();
        }

        //private void Run()
        //{
        //    Application.Idle += Application_Idle;
        //}

        void Application_Idle(object sender, EventArgs e)
        {
            while(glControl1.IsIdle)
            {
                Render();
            }
        }

        /// <summary>
        /// Initialisation du contexte OpenGL
        /// </summary>
        private void InitGLContext()
        {
            GL.Enable(EnableCap.Texture2D);                       // Enable Texture Mapping
            GL.ShadeModel(ShadingModel.Smooth);                   // Enable Smooth Shading
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);                           // Clear the Color

            // Clear the Color and Depth Buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.ClearDepth(1.0f);										     // Depth Buffer Setup
            GL.Enable(EnableCap.DepthTest);								     // Enables Depth Testing
            GL.DepthFunc(DepthFunction.Lequal);							     // The Type Of Depth Testing To Do
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);  // Really Nice Perspective Calculations


            // Lumiere

            float[] lightPos = { 10.0f, 10.0f, 100.0f, 0.1f };
            GL.Light(LightName.Light0, LightParameter.Position, lightPos);
            GL.Light(LightName.Light0, LightParameter.SpotDirection, new float[] { 0, 0, -1 });

            GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(0.6f, 0.6f, 0.6f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Specular, new Color4(0.6f, 0.6f, 0.6f, 1.0f));

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);

        }

        /// <summary>
        /// Viewport OpenGL
        /// </summary>
        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;

            if (h == 0)                                                  // Prevent A Divide By Zero...
                h = 1;                                                   // By Making Height Equal To One

            //_defaultProjection = Matrix4.CreatePerspectiveFieldOfView((float)System.Math.PI / 4, (float)w / (float)h, 0.1f, 100.0f);


            GL.Viewport(0, 0, w, h);              // Use all of the glControl painting area
            GL.MatrixMode(MatrixMode.Projection);                               // Select The Projection Matrix
            GL.LoadIdentity();                                                  // Reset The Projection Matrix

            GL.Ortho(0, w, 0, h, 0, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        /// <summary>
        /// Rendu OpenGL
        /// </summary>
        private void Render()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Fond

            #region Projection Ortho
            GL.MatrixMode(MatrixMode.Projection);                               // Select The Projection Matrix
            GL.LoadIdentity();                                                  // Reset The Projection Matrix

            GL.Ortho(0, w, 0, h, -1.0, 1.0);
            GL.Viewport(0, 0, w, h);
            #endregion
            DrawBackground();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref _proj);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            DrawPointCloud();

            GL.PointSize(10);
            GL.Begin(BeginMode.Points);

            
            GL.Color3(1.0f, 0.0f, 0.0f);

            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);

            GL.Color3(1.0f, 1.0f, 1.0f);


            GL.End();
            //GL.PointSize(1);

            glControl1.SwapBuffers();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        #region Draw Method

        private void DrawBackground()
        {
            int w = _backgroundSize.Width;
            int h = _backgroundSize.Height;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.DepthMask(false);

            GL.BindTexture(TextureTarget.Texture2D, _idTextBackground);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1); GL.Vertex2(0, 0);
            GL.TexCoord2(0, 0); GL.Vertex2(0, h);
            GL.TexCoord2(1, 0); GL.Vertex2(w, h);
            GL.TexCoord2(1, 1); GL.Vertex2(w, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }

        private void DrawPointCloud()
        {

            _pc.Draw();
        }

        #endregion

        private void SetProjectionMat()
        {
            _proj = new Matrix4(1.73205f, 0, 0, 0,
                                0, 1.73205f, 0, 0,
                                0, 0, -1.0002f, -0.020002f,
                                0, 0, -1, 0);
            _proj.Invert();
        }

        private void SetViewMat()
        {
            _pose = new Matrix4(1.f, -)
        }

    }
}
