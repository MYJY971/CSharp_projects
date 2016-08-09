using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging;

//OpenTK
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//Emgu
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

//DllImport
using System.Runtime.InteropServices;
using TexLib;
using System.IO;

namespace MY_Aruco_final
{
    public partial class AruForm : Form
    {
        #region Attributs Emgu

        /// <summary>
        /// Camera emgu
        /// </summary>
        private Capture _cameraCapture;
        #endregion



        /// <summary>
        /// Image récupéré par la caméra
        /// </summary>
        Mat _frame;
        /// <summary>
        /// Booléen indiquant si la caméra est allumé
        /// </summary>
        private bool _cameraOn = false;


        private bool _initContextGLisOk;

        private bool _sizeChanged;
        private float _factorSize = 1f;

        /// <summary>
        /// fichier contenant les paramètre de calibration de la caméra
        /// </summary>
        private string _pathCamPara;

        /// <summary>
        /// nombre de marqueurs détécté
        /// </summary>
        int _nbMarker = 0;

        double[] _modelViewMatrix;

        /// <summary>
        /// taille du marqueur en mètre
        /// </summary>
        float _markerSize;

        private int _objectTextureId;

        private Mat _backgroundImage;
        private Mat _frameComputed, _frameResized;

        /// <summary>
        /// Booléen idiquant si l'on est en plein écran
        /// </summary>
        bool _isFullSize;

        //private bool _ARactived = true;

        /// <summary>
        ///  érosion
        /// </summary>
        private int _tresh1;
        /// <summary>
        ///  dilatation
        /// </summary>
        private int _tresh2;


        /// <summary>
        /// Fonction de détéction de marqueur aruco, renvoie le nombre de marqueurs détécté ainsi que la matrice de projection et de modelview associé au marqueur
        /// </summary>
        /// <param name="image">Image où sera éffectué le traitement</param>
        /// <param name="path_CamPara">Paramètre de calibration de la caméra</param>
        /// <param name="imageWidth">Largeur de l'image en pixel</param>
        /// <param name="imageHeight">Longueur de l"image en pixel</param>
        /// <param name="glWidth">Largeur de la fenêtre OpenGL</param>
        /// <param name="glHeight">Longueur de la fenêtre OpenGL</param>
        /// <param name="gnear"></param>
        /// <param name="gfar"></param>
        /// <param name="proj_matrix">matrice de projection OpenGL</param>
        /// <param name="modelview_matrix">matrice modelview OpenGL</param>
        /// <param name="markerSize"> taille du marqueur en mètre</param>
        /// <param name="nbDetectedMarkers">Nombre de marqueurs détécté</param>
        /// <param name="treshParam1">Paramètre de seuillage : érosion </param>
        /// <param name="treshParam2">Paramètre de seuillage : dilatation </param>
        //[DllImport("ArucoDll2013.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        [DllImport("C:\\Stage\\Yanis\\CSharp_projects\\Aruco_final\\x64\\Debug\\MYAruco.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //[DllImport("MYAruco.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PerformARMarker(byte[] image, string path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
        double gnear, double gfar, double[] proj_matrix, double[] modelview_matrix, float markerSize, out int nbDetectedMarkers, int treshParam1, int treshParam2);

        Mesh _mesh;

        public AruForm()
        {
            InitializeComponent();
            try
            {
                //initialisation des variables

                _pathCamPara = "DATA\\intrinsics.yml";

                _markerSize = /**/1.09f;/*0.2f;/**/

                _isFullSize = false;

                _tresh1 = 12;
                _tresh2 = 13;


                _modelViewMatrix = new double[16];

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            //initialisation des variables propres a OpenGL

            //Objet .OBJ
            _mesh = new Mesh();
            //sphere troué
            //_mesh.Load("DATA\\sphere.obj");
            //torus
            _mesh.Load("DATA\\torus.obj");
            //coque piscine
            //_mesh.Load("DATA\\Caraïbes.obj");

            //texture
            _objectTextureId = TexUtil.CreateTextureFromFile("DATA\\texture.png");

            Run();
            SetupViewport();
        }


        private void Run()
        {
            try
            {
                //Allume la caméra 
                _cameraCapture = new Capture();

                /* La résolution par défaut prise en compte par Emgu est de 640x480
                 * Ici on fixe une résoltion de base, parmis les différentes résolution que peut prendre la caméra,
                 * elle prendra celle qui se rapproche le plus de la taille fixé.
                 * 
                 * (5120 x 2160) correspond à une résolution 4K
                 *                  
                 */
                _cameraCapture.SetCaptureProperty(CapProp.FrameWidth, 1280/*5120*/);
                _cameraCapture.SetCaptureProperty(CapProp.FrameHeight, 720 /*2160*/);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "Aucune caméra détéctée");
                return;
            }

            Application.Idle += ProcessFrame;
        }

        /// <summary>
        /// Capture en continue l'image filmé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessFrame(object sender, EventArgs e)
        {

            while (glControl1.IsIdle)
            {
                //récuperation de l'image courante filmé par la caméra
                _frame = _cameraCapture.QueryFrame();

                //indique que la caméra est allumé
                if (!_cameraOn)
                    _cameraOn = true;

                _backgroundImage = _frame;

                _frameComputed = _frame;
                _frameResized = _frame;

                //Redimensionne l'image 
                CvInvoke.Resize(_frameResized, _frameComputed, new Size(640, 480));

                Render();
            }


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

        protected void InitGLContext()
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
            //GL.Enable(EnableCap.CullFace);

            _initContextGLisOk = true;

        }

        /// <summary>
        /// Fonction de rendu
        /// </summary>
        private void Render()
        {
            if (!_initContextGLisOk)
                InitGLContext();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_cameraOn) 
            {
                #region Affiche en plain écran ou taille réelle
                if (!_isFullSize)
                {
                    if (!_sizeChanged)
                    {

                        AdaptSize(_frame.Size, out _factorSize);
                        _sizeChanged = true;

                    }

                }
                else
                {
                    glControl1.Size = panelImage.Size;
                    glControl1.SetBounds(0, 0, glControl1.Width, glControl1.Height);
                }
                #endregion

                //affiche la résolution de l'image
                label2.Text = "" + _frame.Width + " x " + _frame.Height;

                int w = glControl1.Width;
                int h = glControl1.Height;


                //Set Projection Ortho
                GL.MatrixMode(MatrixMode.Projection);                               // Select The Projection Matrix
                GL.LoadIdentity();                                                  // Reset The Projection Matrix

                GL.Ortho(0, w, 0, h, -1.0, 1.0);
                GL.Viewport(0, 0, w, h);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();



                GL.Disable(EnableCap.Texture2D);


                Image<Bgr, byte> _imageForARCompute = new Image<Bgr, byte>(_frameComputed.Width, _frameComputed.Height);
                CvInvoke.Resize(_frameComputed, _imageForARCompute, _frameComputed.Size);
                byte[] byteImageForARCompute = _imageForARCompute.Bytes;

                double[] projMatrix = new double[16];
                double[] modelviewMatrix = new double[16];

                //fonction de détection des marqueurs
                //if (_ARactived)
                PerformARMarker(byteImageForARCompute, _pathCamPara, _frameComputed.Width, _frameComputed.Height, glControl1.Width, glControl1.Height, 0.1, 100, projMatrix, modelviewMatrix, _markerSize, out _nbMarker, _tresh1, _tresh2);

                GL.RasterPos3(0f, h - 0.5f, -1.0f);

                if (!_isFullSize)
                {
                    GL.PixelZoom(1.0f * _factorSize, -1.0f * _factorSize);
                    _backgroundImage = _frame;
                    GL.DrawPixels(_backgroundImage.Width, _backgroundImage.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, _backgroundImage.DataPointer);

                }
                else
                {

                    GL.PixelZoom(1.0f, -1.0f);
                    CvInvoke.Resize(_frameResized, _backgroundImage, glControl1.Size);
                    GL.DrawPixels(_backgroundImage.Width, _backgroundImage.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, _backgroundImage.DataPointer);
                    _sizeChanged = false;
                }


                //Scene 3D

                label1.Text = "nb:" + _nbMarker;

                GL.MatrixMode(MatrixMode.Projection);

                GL.LoadIdentity();
                GL.MultMatrix(projMatrix);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                ////Conserve la dernière position si aucun marker détécté
                //if (_nbMarker != 0)
                //{
                _modelViewMatrix = modelviewMatrix;
                //}
                GL.LoadMatrix(_modelViewMatrix);
                DrawScene();

                GL.PushMatrix();

                GL.PopMatrix();

            }




            glControl1.SwapBuffers();
            //Refresh();
        }

        #region Draw Methodes

        private void DrawScene()
        {

            GL.Translate(0, 0, _markerSize + 0.0001f);
            DrawTrihedral(_markerSize / 2);
            GL.Translate(0, 0, -(_markerSize + 0.0001f));

            GL.Enable(EnableCap.Texture2D);
            DrawCube(_markerSize);
            GL.Disable(EnableCap.Texture2D);
            //int indiceat = 0;

            GL.Translate(0, 0, _markerSize / 2);
            GL.Color3(1.0f, 0.0f, 0.0f);
            _mesh.Draw();
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Translate(0, 0, -_markerSize / 2);
            GL.End();


        }

        /// <summary>
        /// Cube 3D
        /// </summary>
        private void DrawCube()
        {

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new Color4(0.3f, 0.3f, 0.3f, 1.0f));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 128f);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new Color4(0.0f, 0.0f, 0.0f, 1.0f)/*@l_couleur*/);

            GL.Begin(BeginMode.Quads);
            // Front Face
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);

            // Back Face
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Normal3(0.0f, 0.0f, -1.0f);
            GL.Vertex3(-0.5, -0.5, -0.5);
            GL.Vertex3(-0.5, 0.5, -0.5);
            GL.Vertex3(0.5, 0.5, -0.5);
            GL.Vertex3(0.5, -0.5, -0.5);

            // Top Face
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(-0.5, 0.5, -0.5);
            GL.Vertex3(-0.5, 0.5, 0.5);
            GL.Vertex3(0.5, 0.5, 0.5);
            GL.Vertex3(0.5, 0.5, -0.5);

            // Bottom Face
            GL.Color3(1f, 0.4f, 0f);
            GL.Normal3(0.0f, -1.0f, 0.0f);
            GL.Vertex3(-0.5, -0.5, -0.5);
            GL.Vertex3(0.5, -0.5, -0.5);
            GL.Vertex3(0.5, -0.5, 0.5);
            GL.Vertex3(-0.5, -0.5, 0.5);

            // Right face
            //GL.Color3(1f, 0f, 1f);
            GL.Normal3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(0.5, -0.5, -0.5);
            GL.Vertex3(0.5, 0.5, -0.5);
            GL.Vertex3(0.5, 0.5, 0.5);
            GL.Vertex3(0.5, -0.5, 0.5);

            // Left Face
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Normal3(-1.0f, 0.0f, 0.0f);
            GL.Vertex3(-0.5, -0.5, -0.5);
            GL.Vertex3(-0.5, -0.5, 0.5);
            GL.Vertex3(-0.5, 0.5, 0.5);
            GL.Vertex3(-0.5, 0.5, -0.5);

            GL.Color3(1.0f, 1.0f, 1.0f);

            GL.End();
        }

        /// <summary>
        /// Trihèdre 
        /// </summary>
        /// <param name="size"></param>
        private void DrawTrihedral(float size)
        {
            //DrawTrihedral ///////////////////////

            GL.LineWidth(2);
            float[] l_couleur = new float[4];
            float l_shin;
            float l_lenghAxis = size;
            float l_flecheW = size / 5; float l_flecheH = size / 5;

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
            GL.Vertex3(0.0f, 0.0f, 0.0f);
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
            GL.Vertex3(0.0f, l_lenghAxis, 0.0f);
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

        /// <summary>
        /// Cube 3D avec taille spécifié
        /// </summary>
        /// <param name="size"></param>
        private void DrawCube(float size)
        {
            float[] l_couleur = new float[4];
            float l_shin;

            // axe X
            //GL.Color4(1.0f, 0.0f, 0.0f, 0.5f);
            //l_couleur[0] = 1.0f; l_couleur[1] = 0.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            ///*
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new Color4(0.3f, 0.3f, 0.3f, 1.0f));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 128f);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new Color4(0.0f, 0.0f, 0.0f, 1.0f)/*@l_couleur*/);

            GL.Begin(BeginMode.Quads);
            //GL.Color3(1.0f, 0.0f, 0.0f);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _objectTextureId);
            // Front Face
            //GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.TexCoord2(0, 0); GL.Vertex3(-size / 2, -size / 2, 0);
            GL.TexCoord2(1, 0); GL.Vertex3(-size / 2, -size / 2, size);
            GL.TexCoord2(1, 1); GL.Vertex3(size / 2, -size / 2, size);
            GL.TexCoord2(0, 1); GL.Vertex3(size / 2, -size / 2, 0.0);

            // Back Face
            //GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Normal3(0.0f, 0.0f, -1.0f);
            GL.TexCoord2(0, 1); GL.Vertex3(-size / 2, size / 2, 0);
            GL.TexCoord2(1, 1); GL.Vertex3(-size / 2, size / 2, size);
            GL.TexCoord2(1, 0); GL.Vertex3(size / 2, size / 2, size);
            GL.TexCoord2(0, 0); GL.Vertex3(size / 2, size / 2, 0.0);

            // Top Face
            //GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.TexCoord2(0, 1); GL.Vertex3(-size / 2, -size / 2, size);
            GL.TexCoord2(0, 0); GL.Vertex3(-size / 2, size / 2, size);
            GL.TexCoord2(1, 0); GL.Vertex3(size / 2, size / 2, size);
            GL.TexCoord2(1, 1); GL.Vertex3(size / 2, -size / 2, size);

            // Bottom Face
            //GL.Color3(1f, 0.4f, 0f);
            GL.Normal3(0.0f, -1.0f, 0.0f);
            GL.TexCoord2(1, 1); GL.Vertex3(-size / 2, -size / 2, 0);
            GL.TexCoord2(0, 1); GL.Vertex3(-size / 2, size / 2, 0);
            GL.TexCoord2(0, 0); GL.Vertex3(size / 2, size / 2, 0);
            GL.TexCoord2(1, 0); GL.Vertex3(size / 2, -size / 2, 0);


            // Right face
            //GL.Color3(1f, 0f, 1f);
            GL.Normal3(1.0f, 0.0f, 0.0f);
            GL.TexCoord2(1, 0); GL.Vertex3(size / 2, -size / 2, 0);
            GL.TexCoord2(1, 1); GL.Vertex3(size / 2, -size / 2, size);
            GL.TexCoord2(0, 1); GL.Vertex3(size / 2, size / 2, size);
            GL.TexCoord2(0, 0); GL.Vertex3(size / 2, -size / 2, size);


            // Left Face
            //GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Normal3(-1.0f, 0.0f, 0.0f);
            GL.TexCoord2(0, 0); GL.Vertex3(-size / 2, -size / 2, 0);
            GL.TexCoord2(1, 0); GL.Vertex3(-size / 2, -size / 2, size);
            GL.TexCoord2(1, 1); GL.Vertex3(-size / 2, size / 2, size);
            GL.TexCoord2(0, 1); GL.Vertex3(-size / 2, -size / 2, size);

            GL.Disable(EnableCap.Texture2D);
            GL.Color3(1.0f, 1.0f, 1.0f);

            GL.End();
        }


        private void Draw3DScene()
        {
            //GL.Disable(EnableCap.Texture2D);
            //GL.Enable(EnableCap.DepthTest);
            _mesh.Draw();

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



        #endregion


        #region Listeners
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void buttonTresh1P_Click(object sender, EventArgs e)
        {
            _tresh1++;
        }

        private void buttonTresh1M_Click(object sender, EventArgs e)
        {
            _tresh1--;
        }

        private void buttonTresh2P_Click(object sender, EventArgs e)
        {
            _tresh2++;

        }

        private void buttonTresh2M_Click(object sender, EventArgs e)
        {
            _tresh2--;
        }

        private void buttonFullSize_Click(object sender, EventArgs e)
        {

            _isFullSize = true;

        }

        private void buttonAdaptedSize_Click(object sender, EventArgs e)
        {

            _isFullSize = false;

        }
        #endregion





        /// <summary>
        /// Adapte la taille du viewer OpenGL en fonction de la taille de la fenêtre et de l'image filmée
        /// </summary>
        /// <param name="frameSize"> taille de l'image filmée </param>
        /// <param name="factor"> facteur de reduction si la taille de l'image dépasse celle de la fenêtre, sinon est égale à 1 </param>
        private void AdaptSize(Size frameSize, out float factor)
        {

            float diffW, diffH;
            Size panSize = panelImage.Size;

            diffW = frameSize.Width - panSize.Width;
            diffH = frameSize.Height - panSize.Height;

            if (diffW > 0 || diffH > 0)
            {
                if (frameSize.Width <= frameSize.Height)
                {
                    factor = (float)panelImage.Size.Width / (float)frameSize.Width;
                }
                else
                {
                    factor = (float)panelImage.Size.Height / (float)frameSize.Height;
                }
            }
            else
            {
                factor = 1.0f;
            }

            Size sizeGL = new Size((int)(frameSize.Width * factor), (int)(frameSize.Height * factor));


            CenterImage(sizeGL);

        }

        //Centre L'image 
        private void CenterImage(Size size)
        {
            float diffW, diffH;
            int x, y;
            Size sizePan = panelImage.Size;

            diffW = sizePan.Width - size.Width;
            diffH = sizePan.Height - size.Height;

            x = 0;
            y = 0;

            if (diffW > 0)
            {
                x = (int)diffW / 2;
            }
            if (diffH > 0)
            {
                y = (int)diffH / 2;
            }

            glControl1.SetBounds(x, y, size.Width, size.Height);
        }
    }
}
