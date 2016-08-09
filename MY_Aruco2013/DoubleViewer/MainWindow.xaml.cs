using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Img = System.Drawing.Imaging;

using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Windows.Interop;

namespace DoubleViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static GLControl opengl;

        #region Attributs
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

        //private Mat _backgroundImage;
        //private Mat _frameComputed, _frameResized;



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



        #endregion

        private Bitmap _backgroundImage;
        private int _IdTextBackground;

        public MainWindow()
        {
            InitializeComponent();
            opengl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 4));
            opengl.Width = 640;
            opengl.Height = 480;
            opengl.Load += new EventHandler(opengl_Load);
            opengl.Paint += new System.Windows.Forms.PaintEventHandler(opengl_Paint);
            winhost.Child = opengl;
        }

        void opengl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            RenderGL();
        }

        void opengl_Load(object sender, EventArgs e)
        {
            InitGLContext();
            SetupViewport();
            Run();

        }



        private void Run()
        {
            try
            {
                //Allume la caméra 
                if (_cameraCapture == null)
                {
                    _cameraCapture = new Capture();

                    ComponentDispatcher.ThreadIdle += new System.EventHandler(ProcessFrame);
                }

                /* La résolution par défaut prise en compte par Emgu est de 640x480
                 * Ici on fixe une résoltion de base, parmis les différentes résolution que peut prendre la caméra,
                 * elle prendra celle qui se rapproche le plus de la taille fixé.
                 * 
                 * (5120 x 2160) correspond à une résolution 4K
                 *                  
                 */
                //_cameraCapture.SetCaptureProperty(CapProp.FrameWidth, 1280/*5120*/);
                //_cameraCapture.SetCaptureProperty(CapProp.FrameHeight, 720 /*2160*/);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "aucune caméra détécté");
                return;
            }

        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            while (opengl.IsIdle)
            {
                //récuperation de l'image courante filmé par la caméra
                _frame = _cameraCapture.QueryFrame();
               // Bitmap bt = _frame.Bitmap;
                //_IdTextBackground = CreateTextureFromBitmap(bt);
                //indique que la caméra est allumé
                if (!_cameraOn)
                    _cameraOn = true;

                //_backgroundImage = _frame;

                //_frameComputed = _frame;
                //_frameResized = _frame;

                ////Redimensionne l'image 
                //CvInvoke.Resize(_frameResized, _frameComputed, new Size(640, 480));

                //bt.Dispose();
                RenderGL();
            }

        }


        public static int CreateTextureFromBitmap(Bitmap bitmap)
        {
            Img.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
              Img.ImageLockMode.ReadOnly,
              Img.PixelFormat.Format32bppArgb);

            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexImage2D(
              TextureTarget.Texture2D,
              0,
              PixelInternalFormat.Rgba,
              data.Width, data.Height,
              0,
              OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
              PixelType.UnsignedByte,
              data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);

            return tex;
        }

        private void RenderGL()
        {

            

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_cameraOn)
            {
                _IdTextBackground = CreateTextureFromBitmap(_frame.Bitmap);
                int w = opengl.Width;
                int h = opengl.Height;


                //Set Projection Ortho
                GL.MatrixMode(MatrixMode.Projection);                               // Select The Projection Matrix
                GL.LoadIdentity();                                                  // Reset The Projection Matrix

                GL.Ortho(0, w, 0, h, -1.0, 1.0);
                GL.Viewport(0, 0, w, h);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                //GL.Disable(EnableCap.Texture2D);
                //GL.Color3(1.0, 0.0, 0.0);
                GL.Enable(EnableCap.Texture2D);
                GL.Begin(BeginMode.Quads);
                GL.BindTexture(TextureTarget.Texture2D, _IdTextBackground);
                GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
                GL.TexCoord2(0, 1); GL.Vertex2(0, h);
                GL.TexCoord2(1, 1); GL.Vertex2(w, h);
                GL.TexCoord2(1, 0); GL.Vertex2(w, 0);

                GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
                GL.TexCoord2(0, 1); GL.Vertex2(0, 100);
                GL.TexCoord2(1, 1); GL.Vertex2(100, 100);
                GL.TexCoord2(1, 0); GL.Vertex2(100, 0);

                GL.End();



                GL.Disable(EnableCap.Texture2D);

                //Background



                //Scene 3D



            }

            opengl.SwapBuffers();


        }

        private void SetupViewport()
        {
            int w = opengl.Width;
            int h = opengl.Height;

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

            GL.Light(LightName.Light0, LightParameter.Ambient, new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Diffuse, new OpenTK.Graphics.Color4(0.6f, 0.6f, 0.6f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Specular, new OpenTK.Graphics.Color4(0.6f, 0.6f, 0.6f, 1.0f));

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            //GL.Enable(EnableCap.CullFace);

            _initContextGLisOk = true;
        }


    }


}
