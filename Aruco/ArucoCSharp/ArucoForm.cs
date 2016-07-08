using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

//using ArucoCSharp;
//using OpenCV;

using System.Runtime.InteropServices;

namespace ArucoCSharp
{
    public partial class ArucoForm : Form
    {
        private static Capture _cameraCapture;
        private Image<Bgr, Byte> _imageForARCompute;
        double _nbMarkers;
        private Size _glWindowSize;
        //public  = "";
        //public string _path = "C:\\Stage\\Yanis\\CSharp_projects\\Librairies\\ARUCO\\aruco-2.0.7\\MY-Build\\aruco-test-data-2.0\\1_single\\out_cam_calibration.yml";

        /*
        [DllImport("C:\\Stage\\Yanis\\CSharp_projects\\MY_Stage\\ArucoCSharp\\Debug\\ArucoDll.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern double Add(double a, double b);//public static extern int performAR(byte[] image, int imageWidth, int imageHeight);
        */

        //[DllImport("C:\\Stage\\Yanis\\CSharp_projects\\MY_Stage\\ArucoCSharp\\Debug\\ArucoDll.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern double Add(double a, double b);

        //[DllImport("..\\..\\..\\Debug\\ArucoDll.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern double Add(double a, double b);
        //public static extern double TestAR(byte[] image, int width, int height, string path);
        //public static extern double performAR2(string path);

        public ArucoForm()
        {
            InitializeComponent();
            Run();
        }

        private void Run()
        {
            try
            {
                _cameraCapture = new Capture();

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
            Mat frame = _cameraCapture.QueryFrame();
            _imageForARCompute = new Image<Bgr, byte>(frame.Width, frame.Height);
            CvInvoke.Resize(frame, _imageForARCompute, frame.Size);
            byte[] byteImageForARCompute = _imageForARCompute.Bytes;



            //_nbMarkers =(int) Add(5, 5);//TestAR(byteImageForARCompute, _imageForARCompute.Width, _imageForARCompute.Height, _path);
            label1.Text = "" + _nbMarkers;

            imageVideo.Image = frame;
        }

        private void ImageVideo_Resize(object sender, EventArgs e)
        {
            _cameraCapture.SetCaptureProperty(CapProp.FrameWidth, 1920/*imageVideo.Size.Width*/);
            _cameraCapture.SetCaptureProperty(CapProp.FrameHeight, 1080/*imageVideo.Size.Height*/);
        }
    }
}
