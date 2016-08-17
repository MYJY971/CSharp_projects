using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
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

//Bibliothèque pour texture OpenTK
using TexLib;

//Sensor
using Windows.Devices.Sensors;

namespace MY_Aruco2013_v3
{
    public partial class AruForm : Form
    {
        #region Attributs Emgu

        /// <summary>
        /// Camera emgu
        /// </summary>
        private Capture _cameraCapture;


        /// <summary>
        /// Image récupéré par la caméra
        /// </summary>
        Mat _frame;


        private Mat _backgroundImage;// = new Mat();
        private Mat _frameComputed;// = new Mat();
        //private Mat _frameResized;

        #endregion


        /// <summary>
        /// Booléen indiquant si la caméra est allumé
        /// </summary>
        private bool _cameraOn = false;


        //private bool _initContextGLisOk;

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
        /// objet .obj
        /// </summary>
        Mesh _mesh;

        /// <summary>
        /// Capteur d'orientation
        /// </summary>
        private OrientationSensor _orientationSensor;

        /// <summary>
        /// Matrices de rotation utilisées pour corriger le repère de la tablette
        /// </summary>
        private Matrix4 _RotationX;
        private Matrix4 _RotationZ;

        /// <summary>
        /// Vecteurs caractéristiques de la matrice de vue (lookat)
        /// </summary>
        private Vector3 _eye, _target, _up, _target0, _up0;

        /// <summary>
        /// Matrice de rotation récupéré par le capteur
        /// </summary>
        private Matrix4 _sensorMatrix;

        /// <summary>
        /// Indique si un capteur d'orientation à été trouvé
        /// </summary>
        private bool _sensorFound;

        /// <summary>
        /// Indique si on se sert du capteur pour effectuer une rotation
        /// </summary>
        private bool _activateSensor;

        /// <summary>
        /// indique si le marqueur aruco à été trouvé au moins une fois
        /// </summary>
        private bool _arucoSuccess;

        /// <summary>
        /// target0 et up0 doivent être initialisé une seule fois à chaque fois que le marqueur n'est plus reconnu
        /// </summary>
        private bool inittargetUp0;

        /// <summary>
        /// Indique si l'on peu utiliser les vecteur eye, target et up pour générer un lookat
        /// </summary>
        private bool _lookatOK;

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
        [DllImport("ArucoDll2013.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PerformARMarker(byte[] image, string path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
        double gnear, double gfar, double[] proj_matrix, double[] modelview_matrix, float markerSize, out int nbDetectedMarkers, int treshParam1, int treshParam2);



        public AruForm()
        {
            InitializeComponent();
            try
            {
                //initialisation des variables
                _sensorFound = false;
                _activateSensor = false;
                _arucoSuccess = false;
                _lookatOK = false;

                _backgroundImage = new Mat();
                _frameComputed = new Mat();

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
            InitGLContext();
            SetupViewport();
        }


        private void Run()
        {
            try
            {
                //Allume la caméra 
                //test personnel, idx = 1 zedCamera

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


            _orientationSensor = OrientationSensor.GetDefault();
            if (_orientationSensor != null)
            {
                _sensorFound = true;
                _orientationSensor.ReadingChanged += _orientationSensor_ReadingChanged;
            }

            Application.Idle += ProcessFrame;
        }

        /// <summary>
        /// Récupère la matrice d'orientation obtenue par le capteur et l'adapte au repère OpenTK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void _orientationSensor_ReadingChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            double M11, M12, M13, M14,
                   M21, M22, M23, M24,
                   M31, M32, M33, M34,
                   M41, M42, M43, M44;

            M11 = args.Reading.RotationMatrix.M11;
            M12 = args.Reading.RotationMatrix.M12;
            M13 = args.Reading.RotationMatrix.M13;
            M14 = 0.0;

            M21 = args.Reading.RotationMatrix.M21;
            M22 = args.Reading.RotationMatrix.M22;
            M23 = args.Reading.RotationMatrix.M23;
            M24 = 0.0;

            M31 = args.Reading.RotationMatrix.M31;
            M32 = args.Reading.RotationMatrix.M32;
            M33 = args.Reading.RotationMatrix.M33;
            M34 = 0.0;

            M41 = 0.0;
            M42 = 0.0;
            M43 = 0.0;
            M44 = 1.0;

            _sensorMatrix = new Matrix4((float)M11, (float)M12, (float)M13, (float)M14,
                                     (float)M21, (float)M22, (float)M23, (float)M24,
                                     (float)M31, (float)M32, (float)M33, (float)M34,
                                     (float)M41, (float)M42, (float)M43, (float)M44);



            _RotationX = Matrix4.CreateRotationX((float)-Math.PI / 2);
            _RotationZ = Matrix4.CreateRotationZ((float)-Math.PI / 2);

            //Les Matrices doivent être inversé pour la multiplication matricielle car Opentk multiplie par colonne et non lineairement
            _RotationX.Invert();
            _RotationZ.Invert();

            //le repère de la surface n'est pas le même que OpenTK,
            //on fait corespondre les deux repère par une rotation de -90° autour de X et Z
            _sensorMatrix = Matrix4.Mult(_sensorMatrix, _RotationX);
            _sensorMatrix = Matrix4.Mult(_sensorMatrix, _RotationZ);

            if (_activateSensor)
            {
                Vector3 tmp1, tmp2;

                _sensorMatrix.Invert();
                tmp1 = Vector3.Transform(_eye, _sensorMatrix);
                tmp2 = _target0 - tmp1;
                _target = tmp2 + _eye;

                _up = Vector3.Transform(_up0, _sensorMatrix);

                _lookatOK = true;
            }
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

                //_backgroundImage = _frame;

                //_frameComputed = _frame;
                //_frameResized = _frame;

                //Redimensionne l'image 
                CvInvoke.Resize(_frame, _frameComputed, new Size(640, 480));

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

            //_initContextGLisOk = true;

        }

        /// <summary>
        /// Fonction de rendu
        /// </summary>
        private void Render()
        {

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

                PerformARMarker(byteImageForARCompute, _pathCamPara, _frameComputed.Width, _frameComputed.Height, glControl1.Width, glControl1.Height, 0.1, 100, projMatrix, modelviewMatrix, _markerSize, out _nbMarker, _tresh1, _tresh2);

                if (!_arucoSuccess && _nbMarker > 0)
                {
                    _arucoSuccess = true;
                }


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
                    CvInvoke.Resize(_frame, _backgroundImage, glControl1.Size);
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
                //Conserve la dernière position si aucun marker détécté
                if (_nbMarker != 0)
                {
                    if (_sensorFound && _activateSensor)
                        _activateSensor = false;

                    _modelViewMatrix = modelviewMatrix;
                    ExtractEyeTargetUp(_modelViewMatrix, out _eye, out _target, out _up);
                }
                else
                {
                    if (_sensorFound && _arucoSuccess)
                    {
                        ExtractEyeTargetUp(_modelViewMatrix, out _eye, out _target, out _up);

                        if (!_activateSensor)
                        {
                            _activateSensor = true;


                            _target0 = _target;
                            _up0 = _up;

                        }

                        if (_lookatOK)
                        {
                            Matrix4 lookat = Matrix4.LookAt(_eye, _target, _up);
                            _modelViewMatrix = Matrix4ToDouble(lookat);
                        }
                    }
                    //else
                    //    _modelViewMatrix = modelviewMatrix;


                }

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

            GL.PointSize(10);
            GL.Begin(BeginMode.Points);
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(_target);
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);
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
        /// converti le type Matrix4 d'OpenTK en tableau de double
        /// </summary>
        /// <param name="m4">Matrix4 à convertir </param>
        /// <returns></returns>
        private double[] Matrix4ToDouble(Matrix4 m4)
        {
            double[] res = new double[16] {m4.M11, m4.M12, m4.M13, m4.M14,
                                           m4.M21, m4.M22, m4.M23, m4.M24,
                                           m4.M31, m4.M32, m4.M33, m4.M34,
                                           m4.M41, m4.M42, m4.M43, m4.M44 };

            return res;
        }

        /// <summary>
        /// Converti une matrice double en Matrix4 d'OpenTK
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private Matrix4 DoubleToMatrix4(double[] m)
        {
            Matrix4 res = new Matrix4((float)m[0], (float)m[1], (float)m[2], (float)m[3],
                                      (float)m[4], (float)m[5], (float)m[6], (float)m[7],
                                      (float)m[8], (float)m[9], (float)m[10], (float)m[11],
                                      (float)m[12], (float)m[13], (float)m[14], (float)m[15]);

            return res;
        }

        /// <summary>
        /// Déplace la vecteur pos d'une distance d dans la direction du vecteur dir (fonction move de Vect3D)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private Vector3 MoveVector3(Vector3 pos, Vector3 dir, float d)
        {
            Vector3 l_dir = dir;
            l_dir.Normalize();
            l_dir = Vector3.Multiply(l_dir, d);
            return pos + l_dir;
        }

        /// <summary>
        /// recupère les vecteur eye, target et up à partir de la matrice modelView
        /// </summary>
        private void ExtractEyeTargetUp(double[] modelViewMatrix, out Vector3 eye, out Vector3 target, out Vector3 up)
        {
            Vector4 eyePos;

            Matrix4 modelView = DoubleToMatrix4(modelViewMatrix);

            Matrix4 rotatMatrixTransposed = modelView;
            rotatMatrixTransposed.Row3 = Vector4.UnitW;

            rotatMatrixTransposed.Transpose();

            up = Vector3.Transform(new Vector3(0, 1, 0), rotatMatrixTransposed);

            modelView.Invert();
            eyePos = Vector4.Transform(new Vector4(0, 0, 0, 1), modelView);
            eyePos /= eyePos.W;
            eye = new Vector3(eyePos);

            Vector3 ZVectorTransf = Vector3.Transform(new Vector3(0, 0, -1), rotatMatrixTransposed);

            target = MoveVector3(eye, ZVectorTransf, eye.Length);

        }

        //private void InitTarget0Up0(out Vector3 t0, out Vector3 u0)
        //{

        //}


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
                if (diffW >= diffH/*frameSize.Width <= frameSize.Height*/)
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
