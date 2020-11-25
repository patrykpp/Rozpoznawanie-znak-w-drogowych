using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    public class SURF_
    {
       
        //public CudaSURF cudaSURF = new CudaSURF(); // optional future

        public void UpdateLastModelDescriptors()
        {
            
                Mat f = new Mat();
                VectorOfKeyPoint kp = new VectorOfKeyPoint();

            switch (Config.algorithmType)
            {
                case Config.Algorithm.KAZE:
                    Config.kaze.DetectAndCompute(Config.listModelSigns.Last().imgSign, null, kp, f, false);
                    break;
                case Config.Algorithm.SIFT:
                    Config.sift.DetectAndCompute(Config.listModelSigns.Last().imgSign, null, kp, f, false);
                    break;
                case Config.Algorithm.SURF:
                    Config.surf.DetectAndCompute(Config.listModelSigns.Last().imgSign, null, kp, f, false);
                    break;
                default:
                    break;
            }


                Config.listModelSigns.Last().KeyPoints = kp;
                Config.listModelSigns.Last().Features = f;
            
        }
       
        
    }
}
