﻿using System;
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
//using ObjLoader.Loader.Loaders;

namespace MY_Aruco_v2
{
    public partial class AruForm : Form
    {
        private Matrix4 _defaultProjection, _defaultlookatMatrix;
        private Capture _cameraCapture;
        Mat _frame;
        private bool _cameraOn = false;
        private bool _initContextGLisOk;
        private Vector3 _eye = new Vector3(-10.0f, 0.0f, 0.0f);
        private Vector3 _target = Vector3.Zero;
        private Vector3 _up = new Vector3(0.0f, 0.0f, 1.0f);
        private bool _sizeChanged;
        private float _factorSize = 1f;
        private string _pathCamPara, _pathMapPara;
        int _nbMarker = 0;
        double[] _modelViewMatrix;
        float _markerSize;
        private int _objectTextureId;
        bool _changeResolution = false;
        private int _xCenter, _yCenter;
        private Size _sizeGL, _sizeGLAdapted;

        private Mat _backgroundImage;
        private Mat _frameComputed, _frameResized;
        private bool _stop = false;

        bool _isFullSize;
        bool _isAdaptedSize;
        bool _segmented;

        private bool _ARactived = true;

        byte[] _imageSegmented;
        private int _tresh1, _tresh2;

        private bool _isDetected;

        private double[] _modelViewMat;

        [DllImport("..\\..\\..\\Debug\\ArucoDll.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PerformARMarkerTEST(byte[] image, string path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
        double gnear, double gfar, double[] proj_matrix, double[] modelview_matrix, float markerSize, out int nbDetectedMarkers, int treshParam1, int treshParam2);

        //[DllImport("ArucoDll.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern void PerformARMarker(byte[] image, string path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
        //double gnear, double gfar, double[] proj_matrix, double[] modelview_matrix, float markerSize, out int nbDetectedMarkers, int treshParam1, int treshParam2);

        //[DllImport("ArucoDll.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern void PerformAR(byte[] image, string path_MapPara, string path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
        //double gnear, double gfar, double[] proj_matrix, double[] modelview_matrix, float markerSize, out int nbDetectedMarkers);

        Mesh _mesh;

        public AruForm()
        {
            InitializeComponent();
            try
            {
                _pathCamPara = "DATA\\intrinsics.yml";
                _pathMapPara = "DATA\\map4.yml";
                

                _markerSize = /**/1.09f;/*0.2f;/**/
                _isAdaptedSize = true;
                _isFullSize = false;

                _tresh1 = 12;
                _tresh2 = 13;

                _segmented = false;
                _isDetected = true;

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
            _mesh = new Mesh();
            //_mesh.Load("DATA\\sphere.obj");
            _mesh.Load("DATA\\torus.obj");
            //_mesh.Load("DATA\\Caraïbes.obj");
            //_mesh.Load("DATA\\Pokeball.obj");

            _objectTextureId = TexUtil.CreateTextureFromFile("DATA\\texture.png");
            Run();
            SetupViewport();
        }

        private void Run()
        {

            try
            {


                _cameraCapture = new Capture();
                _cameraCapture.SetCaptureProperty(CapProp.FrameWidth, 1280/*5120*/);
                _cameraCapture.SetCaptureProperty(CapProp.FrameHeight, 720 /*2160*/);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            Application.Idle += ProcessFrame;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {

            while (glControl1.IsIdle)
            {

                //////////////////////////////////////////////////////

                _frame = _cameraCapture.QueryFrame();

                if (!_cameraOn && !_stop)
                    _cameraOn = true;
                _backgroundImage = _frame;

                _frameComputed = _frame;
                _frameResized = _frame;
                CvInvoke.Resize(_frameResized, _frameComputed, new Size(640, 480));
                //CvInvoke.Resize(_frame, _frameResized, new Size(640, 480));
                Render();
            }

        }

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

            float[] lightPos = { 10.0f, 10.0f, 100.0f, 0.1f };
            GL.Light(LightName.Light0, LightParameter.Position, lightPos);
            GL.Light(LightName.Light0, LightParameter.SpotDirection, new float[] { 0, 0, -1});

            GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(0.6f, 0.6f, 0.6f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Specular, new Color4(0.6f, 0.6f, 0.6f, 1.0f));

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            //GL.Enable(EnableCap.CullFace);

            _initContextGLisOk = true;

        }


        private void Render()
        {
            if (!_initContextGLisOk)
                InitGLContext();
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_cameraOn)
            {

                if (_isAdaptedSize)
                {
                    if (!_sizeChanged)
                    {

                        //Size frameSize = new Size(_frame.Width, _frame.Height);
                        AdaptSize(_frame.Size, out _factorSize);
                        _sizeChanged = true;

                    }

                }
                else
                {

                    glControl1.Size=panelImage.Size;
                    glControl1.SetBounds(0, 0, glControl1.Width, glControl1.Height);
                }

                label2.Text = "" + _frame.Width + " x " + _frame.Height;
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


                
                GL.Disable(EnableCap.Texture2D);

                
                Image<Bgr, byte> _imageForARCompute = new Image<Bgr, byte>(_frameComputed.Width, _frameComputed.Height);
                CvInvoke.Resize(_frameComputed, _imageForARCompute, _frameComputed.Size);
                byte[] byteImageForARCompute = _imageForARCompute.Bytes;

                double[] projMatrix = new double[16];
                double[] modelviewMatrix = new double[16];
                int tmp=0;
                if(_ARactived)
                //PerformARMarker(byteImageForARCompute, _pathCamPara, _frameComputed.Width, _frameComputed.Height,glControl1.Width,glControl1.Height, 0.1, 100, projMatrix, modelviewMatrix, _markerSize, out _nbMarker, _tresh1, _tresh2);
                tmp=PerformARMarkerTEST(byteImageForARCompute, _pathCamPara, _frameComputed.Width, _frameComputed.Height, glControl1.Width, glControl1.Height, 0.1, 100, projMatrix, modelviewMatrix, _markerSize, out _nbMarker, _tresh1, _tresh2);
                //PerformAR(byteImageForARCompute, _pathMapPara, _pathCamPara, _frame.Width, _frame.Height, glControl1.Width, glControl1.Height, 0.1, 100, projMatrix, lookatMatrix, _markerSize, out _nbMarker);
                GL.RasterPos3(0f, h - 0.5f, -1.0f);
                if (!_isFullSize)
                {
                    GL.PixelZoom(1.0f * _factorSize, -1.0f * _factorSize);
                    _backgroundImage = _frame;
                    if (_segmented)
                    {
                        
                      
                    }
                    else
                    {
                        GL.DrawPixels(_backgroundImage.Width, _backgroundImage.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, _backgroundImage.DataPointer);
                    }
                }
                else
                {
                   
                    GL.PixelZoom(1.0f, -1.0f);
                    CvInvoke.Resize(_frameResized, _backgroundImage, glControl1.Size);
                    GL.DrawPixels(_backgroundImage.Width, _backgroundImage.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, _backgroundImage.DataPointer);
                    _sizeChanged = false;
                }
                

                //Scene 3D

                //PerformARMarkers(byteImageForARCompute, _pathCamPara, _frame.Width, _frame.Height, glControl1.Width, glControl1.Height, 0.1, 100, projMatrix, lookatMatrix, _markerSize,out _nbMarker);
                //PerformAR(byteImageForARCompute, _pathMapPara, _pathCamPara, _frame.Width, _frame.Height, glControl1.Width, glControl1.Height, 0.1, 100, projMatrix, lookatMatrix, _markerSize, out _nbMarker);

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

                //glutWireCube(TheMarkerSize);

                GL.PopMatrix();



                



            }

            else
            {
                //Scene 3D

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.MultMatrix(ref _defaultProjection);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                _defaultlookatMatrix = /*Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);*/Matrix4.LookAt(_eye, _target, _up);
                GL.LoadMatrix(ref _defaultlookatMatrix);

                Draw3DScene();

            }


            glControl1.SwapBuffers();
            //Refresh();
        }

        #region Draw Methodes

        private void DrawScene()
        {

            GL.Translate(0, 0, _markerSize + 0.0001f);
            DrawTrihedral(_markerSize/2);
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
            float l_flecheW = size/5; float l_flecheH = size / 5;

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
        /// Cube 3D
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

        private void buttonSegmented_Click(object sender, EventArgs e)
        {
            _isAdaptedSize = true;
            _isFullSize = false;

            _segmented = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            _ARactived = false;
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
            _isAdaptedSize = false;
            _isFullSize = true;
            _segmented = false;
        }

        private void buttonAdaptedSize_Click(object sender, EventArgs e)
        {
            _isAdaptedSize = true;
            _isFullSize = false;
            _segmented = false;
        }
        #endregion

        



        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameSize"></param>
        /// <param name="factor"></param>
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
