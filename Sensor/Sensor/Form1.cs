﻿using System;
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
using MYTestOpenGl;

//sensor
using Windows.Devices.Sensors;

namespace Sensor
{
    public partial class Form1 : Form
    {
        List<int> _idTextures;

        Mesh _obj1, _sky;

        double[] _modelViewMatrix;

        Vector3 _eye = new Vector3(15.0f, 0.0f, 4.0f);
        Vector3 _target = Vector3.Zero;
        Vector3 _up = Vector3.UnitZ;

        Vector3 _eye0, _target0, _up0;


        Matrix4 _sensorMatrix;

        private Matrix4 _RotationX;
        private Matrix4 _RotationZ;

        //sensor
        private OrientationSensor _orientationSensor;
        private Compass _compass;

        private bool _compassON;

        private double _angleCompass;

        #region tmp
        float _angle;

        #endregion
        public Form1()
        {


            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            _compassON = false;

            panel1.Visible = false;
            panel2.Visible = false;

            _eye0 = _eye;
            _target0 = _target;
            _up0 = _up;

            _modelViewMatrix = new double[16];

            _sensorMatrix = Matrix4.Identity;

            //initRotationXZ();

            initMeshes();
            initTextures();
            TexUtil.InitTexturing();
            InitGLContext();
            SetupViewport();

            //TestComputeSensorOr();

            Run();
        }

        private void Run()
        {
            _orientationSensor = OrientationSensor.GetDefault();
            if (_orientationSensor != null && !_compassON)
            {

                _orientationSensor.ReadingChanged += _orientationSensor_ReadingChanged;
            }
            else
            {
                MessageBox.Show("Aucun capteur d'orientation détécté");
            }

            _compass = Compass.GetDefault();
            if(_compass != null && _compassON)
            {
                _compass.ReadingChanged += _compass_ReadingChanged;
            }
            else
            {
                MessageBox.Show("Aucune boussole détécté");
            }
            

            Application.Idle += Application_Idle;
        }

        

        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                if (_orientationSensor == null)
                {
                    //_angle += 0.001f;
                    //_sensorMatrix = Matrix4.CreateRotationZ(_angle);
                }
                Render();
            }
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


            Vector3 tmp1, tmp2;

            _sensorMatrix.Invert();
            tmp1 = Vector3.Transform(_eye, _sensorMatrix);
            tmp2 = _target0 - tmp1;
            _target = tmp2 + _eye;

            _up = Vector3.Transform(_up0, _sensorMatrix);


            

        }

        /// <summary>
        /// Recupère l'angle en degrès entre notre position et le nord
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void _compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            _angleCompass = Math.PI * -args.Reading.HeadingMagneticNorth / 180.0;

            Vector3 tmp1, tmp2;

            Matrix4 _compassMat = Matrix4.CreateRotationZ((float)_angleCompass);
            tmp1 = Vector3.Transform(_eye, _compassMat);
            tmp2 = _target0 - tmp1;
            _target = tmp2 + _eye;

            _up = Vector3.Transform(_up0, _compassMat);

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

        /// <summary>
        /// Initialise les textures les ajoute dans _idTextures
        /// </summary>
        private void initTextures()
        {

            _idTextures = new List<int>();

            int sky = TexUtil.CreateTextureFromFile("DATA\\sky10.jpg");
            int sand = TexUtil.CreateTextureFromFile("Data\\sand_texture.jpg");
            int obj1 = TexUtil.CreateTextureFromFile("DATA\\garnet_texture.jpg");
            int sky2 = TexUtil.CreateTextureFromFile("DATA\\sky_photo.jpg");


            _idTextures.Add(sky);
            _idTextures.Add(sand);
            _idTextures.Add(obj1);
            _idTextures.Add(sky2);
        }

        /// <summary>
        /// initialise les objet .obj
        /// </summary>
        private void initMeshes()
        {

            _obj1 = new Mesh("DATA\\icosphere.obj");
            _sky = new Mesh("DATA\\dome.obj");
            //_obj1 = new Mesh("DATA\\dome.obj");

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
            #region ModelView
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4 lookat = Matrix4.LookAt(_eye, _target, _up);
            _modelViewMatrix = Matrix4ToDouble(lookat);

            GL.LoadMatrix(/*ref modelView /**/_modelViewMatrix/**/);
            #endregion

            DrawGround();
            DrawTrihedral();
            DrawObject1();
            DrawSky();

            //Vector3 e, t, u;
            //ExtractEyeTargetUp(_modelViewMatrix, out e, out t, out u);
        }

        private void DrawGround()
        {
            //GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            //GL.DepthMask(false);

            GL.BindTexture(TextureTarget.Texture2D, _idTextures[1]);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 10); GL.Vertex3(-100.0f, -100.0f, 0.0f);
            GL.TexCoord2(0, 0); GL.Vertex3(-100.0f, 100.0f, 0.0f);
            GL.TexCoord2(10, 0); GL.Vertex3(100.0f, 100.0f, 0.0f);
            GL.TexCoord2(10, 10); GL.Vertex3(100.0f, -100.0f, 0.0f);
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

        private void DrawSky()
        {
            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, _idTextures[3]);

            int s = 50;

            GL.Scale(s, s, s);
            _sky.Draw();
            GL.Scale(1 / s, 1 / s, 1 / s);


            GL.Disable(EnableCap.Texture2D);
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
        /// Affiche les valeur de la matrice de rotation récupéré par le capteur d'orientation
        /// </summary>
        private void DisplayRotationMatrix()
        {
            panel1.Visible = true;

            double[] sensorMatrix = Matrix4ToDouble(_sensorMatrix);

            textBoxM11.Text = "" + sensorMatrix[0];
            textBoxM12.Text = "" + sensorMatrix[1];
            textBoxM13.Text = "" + sensorMatrix[2];
            textBoxM14.Text = "" + sensorMatrix[3];

            textBoxM21.Text = "" + sensorMatrix[4];
            textBoxM22.Text = "" + sensorMatrix[5];
            textBoxM23.Text = "" + sensorMatrix[6];
            textBoxM24.Text = "" + sensorMatrix[7];

            textBoxM31.Text = "" + sensorMatrix[8];
            textBoxM32.Text = "" + sensorMatrix[9];
            textBoxM33.Text = "" + sensorMatrix[10];
            textBoxM34.Text = "" + sensorMatrix[11];

            textBoxM41.Text = "" + sensorMatrix[12];
            textBoxM42.Text = "" + sensorMatrix[13];
            textBoxM43.Text = "" + sensorMatrix[14];
            textBoxM44.Text = "" + sensorMatrix[15];
        }

        /// <summary>
        /// Affiche la matrice de modelview
        /// </summary>
        private void DisplayModelViewMatrix()
        {
            panel2.Visible = true;

            textBoxMV11.Text = "" + _modelViewMatrix[0];
            textBoxMV12.Text = "" + _modelViewMatrix[1];
            textBoxMV13.Text = "" + _modelViewMatrix[2];
            textBoxMV14.Text = "" + _modelViewMatrix[3];

            textBoxMV21.Text = "" + _modelViewMatrix[4];
            textBoxMV22.Text = "" + _modelViewMatrix[5];
            textBoxMV23.Text = "" + _modelViewMatrix[6];
            textBoxMV24.Text = "" + _modelViewMatrix[7];

            textBoxMV31.Text = "" + _modelViewMatrix[8];
            textBoxMV32.Text = "" + _modelViewMatrix[9];
            textBoxMV33.Text = "" + _modelViewMatrix[10];
            textBoxMV34.Text = "" + _modelViewMatrix[11];

            textBoxMV41.Text = "" + _modelViewMatrix[12];
            textBoxMV42.Text = "" + _modelViewMatrix[13];
            textBoxMV43.Text = "" + _modelViewMatrix[14];
            textBoxMV44.Text = "" + _modelViewMatrix[15];
        }

        /// <summary>
        /// recupère les vecteur eye, target et up à partir de la matrice modelView
        /// </summary>
        private void ExtractEyeTargetUp(double[] modelViewMatrix, out Vector3 eye, out Vector3 target, out Vector3 up)
        {
            //Vector3 eye, target, up;
            Vector4 eyePos;
             

            Matrix4 modelView = DoubleToMatrix4(modelViewMatrix);
            //Matrix4 OriginalModelView = modelView;

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


            //Matrix4 extratedModelView = Matrix4.LookAt(_eye, target, _up);
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


        #region Test Personnel

        #endregion
    }
}
