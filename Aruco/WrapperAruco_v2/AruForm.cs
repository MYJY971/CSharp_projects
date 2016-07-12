﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using Emgu.CV;
using Emgu.CV.CvEnum;

using Aruco.Net;
using OpenCV.Net;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing.Imaging;

using cvSize = OpenCV.Net.Size;
using sSize = System.Drawing.Size;
using TexLib;

namespace WrapperAruco_v2
{
    public partial class AruForm : Form
    {
        private Capture _cameraCapture;
        private CameraParameters _parameters;
        private float _markerSize;
        private Mat _cameraMatrix;
        private Mat _distortion;
        private MarkerDetector _detector;
        private IList<Marker> _detectedMarkers;
        private bool _initContextGLisOk;
        Matrix4 _defaultProjection, _lookatMatrix;
        IplImage _backgroundImage;
        IplImage _copyFrame;
        IntPtr _backgroundData;
        bool _cameraOn = false;
        bool _sizeChanged = false;
        float _factorSize = 1f;

        private BoardDetector _boardDetector;
        private Board _board;
        private BoardConfiguration _boardConfig;
        private IList<Marker> _detectedMarkersBoard;

        private Vector3 _eye = new Vector3(-10.0f, 0.0f, 0.0f);
        private Vector3 _target = Vector3.Zero;
        private Vector3 _up = new Vector3(0.0f, 0.0f, 1.0f);

        float _angle = 0;

        private int _objectTextureId;

        IplImage _frame;
        Emgu.CV.Mat _emguFrame;

        public AruForm()
        {

            InitializeComponent();
            //Create camera parameters and load them from a YML file
            _parameters = new CameraParameters();


            _cameraMatrix = new Mat(3, 3, Depth.F32, 1);
            _distortion = new Mat(1, 4, Depth.F32, 1);

            _detector = new MarkerDetector();
            _detector.ThresholdMethod = ThresholdMethod.AdaptiveThreshold;
            _detector.Param1 = 7.0;
            _detector.Param2 = 7.0;
            _detector.MinSize = 0.04f;
            _detector.MaxSize = 0.5f;
            _detector.CornerRefinement = CornerRefinementMethod.Lines;

            _boardDetector = new BoardDetector();
            _boardConfig = new BoardConfiguration();

            Marker m;
            
            _markerSize = 4f;


            try
            {
                _parameters.ReadFromXmlFile("intrinsics.yml");
                OpenCV.Net.Size size;
                _parameters.CopyParameters(_cameraMatrix, _distortion, out size);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }




        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            //SetupViewport();
            _objectTextureId = TexUtil.CreateTextureFromFile("texture.png");
            Run();
            SetupViewport();
        }



        private void Run()
        {
            try
            {

                _cameraCapture = OpenCV.Net.Capture.CreateCameraCapture(0);
                //_cameraCapture.SetProperty(CaptureProperty.FrameWidth, panelVideo.Width/*5120*/);
                //_cameraCapture.SetProperty(CaptureProperty.FrameHeight,panelVideo.Height/*2160*/);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }



            //imageVideo.Resize += ImageVideo_Resize;
            Application.Idle += ProcessFrame;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                  try
                   {
                   

                //_cameraCapture = OpenCV.Net.Capture.CreateCameraCapture(0);

                _frame = _cameraCapture.QueryFrame();

                    //IList<Marker> detectedMarkers;

                     _detectedMarkers = _detector.Detect(_frame, _cameraMatrix, _distortion, _markerSize);
                     foreach (var marker in _detectedMarkers)
                     {
                         label1.Text = "" + marker.Id;
                     }

                    //_board = _boardDetector.Detect(_detectedMarkers, _boardConfig, _cameraMatrix, _distortion, _markerSize);


                //label1.Text = "nb marqueurs: " + _detectedMarkers.Count;


                System.Drawing.Size sizeFrame = new System.Drawing.Size(_frame.Width, _frame.Height);

                _emguFrame = new Emgu.CV.Mat(sizeFrame, DepthType.Cv8U, _frame.Channels, _frame.ImageData, _frame.WidthStep);


                if (!_cameraOn)
                    _cameraOn = true;
                //imageVideo.Image = _emguFrame;

                //_angle += 10;
                _backgroundImage = _frame;

                /*
                 cvSize size = new cvSize(panelVideo.Width, panelVideo.Height);
                 IplImage frameResized = new IplImage(size, OpenCV.Net.IplDepth.U8, 3);
                 OpenCV.Net.CV.Resize(_backgroundImage, frameResized);
                */

                Render();
                }
                catch (Exception ex)
                {
                    try
                    {
                        _cameraCapture = OpenCV.Net.Capture.CreateCameraCapture(0);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message);

                        return;
                    }
                }
            }
        }

        private void ImageVideo_Resize(object sender, EventArgs e)
        {
            //_cameraCapture.SetCaptureProperty(CapProp.FrameWidth, /*1920*/imageVideo.Size.Width);
            //_cameraCapture.SetCaptureProperty(CapProp.FrameHeight, /*1080*/imageVideo.Size.Height);
        }


        //OpenGL


        private void SetupViewport()
        {


            int w = glControl1.Width;
            int h = glControl1.Height;

            if (h == 0)                                                  // Prevent A Divide By Zero...
                h = 1;                                                   // By Making Height Equal To One

            _defaultProjection = Matrix4.CreatePerspectiveFieldOfView((float)System.Math.PI / 4, (float)w / (float)h, 0.1f, 100.0f);


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

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {

            Render();
        }

        private void Render()
        {
            if (!_initContextGLisOk)
                InitGLContext();




            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_cameraOn)
            {
                if (!_sizeChanged)
                {
                    //glControl1.Size = new sSize(_backgroundImage.Width, _backgroundImage.Height);
                    //glControl1.Size = panelVideo.Size;
                    sSize frameSize = new sSize(_frame.Width, _frame.Height);
                    AdaptSize(frameSize, out _factorSize);
                    label2.Text = "" + _frame.Width + " x " + _frame.Height;
                    _sizeChanged = true;
                    //glControl1.SetBounds(0, 10, _backgroundImage.Width, _backgroundImage.Height);
                }

                int w = glControl1.Width;
                int h = glControl1.Height;

                //background = video

                //Set Projection Ortho
                GL.MatrixMode(MatrixMode.Projection);                               // Select The Projection Matrix
                GL.LoadIdentity();                                                  // Reset The Projection Matrix

                GL.Ortho(0, w, 0, h, -1.0, 1.0);
                GL.Viewport(0, 0, w, h);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                float fact = _factorSize;
                GL.Disable(EnableCap.Texture2D);
                GL.PixelZoom(1.0f * fact, -1.0f * fact);
                GL.RasterPos3(0f, h - 0.5f, -1.0f);

                cvSize s = new cvSize(w, h);

                //IplImage tmpFrame = _frame;
                //_backgroundImage = new IplImage(s, _frame.Depth, _frame.Channels);
                //OpenCV.Net.CV.Resize(_copyFrame, _backgroundImage);


                //_backgroundImage = _frame;

                GL.DrawPixels(_backgroundImage.Width, _backgroundImage.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, _backgroundImage.ImageData);
                GL.Enable(EnableCap.Texture2D);

                //Scene 3D
                GL.MatrixMode(MatrixMode.Projection);
                double[] projMatrix = new double[16];
                cvSize glSize = new cvSize(glControl1.Width, glControl1.Height);
                GlGetProjectionMatrix(_backgroundImage.Size, glSize, out projMatrix, 0.1, 100, false);
                GL.LoadIdentity();
                GL.MultMatrix(projMatrix);

                //now, for each marker,
                double[] modelview_matrix = new double[16];


                for (int m = 0; m < _detectedMarkers.Count; m++)
                {
                    modelview_matrix = _detectedMarkers.ElementAt(m).GetGLModelViewMatrix();
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();
                    GL.LoadMatrix(modelview_matrix);

                    //axis(TheMarkerSize);
                    //DrawCube();
                    GL.Rotate(-90, Vector3.UnitY);
                    GL.Rotate(-90, Vector3.UnitX);
                    //DrawTrihedral();
                    DrawCube(_markerSize);
                    GL.Rotate(90, Vector3.UnitX);
                    GL.Rotate(90, Vector3.UnitY);

                    GL.Translate(0, 0, _markerSize / 2);
                    GL.PushMatrix();
                    //glutWireCube(TheMarkerSize);

                    GL.PopMatrix();

                }




            }

            else
            {
                //Scene 3D

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.MultMatrix(ref _defaultProjection);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                _lookatMatrix = Matrix4.LookAt(_eye, _target, _up);
                GL.LoadMatrix(ref _lookatMatrix);

                DrawScene();
            }


            glControl1.SwapBuffers();
            //Refresh();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            SetupViewport();
            //glControl1.Invalidate();
        }
        /// Draw Methods
        private void DrawTrihedral()
        {
            //DrawTrihedral ///////////////////////

            float[] l_couleur = new float[4];
            float l_shin;
            float l_lenghAxis = 1.0f;
            float l_flecheW = 0.5f; float l_flecheH = 0.5f;

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
        }

        private void DrawCube()
        {
            float[] l_couleur = new float[4];
            float l_shin;
            GL.Rotate(_angle, Vector3.UnitZ);
            // axe X
            //GL.Color4(1.0f, 0.0f, 0.0f, 0.5f);
            //l_couleur[0] = 1.0f; l_couleur[1] = 0.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            ///*
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

        private void DrawScene()
        {
            DrawTrihedral();

            GL.Rotate(_angle, Vector3.UnitZ);
            DrawCube();
        }

        ///ARUCO
        ///GLProjection

        #region Aruco Methods

        private void GlGetProjectionMatrix(cvSize orgImgSize, cvSize size, out double[] proj_matrix, double gnear, double gfar, bool invert)
        {
            // Deterime the rsized info
            double Ax = (double)size.Width / (double)orgImgSize.Width;
            double Ay = (double)size.Height / (double)orgImgSize.Height;
            double fx = _cameraMatrix[0, 0].Val0;
            double cx = _cameraMatrix[0, 2].Val0;
            double fy = _cameraMatrix[1, 1].Val0;
            double cy = _cameraMatrix[1, 2].Val0;

            double[,] cparam = new double[3, 4] { { fx, 0, cx, 0 }, { 0, fy, cy, 0 }, { 0, 0, 1, 0 } };

            argConvGLcpara2(cparam, size.Width, size.Height, gnear, gfar, out proj_matrix, invert);
        }
        #region glGetProjectionMatrix



        private void argConvGLcpara2(double[,] cparam, int width, int height, double gnear, double gfar, out double[] m, bool invert)
        {
            m = new double[16];
            double[,] icpara = new double[3, 4];
            double[,] trans = new double[3, 4];
            double[,] p = new double[3, 3];
            double[,] q = new double[4, 4];
            int i, j;

            cparam[0, 2] *= -1.0;
            cparam[1, 2] *= -1.0;
            cparam[2, 2] *= -1.0;

            if (arParamDecompMat(cparam, icpara, trans) < 0)
                MessageBox.Show("Error");

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    p[i, j] = icpara[i, j] / icpara[2, 2];
                }
            }

            q[0, 0] = (2.0 * p[0, 0] / width);
            q[0, 1] = (2.0 * p[0, 1] / width);
            q[0, 2] = ((2.0 * p[0, 2] / width) - 1.0);
            q[0, 3] = 0.0;

            q[1, 0] = 0.0;
            q[1, 1] = (2.0 * p[1, 1] / height);
            q[1, 2] = ((2.0 * p[1, 2] / height) - 1.0);
            q[1, 3] = 0.0;

            q[2, 0] = 0.0;
            q[2, 1] = 0.0;
            q[2, 2] = (gfar + gnear) / (gfar - gnear);
            q[2, 3] = -2.0 * gfar * gnear / (gfar - gnear);

            q[3, 0] = 0.0;
            q[3, 1] = 0.0;
            q[3, 2] = 1.0;
            q[3, 3] = 0.0;

            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    m[i + j * 4] = q[i, 0] * trans[0, j] + q[i, 1] * trans[1, j] + q[i, 2] * trans[2, j];
                }
                m[i + 3 * 4] = q[i, 0] * trans[0, 3] + q[i, 1] * trans[1, 3] + q[i, 2] * trans[2, 3] + q[i, 3];
            }

            if (!invert)
            {
                m[13] = -m[13];
                m[1] = -m[1];
                m[5] = -m[5];
                m[9] = -m[9];
            }
        }

        double norm(double a, double b, double c) { return (Math.Sqrt(a * a + b * b + c * c)); }

        double dot(double a1, double a2, double a3, double b1, double b2, double b3) { return (a1 * b1 + a2 * b2 + a3 * b3); }

        private int arParamDecompMat(double[,] source, double[,] cpara, double[,] trans)
        {
            int r, c;
            double[,] Cpara = new double[3, 4];
            double rem1, rem2, rem3;

            if (source[2, 3] >= 0)
            {
                for (r = 0; r < 3; r++)
                {
                    for (c = 0; c < 4; c++)
                    {
                        Cpara[r, c] = source[r, c];
                    }
                }
            }
            else
            {
                for (r = 0; r < 3; r++)
                {
                    for (c = 0; c < 4; c++)
                    {
                        Cpara[r, c] = -(source[r, c]);
                    }
                }
            }

            for (r = 0; r < 3; r++)
            {
                for (c = 0; c < 4; c++)
                {
                    cpara[r, c] = 0.0;
                }
            }

            cpara[2, 2] = norm(Cpara[2, 0], Cpara[2, 1], Cpara[2, 2]);
            trans[2, 0] = Cpara[2, 0] / cpara[2, 2];
            trans[2, 1] = Cpara[2, 1] / cpara[2, 2];
            trans[2, 2] = Cpara[2, 2] / cpara[2, 2];
            trans[2, 3] = Cpara[2, 3] / cpara[2, 2];

            cpara[1, 2] = dot(trans[2, 0], trans[2, 1], trans[2, 2], Cpara[1, 0], Cpara[1, 1], Cpara[1, 2]);
            rem1 = Cpara[1, 0] - cpara[1, 2] * trans[2, 0];
            rem2 = Cpara[1, 1] - cpara[1, 2] * trans[2, 1];
            rem3 = Cpara[1, 2] - cpara[1, 2] * trans[2, 2];
            cpara[1, 1] = norm(rem1, rem2, rem3);
            trans[1, 0] = rem1 / cpara[1, 1];
            trans[1, 1] = rem2 / cpara[1, 1];
            trans[1, 2] = rem3 / cpara[1, 1];

            cpara[0, 2] = dot(trans[2, 0], trans[2, 1], trans[2, 2], Cpara[0, 0], Cpara[0, 1], Cpara[0, 2]);
            cpara[0, 1] = dot(trans[1, 0], trans[1, 1], trans[1, 2], Cpara[0, 0], Cpara[0, 1], Cpara[0, 2]);
            rem1 = Cpara[0, 0] - cpara[0, 1] * trans[1, 0] - cpara[0, 2] * trans[2, 0];
            rem2 = Cpara[0, 1] - cpara[0, 1] * trans[1, 1] - cpara[0, 2] * trans[2, 1];
            rem3 = Cpara[0, 2] - cpara[0, 1] * trans[1, 2] - cpara[0, 2] * trans[2, 2];
            cpara[0, 0] = norm(rem1, rem2, rem3);
            trans[0, 0] = rem1 / cpara[0, 0];
            trans[0, 1] = rem2 / cpara[0, 0];
            trans[0, 2] = rem3 / cpara[0, 0];

            trans[1, 3] = (Cpara[1, 3] - cpara[1, 2] * trans[2, 3]) / cpara[1, 1];
            trans[0, 3] = (Cpara[0, 3] - cpara[0, 1] * trans[1, 3] - cpara[0, 2] * trans[2, 3]) / cpara[0, 0];

            for (r = 0; r < 3; r++)
            {
                for (c = 0; c < 3; c++)
                {
                    cpara[r, c] /= cpara[2, 2];
                }
            }

            return 0;

        }


        #endregion

        #endregion

        private void AdaptSize(sSize frameSize, out float factor)
        {

            float diffW, diffH;
            sSize panSize = panelVideo.Size;

            diffW = frameSize.Width - panSize.Width;
            diffH = frameSize.Height - panSize.Height;

            if (diffW > 0 || diffH > 0)
            {
                if (frameSize.Width <= frameSize.Height)
                {
                    factor = (float)panelVideo.Size.Width / (float)frameSize.Width;
                }
                else
                {
                    factor = (float)panelVideo.Size.Height / (float)frameSize.Height;
                }
            }
            else
            {
                factor = 1.0f;
            }

            sSize newSize = new sSize((int)(frameSize.Width * factor), (int)(frameSize.Height * factor));

            CenterImage(newSize);

        }

        private void CenterImage(sSize size)
        {
            float diffW, diffH;
            sSize sizePan = panelVideo.Size;

            diffW = sizePan.Width - size.Width;
            diffH = sizePan.Height - size.Height;

            int x = 0, y = 0;

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

        private void MultMatGL(double[] mat, double f)
        {
            //int i;

            for (int i = 0; i < mat.Count(); i++)
            {
                mat[i] = mat[i] * f;
            }

        }

        private double[] AddMatGL(double[] mat1, double[] mat2)
        {
            for (int i = 0; i < mat1.Count(); i++)
            {
                mat1[i] = mat1[i] + mat2[2];
            }
            return mat1;
        }

        private double[] DivMatGL(double[] mat, double q)
        {
            for (int i = 0; i < mat.Count(); i++)
            {
                if (mat[i] != 0)
                {
                    mat[i] = mat[i] / q;
                }
            }

            return mat;
        }
    }
}
