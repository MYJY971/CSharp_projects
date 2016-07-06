using System;
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

namespace OpenCVFormTest
{
    public partial class OpenForm : System.Windows.Forms.Form
    {
        private Capture _cameraCapture;
        private CameraParameters _parameters;
        private int _markerSize;
        private Mat _cameraMatrix;
        private Mat _distortion;
        private MarkerDetector detector;
        private IList<Marker> detectedMarkers;

        public OpenForm()
        {

            InitializeComponent();
            try
            {
                //Create camera parameters and load them from a YML file
                _parameters = new CameraParameters();
                _parameters.ReadFromXmlFile("C:\\Stage\\Yanis\\Data\\intrinsics.yml");
                OpenCV.Net.Size size;
                _cameraMatrix = new Mat(3, 3, Depth.F32, 1);
                _distortion = new Mat(1, 4, Depth.F32, 1);
                _parameters.CopyParameters(_cameraMatrix, _distortion, out size);
                detector = new MarkerDetector();
                _markerSize = 10;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }



            Run();
        }

        private void Run()
        {
            try
            {
                _cameraCapture = OpenCV.Net.Capture.CreateCameraCapture(0);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }



            imageVideo.Resize += ImageVideo_Resize;
            Application.Idle += ProcessFrame;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            IplImage frame = _cameraCapture.QueryFrame();

            detector.ThresholdMethod = ThresholdMethod.AdaptiveThreshold;
            detector.Param1 = 7.0;
            detector.Param2 = 7.0;
            detector.MinSize = 0.04f;
            detector.MaxSize = 0.5f;
            detector.CornerRefinement = CornerRefinementMethod.Lines;

            detectedMarkers = detector.Detect(frame, _cameraMatrix, _distortion, _markerSize);
            foreach (var marker in detectedMarkers)
            {
                label1.Text = "" + marker.Id;
            }


            System.Drawing.Size sizeFrame = new System.Drawing.Size(frame.Width, frame.Height);

            Emgu.CV.Mat emguFrame = new Emgu.CV.Mat(sizeFrame, DepthType.Cv8U, frame.Channels, frame.ImageData, frame.WidthStep);
            imageVideo.Image = emguFrame;

        }

        private void ImageVideo_Resize(object sender, EventArgs e)
        {
            //_cameraCapture.SetCaptureProperty(CapProp.FrameWidth, /*1920*/imageVideo.Size.Width);
            //_cameraCapture.SetCaptureProperty(CapProp.FrameHeight, /*1080*/imageVideo.Size.Height);
        }

        private static void DrawTrihedral()
        {
            float[] l_couleur = new float[4];
            float l_shin;
            float l_lenghAxis = 1.0f;
            float l_flecheW = 0.1f; float l_flecheH = 0.05f;

            /*
            // axe X
            Gl.glColor4f(1.0f, 0.0f, 0.0f, 1.0f);
            l_couleur[0] = 1.0f; l_couleur[1] = 0.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, @l_couleur);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, @l_couleur);
            l_shin = 128;
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, ref l_shin);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, @l_couleur);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, @l_couleur);

            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3f(0.0f, 0.0f, 0.0f);
            Gl.glVertex3f(l_lenghAxis, 0.0f, 0.0f);
            Gl.glEnd();
            // point axe X
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3f(l_lenghAxis - l_flecheW, l_flecheH, 0.0f);
            Gl.glVertex3f(l_lenghAxis, 0.0f, 0.0f);
            Gl.glVertex3f(l_lenghAxis - l_flecheW, -l_flecheH, 0.0f);
            Gl.glEnd();

            // axe Y
            Gl.glColor4f(0.0f, 1.0f, 0.0f, 1.0f);
            l_couleur[0] = 0.0f; l_couleur[1] = 1.0f; l_couleur[2] = 0.0f; l_couleur[3] = 1.0f;
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, @l_couleur);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, @l_couleur);
            l_shin = 128;
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, ref l_shin);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, @l_couleur);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, @l_couleur);
            Gl.glColor3f(0.0f, 1.0f, 0.0f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3f(0.0f, 0.0f, 0.0f);
            Gl.glVertex3f(0.0f, l_lenghAxis, 0.0f);
            Gl.glEnd();
            // pointe axe Y
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3f(0.0f - l_flecheH, l_lenghAxis - l_flecheW, 0.0f);
            Gl.glVertex3f(0.0f, l_lenghAxis, 0.0f);
            Gl.glVertex3f(0.0f + l_flecheH, l_lenghAxis - l_flecheW, 0.0f);
            Gl.glEnd();

            // axe Z
            Gl.glColor4f(0.0f, 0.0f, 1.0f, 1.0f);
            l_couleur[0] = 0.0f; l_couleur[1] = 0.0f; l_couleur[2] = 1.0f; l_couleur[3] = 1.0f;
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, @l_couleur);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, @l_couleur);
            l_shin = 128;
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, ref l_shin);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, @l_couleur);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, @l_couleur);
            Gl.glColor3f(0.0f, 0.0f, 1.0f);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3f(0.0f, 0.0f, 0.0f);
            Gl.glVertex3f(0.0f, 0.0f, l_lenghAxis);
            Gl.glEnd();
            // pointe axe Z
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3f(l_flecheH, 0.0f, l_lenghAxis - l_flecheW);
            Gl.glVertex3f(0.0f, 0.0f, l_lenghAxis);
            Gl.glVertex3f(-l_flecheH, 0.0f, l_lenghAxis - l_flecheW);
            Gl.glEnd();
            */
        }

        private void DrawScene()
        {

        }
    }
}
