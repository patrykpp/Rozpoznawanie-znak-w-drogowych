using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    public enum SignType
    {
        RED,
        BLUE,
        YELLOW,
        CIRCLE,
        TRIANGLE
    }

    

    public class SignTraffic : Sign
    {
        public VectorOfVectorOfPoint SignCounturs;

        public System.Drawing.Rectangle Rectangle;
        public Image<Gray, byte> SignMask;
        public SignType TypeSign;

        public int Score;

        public Mat Features;


        /// <summary>
        /// 
        /// </summary>

        public VectorOfKeyPoint KeyPoints;

        public VectorOfVectorOfDMatch Match  = new VectorOfVectorOfDMatch();

        public Mat Homography  = new Mat();

        public Image<Gray, byte> imgSignGray;

        public Mat imgNonResized;

        public Mat Mask;

        public CircleF circle;

        public Triangle2DF triangle;

    }
}
