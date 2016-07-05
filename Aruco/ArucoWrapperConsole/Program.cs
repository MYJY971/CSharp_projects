using Aruco.Net;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArucoWrapperConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create camera parameters and load them from a YML file
            var parameters = new CameraParameters();
            parameters.ReadFromXmlFile("C:\\Stage\\Yanis\\Data\\intrinsics.yml");

            // Copy the camera matrix and distortion coefficients
            Size size;
            var cameraMatrix = new Mat(3, 3, Depth.F32, 1);
            var distortion = new Mat(1, 4, Depth.F32, 1);
            parameters.CopyParameters(cameraMatrix, distortion, out size);

            // Initialize a marker detector object and setup detection properties
            using (var detector = new MarkerDetector())
            {
                detector.ThresholdMethod = ThresholdMethod.AdaptiveThreshold;
                detector.Param1 = 7.0;
                detector.Param2 = 7.0;
                detector.MinSize = 0.04f;
                detector.MaxSize = 0.5f;
                detector.CornerRefinement = CornerRefinementMethod.Lines;

                // Detect markers in a sequence of camera images.
                var markerSize = 10;
                using (var capture = Capture.CreateCameraCapture(0))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var image = capture.QueryFrame();
                        var detectedMarkers = detector.Detect(image, cameraMatrix, distortion, markerSize);
                        foreach (var marker in detectedMarkers)
                        {
                            Console.WriteLine(marker.Id);
                        }
                    }
                }
            }

        }
    }
}
