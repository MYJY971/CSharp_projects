using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//OpenTK
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TexLib;//Bibliothèque pour texture OpenTK
using MY_Mesh;//Classe créée pour charger des .obj
using MY_Shape;

namespace SensorApp
{
    public partial class SensorForm : Form
    {
        List<Mesh> _listMeshes;
        List<Shape> _listShapes;
        List<int> _idTextures;

        private bool _isTextured;

        Vector3 _eye = new Vector3(0.0f, -15.0f, 4.0f);
        Vector3 _target = Vector3.Zero;
        Vector3 _up = Vector3.UnitZ;

        Vector3 _eye0, _target0, _up0;

        Matrix4 _sensorMatrix;

        private Matrix4 _RotationX;
        private Matrix4 _RotationZ;

        public SensorForm()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            _isTextured = true;
            initIdTexture();
            initShape();

            TexUtil.InitTexturing();
            InitGLContext();
            SetupViewport();

            Run();
        }



        private void Run()
        {
            Application.Idle += Application_Idle;
        }

        /// <summary>
        /// Boucle de rendu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
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
        /// viewport 
        /// </summary>
        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;

            if (h == 0)                                                  // Prevent A Divide By Zero...
                h = 1;                                                   // By Making Height Equal To One


            GL.Viewport(0, 0, w, h);              // Use all of the glControl painting area
            GL.MatrixMode(MatrixMode.Projection);                               // Select The Projection Matrix
            GL.LoadIdentity();                                                  // Reset The Projection Matrix

            GL.Ortho(0, w, 0, h, 0, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
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

            DrawScene(_isTextured);

            glControl1.SwapBuffers();
        }

        private void initIdTexture()
        {
            _idTextures = new List<int>();
            
            int sky = TexUtil.CreateTextureFromFile("DATA\\sky10.jpg");
            _idTextures.Add(sky);

            int sand = TexUtil.CreateTextureFromFile("Data\\sand_texture.jpg");
            _idTextures.Add(sand);

            int obj1 = TexUtil.CreateTextureFromFile("DATA\\garnet_texture.jpg");
            _idTextures.Add(obj1);

            int sky2 = TexUtil.CreateTextureFromFile("DATA\\sky_photo.jpg");
            _idTextures.Add(sky2);



        }

        private void initShape()
        {
            _listShapes = new List<Shape>();

            Quad ground = new Quad(100);
            ground.GiveColor(Color.Beige);
            ground.GiveTexture(_idTextures[1]);
            ground.ScaleUV(10);

            _listShapes.Add(ground);
        }


        #region Draw Methods

        private void DrawBackground(int w, int h)
        {
            GL.ClearColor(Color.SkyBlue);
        }

        /// <summary>
        /// trihèdre
        /// </summary>
        private void DrawTrihedral()
        {
            //DrawTrihedral ///////////////////////
            GL.Translate(0.0f, 0.0f, 0.001f);

            float[] l_couleur = new float[4];
            float l_shin;
            float l_lenghAxis = 1f;
            float l_flecheW = 0.1f; float l_flecheH = 0.05f;

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

            

            GL.Translate(0.0f, 0.0f, -0.001f);

            GL.Color3(1.0f, 1.0f, 1.0f);
        }

        private void afficherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = true;
        }

        private void masquerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
        }

        private void radioButtonTextured_CheckedChanged(object sender, EventArgs e)
        {
            _isTextured = true;
        }

        private void radioButtonNoTextured_CheckedChanged(object sender, EventArgs e)
        {
            _isTextured = false;
        }

        private void DrawTrihedralSimple()
        {
            //DrawTrihedral ///////////////////////
            GL.Translate(0.0f, 0.0f, 0.001f);

            float[] l_couleur = new float[4];
            float l_shin;
            float l_lenghAxis = 1f;
            float l_flecheW = 0.1f; float l_flecheH = 0.05f;

            // axe X
            GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
            //l_couleur[0] = 1.0f; l_couleur[1] = 0.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, @l_couleur);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, @l_couleur);
            //l_shin = 128;
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, l_shin);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, @l_couleur);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, @l_couleur);

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
            //GL.Vertex4(0.0f, 1.0f, 0.0f, 1.0f);
            //l_couleur[0] = 0.0f; l_couleur[1] = 1.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, @l_couleur);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, @l_couleur);
            //l_shin = 128;
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, l_shin);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, @l_couleur);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, @l_couleur);
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
            //GL.Color4(0.0f, 0.0f, 1.0f, 1.0f);
            //l_couleur[0] = 0.0f; l_couleur[1] = 0.0f; l_couleur[2] = 1.0f; l_couleur[3] = 1.0f;
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, @l_couleur);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, @l_couleur);
            //l_shin = 128;
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, l_shin);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, @l_couleur);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, @l_couleur);
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



            GL.Translate(0.0f, 0.0f, -0.001f);

            GL.Color3(1.0f, 1.0f, 1.0f);
        }


        private void DrawScene(bool textured)
        {
            #region ModelView

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4 lookat = Matrix4.LookAt(_eye, _target, _up);

            GL.LoadMatrix(ref lookat);

            #endregion


            DrawTrihedralSimple();
        

            _listShapes[0].Draw(textured);

            

        }




        #endregion


    }
}
