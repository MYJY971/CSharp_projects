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


//sensor
using Windows.Devices.Sensors;

namespace Sensor
{
    public partial class Form1 : Form
    {
        List<int> _idTextures;

        Mesh _obj1; 

        double[] _modelViewMatrix;

        Vector3 _eye= new Vector3(-5.0f, 0.0f, 0.0f);
        Vector3 _target = Vector3.Zero;
        Vector3 _up = Vector3.UnitZ;

        //sensor
        private OrientationSensor _orientationSensor;

        public Form1()
        {
            InitializeComponent();
            

        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            initMeshes();
            initModelViewMatrix();
            initTextures();
            TexUtil.InitTexturing();
            InitGLContext();
            SetupViewport();

            Run();
        }

        private void Run()
        {
            _orientationSensor = OrientationSensor.GetDefault();
            if (_orientationSensor != null)
            {
                
                _orientationSensor.ReadingChanged += _orientationSensor_ReadingChanged;
            }
            else
            {
                MessageBox.Show("Aucun capteur d'orientation détécté");
            }

            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            
        }

        private void _orientationSensor_ReadingChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            
        }

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

            DrawBackground(w, h);

            //Scene 3D

            #region Projection Perspective
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            Matrix4 projectionMat = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, w / h, 0.1f, 100.0f);

            GL.MultMatrix(ref projectionMat);

            #endregion

            DrawScene();




            glControl1.SwapBuffers();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void initTextures()
        {
            
           
            
            _idTextures = new List<int>();

            int sky = TexUtil.CreateTextureFromFile("DATA\\sky10.jpg");
            int sand = TexUtil.CreateTextureFromFile("Data\\sand_texture.jpg");
            int obj1 = TexUtil.CreateTextureFromFile("DATA\\garnet_texture.jpg");

            _idTextures.Add(sky);
            _idTextures.Add(sand);
            _idTextures.Add(obj1);
        }

        private void initMeshes()
        {
            _obj1 = new Mesh("DATA\\icosphere.obj");

        }

        private void initModelViewMatrix()
        {
            Matrix4 mv = Matrix4.LookAt(_eye, _target, _up);

            _modelViewMatrix = new double[16] { mv.M11, mv.M12, mv.M13, mv.M14,
                                                mv.M21, mv.M22, mv.M23, mv.M24,
                                                mv.M31, mv.M32, mv.M33, mv.M34,
                                                mv.M41, mv.M42, mv.M43, mv.M44};
        }

        #region DrawMethods

        private void DrawBackground(int w, int h)
        {
            //Projection Ortho


            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.DepthMask(false);

            GL.BindTexture(TextureTarget.Texture2D, _idTextures[0]);

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

        private void DrawScene()
        {
            //modelview
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix4 modelView = Matrix4.LookAt(_eye, _target, _up);
            GL.LoadMatrix(ref modelView /*_modelViewMatrix*/);

            //correction repère opentk
            GL.Rotate(-90, Vector3.UnitZ);
            GL.Translate(0.0f, 0.0f, -1.0f);


            DrawGround();
            DrawTrihedral();

            DrawObject1();
        }

        private void DrawGround()
        {
            //GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            //GL.DepthMask(false);

            GL.BindTexture(TextureTarget.Texture2D, _idTextures[1]);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3(-100.0f, -100.0f, 0.0f);
            GL.TexCoord2(0, 0); GL.Vertex3(-100.0f, 100.0f, 0.0f);
            GL.TexCoord2(1, 0); GL.Vertex3(100.0f, 100.0f, 0.0f);
            GL.TexCoord2(1, 1); GL.Vertex3(100.0f, -100.0f, 0.0f);
            GL.End();


            GL.Disable(EnableCap.Texture2D);
            //GL.Enable(EnableCap.DepthTest);
            //GL.DepthMask(true);
        }

        private void DrawTrihedral()
        {
            //DrawTrihedral ///////////////////////

            float[] l_couleur = new float[4];
            float l_shin;
            float l_lenghAxis = 1f;
            float l_flecheW = 0.01f; float l_flecheH = 0.005f;

            // axe X
            GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
            l_couleur[0] = 1.0f; l_couleur[1] = 0.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, @l_couleur);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, @l_couleur);
            l_shin = 128;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, l_shin);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, @l_couleur);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, @l_couleur);

            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(0.0f, 0.0f, 0.0001f);
            GL.Vertex3(l_lenghAxis, 0.0f, 0.0f);
            GL.End();

            // point axe X
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(l_lenghAxis - l_flecheW, l_flecheH, 0.0f);
            GL.Vertex3(l_lenghAxis, 0.0f, 0.0f);
            GL.Vertex3(l_lenghAxis - l_flecheW, -l_flecheH, 0.0f);
            GL.End();

            // axe Y
            GL.Vertex4(0.0f, 1.0f, 0.0f, 1.0f);
            l_couleur[0] = 0.0f; l_couleur[1] = 1.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, @l_couleur);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, @l_couleur);
            l_shin = 128;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, l_shin);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, @l_couleur);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, @l_couleur);
            GL.Color3(0.0f, 1.0f, 0.0f);

            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, l_lenghAxis, 0.0001f);
            GL.End();
            // pointe axe Y
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(0.0f - l_flecheH, l_lenghAxis - l_flecheW, 0.0f);
            GL.Vertex3(0.0f, l_lenghAxis, 0.0f);
            GL.Vertex3(0.0f + l_flecheH, l_lenghAxis - l_flecheW, 0.0f);
            GL.End();

            // axe Z
            GL.Color4(0.0f, 0.0f, 1.0f, 1.0f);
            l_couleur[0] = 0.0f; l_couleur[1] = 0.0f; l_couleur[2] = 1.0f; l_couleur[3] = 1.0f;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, @l_couleur);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, @l_couleur);
            l_shin = 128;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, l_shin);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, @l_couleur);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, @l_couleur);
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, l_lenghAxis);
            GL.End();
            // pointe axe Z
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(l_flecheH, 0.0f, l_lenghAxis - l_flecheW);
            GL.Vertex3(0.0f, 0.0f, l_lenghAxis);
            GL.Vertex3(-l_flecheH, 0.0f, l_lenghAxis - l_flecheW);
            GL.End();

            GL.Color3(1.0f, 1.0f, 1.0f);
        }

        private void DrawObject1()
        {
            
            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, _idTextures[2]);

            GL.Translate(0.0f, 10.0f, 1.0f);
            _obj1.Draw();
            GL.Translate(0.0f, -10.0f, -1.0f);

            GL.Disable(EnableCap.Texture2D);
        }

        #endregion
    }
}
