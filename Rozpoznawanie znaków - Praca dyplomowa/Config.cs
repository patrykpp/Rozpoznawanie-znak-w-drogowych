using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    public static class Config
    {
        public static OpenFileDialog ofd = new OpenFileDialog();
        public static List<string> smartWelcowe = new List<string>() { "Witamy w programie :)", "Miło nam ze z nami jesteś :)", "Co u ciebie słychać ?", "Witamy :)", "No to do pracy :)", "Jaki masz humor ? :)", ":)", "Lubisz ten program ?", "Zaczynamy pracę :)", "Cześć :)", "Witaj :)", "OK. To do pracy :)", "Jaka jest dzisiaj pogoda ?", "Jak się miewasz !", "Co u Ciebie słychać !", "Miłego dnia !", "Miło cię widzieć :)" };
        public static List<SignTraffic> listModelSigns = null;
        public static double uniquenessThreshold = 0.65d;
        public static Algorithm algorithmType = Algorithm.KAZE;
        public static int numberOfCandidats = 0;
        public static bool onEnter = false;
        public static bool isRedChecked = true;
        public static bool isYellowChecked = true;
        public static bool isBlueChecked = true;

        public static double PSNR = 0d;

        public static Mat homeography = null;

        public static bool LearningMode = false; // test

        public static bool isFiltering = true;

        public static bool isVideo = false;

        public static List<SignTraffic> candidats = null;

        public static List<Mat> Masks = null;

        public static double videoFrameSeconds = 1d;

        public static Mat detectedFrame = null;

        public static bool isShapeDetection = false;

        public static bool isColorDetection = true;

        public static int sliderMethodSerachValue = 2;

        public static int imageSize = 64;


        public static System.Windows.Forms.Timer timerVideo = new System.Windows.Forms.Timer();
        public static Emgu.CV.VideoCapture captureVideo = new VideoCapture();

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bmp)
        {
            MemoryStream memory = new MemoryStream();

            bmp.Save(memory, ImageFormat.Png);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }


        public static Bitmap ConvertBitmapImageToBitmap(BitmapImage bmp)
        {
            var stream = bmp.StreamSource;
            var bitmap = new Bitmap(stream);

            return bitmap;
        }

        public enum Algorithm
        {
            KAZE = 0,
            SIFT = 1,
            SURF = 2
        }
        public static KAZE kaze = new KAZE();
        public static SURF surf = new SURF(500);
        public static SIFT sift = new SIFT();

        /// <summary>
        /// Update DataGrid log panel.
        /// </summary>
        /// <param name="dgLog"></param>
        public static void UpdateLog(DataGrid dgLog)
        {

            dgLog.UpdateLayout();
        }


        public static Image<Bgr, byte> DrawTextOnImage(Image<Bgr, byte> inputImage, string text)
        {
            CvInvoke.PutText(inputImage, text, new Point((int)(0.03 * inputImage.Height), (int)(0.03 * inputImage.Width)), Emgu.CV.CvEnum.FontFace.HersheyComplex, 0.5, new Bgr(Color.Red.B, Color.Red.G, Color.Red.R).MCvScalar,1);
            return inputImage;
        }


    }
}
