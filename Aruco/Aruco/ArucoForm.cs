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

namespace Aruco
{
    public partial class ArucoForm : Form
    {
        private Capture _cameraCapture;
        private CameraParameters _parameters;
        private int _markerSize;
        private Mat _cameraMatrix;
        private Mat _distortion;
        private MarkerDetector detector;
        private IList<Marker> detectedMarkers;

        public ArucoForm()
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

        
    }
}
