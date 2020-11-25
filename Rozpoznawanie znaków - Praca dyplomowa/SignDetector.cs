using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using System.Windows.Forms;
using System.Diagnostics;
using Emgu.CV.XFeatures2D;
using System.Windows.Media;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    class SignDetector
    {
        public List<SignTraffic> detectedCandidats = null;
        private ImageViewer iw = new ImageViewer();
        public Image<Bgr, byte> inputImageFiltered;
        private Image<Bgr, byte> inputImageCopy;
        public int numerSignCandidats = 0;
        public int numerSignCandidatsEnd = 0;
        public static string modelNameDetecting = "";
        public static int size = Config.imageSize;
        public List<SignTraffic> outputSigns = null; // Rozpoznane znaki :)

        private Image<Gray, byte>[] FilterSign(Image<Bgr, byte> inputImage, int kernelSize = 5, double alfa = 2, int beta = 1, int colorSigma = 1, int spaceSigma = 1, bool testMode = false) // poprawa kontrastu usunięcie szumu
        {
         
        
            inputImage = inputImage.Resize(800, 480, Inter.Area);
            CvInvoke.GaussianBlur(inputImage, inputImage, new Size(1, 1), 1.3, 1.3);

            inputImage.Save("po filtrtacji.jpg");
            


            inputImageCopy = inputImage.Clone();
            if (Config.isFiltering) inputImage._GammaCorrect(1.8d);
            Image<Gray, byte>[] grayChanels = inputImage.SmoothBilatral(kernelSize, colorSigma, spaceSigma).Convert<Ycc, byte>().Split();
            var ycc = new Image<Ycc, byte>(grayChanels);
            var bgr = ycc.Convert<Bgr, byte>();

            if (Config.isFiltering) inputImage._Mul(2.7d);
            inputImageFiltered = inputImage;
            bgr.Mat.ConvertTo(bgr.Mat, Emgu.CV.CvEnum.DepthType.Default, alfa, beta);
            var hsv = bgr.Convert<Hsv, byte>();




            return hsv.Split();

        }







        private List<SignTraffic> RemoveBadCandidates(List<SignTraffic> detectedCandidates)
        {


            List<SignTraffic> goodCandidates = new List<SignTraffic>();
            List<Rectangle> rectangles = new List<Rectangle>();
            List<SignType> st = new List<SignType>();
            short l = 0;
            short c = 0;
            

            foreach (var item in detectedCandidates)
            {
                for (int i = 0; i < item.SignCounturs.Size; i++)
                {
                    var rectangle_ = CvInvoke.BoundingRectangle(item.SignCounturs[i]);
                    if (rectangle_.Height < 40 || rectangle_.Width < 40) continue;
                    if (rectangle_.Width / rectangle_.Height > 3 || rectangle_.Height / rectangle_.Width > 3) continue;

                    Rectangle rectToRemove = Rectangle.Empty;
                    rectangles.ForEach(other =>
                    {
                        if (other.IntersectsWith(rectangle_))
                        {
                            rectangle_ = CvInvoke.cvMaxRect(rectangle_, other);
                            rectToRemove = other;
                        }
                    });
                    if (!rectToRemove.IsEmpty) rectangles.Remove(rectToRemove);
                    Rectangle paddedRoi = new Rectangle(rectangle_.Location, rectangle_.Size);
                    rectangles.Add(paddedRoi);
                    st.Add(item.TypeSign);
                    c++;

                }
            }

            numerSignCandidats = c;

            rectangles.ForEach(paddedRoi =>
            {
                try
                {
                    Mat f = new Mat();
                    VectorOfKeyPoint kp = new VectorOfKeyPoint();

                    switch (Config.algorithmType)
                    {
                        case Config.Algorithm.KAZE:
                            Config.kaze.DetectAndCompute(inputImageFiltered.Copy(paddedRoi).Resize(size,size, Inter.Area), null, kp, f, false);
                            break;
                        case Config.Algorithm.SIFT:
                            Config.sift.DetectAndCompute(inputImageFiltered.Copy(paddedRoi).Resize(size,size, Inter.Area), null, kp, f, false);
                            break;
                        case Config.Algorithm.SURF:
                            Config.surf.DetectAndCompute(inputImageFiltered.Copy(paddedRoi).Resize(size, size, Inter.Area), null, kp, f, false);
                            break;
                        default:
                            break;
                    }
                    SignTraffic newSign = new SignTraffic { imgSign = inputImageFiltered.Copy(paddedRoi).Mat, TypeSign = st[l], Rectangle = paddedRoi, imgSignGray = inputImageFiltered.Copy(paddedRoi).Convert<Gray, byte>(), imgNonResized = inputImageFiltered.Copy(paddedRoi).Mat, Features = f, KeyPoints = kp };
                    goodCandidates.Add(newSign);
                    //surf.UpdateDescriptors(goodCandidates[l]);
                    l++;

                    CvInvoke.Resize(newSign.imgSign.Clone(), newSign.imgSign, new Size(size, size)); // Dopasowanie do wzorca

                    //iw = new ImageViewer();
                    //iw.Image = newSign.imgSign;
                    //iw.Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            numerSignCandidatsEnd = l;
            return goodCandidates;
        }



        public static void FindMatch(Mat modelImage, Mat observedImage, VectorOfKeyPoint modelKeyPoints, VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, out long score, SignTraffic model, SignTraffic candidate)
        {
            int k = 2;
            double uniquenessThreshold = Config.uniquenessThreshold; // 0,6
            homography = null;

            modelKeyPoints = model.KeyPoints;
            observedKeyPoints = candidate.KeyPoints;

            matches = new VectorOfVectorOfDMatch();
            score = 0;
            mask = null;

            if (CvInvoke.PSNR(modelImage, observedImage) > Config.PSNR) // PNSR
            {

                using (UMat uModelImage = modelImage.GetUMat(AccessType.Fast))
                using (UMat uObservedImage = observedImage.GetUMat(AccessType.Fast))
                {
                    using (var ip = new Emgu.CV.Flann.KdTreeIndexParams())
                    using (var sp = new SearchParams())
                    using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
                    {
                        matcher.Add(model.Features);

                        matcher.KnnMatch(candidate.Features, matches, k, null);
                        mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(255));
                        Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                        
                        for (int i = 0; i < matches.Size; i++)
                        {
                            if (mask.GetData(i)[0] == 0) continue;
                            foreach (var e in matches[i].ToArray())
                                ++score;
                        }



                        int nonZeroCount = CvInvoke.CountNonZero(mask);
                        if (nonZeroCount >= 4)
                        {
                            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);
                            if (nonZeroCount >= 4)
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                        }
                    }


                }

                Console.WriteLine("Trwa rozpoznawanie znaku: " + (modelNameDetecting = model.Name));
            }
        }



        private List<SignTraffic> DetectSignFromCandidats(List<SignTraffic> detectedCandidats)
        {


            KdTreeIndexParams ip = new KdTreeIndexParams();
            SearchParams sp = new SearchParams();
            var matcher = new FlannBasedMatcher(ip, sp);
            VectorOfVectorOfDMatch match = new VectorOfVectorOfDMatch();
            int l = 0;
            Mat homography = null;
            outputSigns = new List<SignTraffic>();

            foreach (var candidate in detectedCandidats)
            {
                //if (l < 16)
                //{
                    int bestscore = 0;
                    int modelIndex = 0;

                      for (int i = 0; i < Config.listModelSigns.Count; i++)
                    {
                        long score = 0;
                        Mat mask = new Mat();

                        FindMatch(Config.listModelSigns[i].imgSign, candidate.imgSign, Config.listModelSigns[i].KeyPoints, candidate.KeyPoints, match, out mask, out homography, out score, Config.listModelSigns[i], candidate);

                        if (score > bestscore)
                        {
                            bestscore = (int)score;
                        }

                        if (homography != null)
                        {
                            outputSigns.Add(candidate);
                            outputSigns.Last().Name = Config.listModelSigns[modelIndex].Name;
                            outputSigns.Last().Score = bestscore;
                            outputSigns.Last().Rectangle = candidate.Rectangle;
                            outputSigns.Last().TypeSign = candidate.TypeSign;
                            outputSigns.Last().Homography = homography;
                            outputSigns.Last().Mask = mask;


                        }

                        modelIndex++;

                    }
                //}
                //else
                //{
                //    break;
                //}


                l++;
            }


                return null;
            
            }





        public List<SignTraffic> FindTrianglesCandidats(Image<Bgr, byte> inputImage)
        {

            List<SignTraffic> goodTriangleCandidates = new List<SignTraffic>();
            Mat edgesCanny = new Mat();
            CvInvoke.Canny(inputImage.Convert<Gray, byte>().ThresholdToZero(new Gray(120)).Erode(1).Dilate(1), edgesCanny, 180, 120, 5);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edgesCanny, contours, null, RetrType.List, ChainApproxMethod.ChainApproxTc89Kcos);
            List<Triangle2DF> triangleSigns = new List<Triangle2DF>();
            int i = 0;

            foreach(var item in Enumerable.Range(1, contours.Size))
            {
                VectorOfPoint counturs = contours[i];
                VectorOfPoint aproxCounturs = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(counturs, aproxCounturs, CvInvoke.ArcLength(counturs, true) * 0.05, true);
                if (CvInvoke.ContourArea(aproxCounturs, false) > 250)
                {
                    if (aproxCounturs.Size == 3)
                    {


                        var pointsTable = aproxCounturs.ToArray();
                        triangleSigns.Add(new Triangle2DF(
                           pointsTable[0],
                           pointsTable[1],
                           pointsTable[2]
                           ));

                        var rectangle = CvInvoke.BoundingRectangle(aproxCounturs);
                        Mat f = new Mat();
                        VectorOfKeyPoint kp = new VectorOfKeyPoint();

                        switch (Config.algorithmType)
                        {
                            case Config.Algorithm.KAZE:
                                Config.kaze.DetectAndCompute(inputImage.Copy(rectangle).Resize(size, size, Inter.Area), null, kp, f, false);
                                break;
                            case Config.Algorithm.SIFT:
                                Config.sift.DetectAndCompute(inputImage.Copy(rectangle).Resize(size, size, Inter.Area), null, kp, f, false);
                                break;
                            case Config.Algorithm.SURF:
                                Config.surf.DetectAndCompute(inputImage.Copy(rectangle).Resize(size,size, Inter.Area), null, kp, f, false);
                                break;
                            default:
                                break;

                        }

                        numerSignCandidats++;
                        SignTraffic newSign = new SignTraffic { imgSign = inputImage.Copy(rectangle).Mat, TypeSign = SignType.TRIANGLE, Rectangle = rectangle, imgSignGray = inputImage.Copy(rectangle).Convert<Gray, byte>(), imgNonResized = inputImage.Copy(rectangle).Mat, Features = f, KeyPoints = kp, triangle = triangleSigns.Last() };
                        goodTriangleCandidates.Add(newSign);
                        CvInvoke.Resize(newSign.imgSign.Clone(), newSign.imgSign, new Size(size,size));

                    }


                }

                i++;
            }
            return goodTriangleCandidates;

        }





        public List<SignTraffic> FindCirclesCandidats(Image<Bgr, byte> inputImage)
        {

            List<SignTraffic> goodCircleCandidates = new List<SignTraffic>();
            try
            {
                UMat imageGPU = new UMat();
                CvInvoke.CvtColor(inputImage, imageGPU, ColorConversion.Bgr2Gray);
                UMat performsDown = new UMat();
                CvInvoke.PyrDown(imageGPU, performsDown);
                CvInvoke.PyrUp(performsDown, imageGPU);
                CircleF[] circlesSign = CvInvoke.HoughCircles(imageGPU, HoughType.Gradient, 20, 20, 180.0, 120, 15, 300);
                int l = 0;
                foreach (var item in circlesSign)
                {
                    var rectangle = new Rectangle((int)(item.Center.X - item.Radius), (int)(item.Center.Y - item.Radius), (int)(2 * item.Radius), (int)(2 * item.Radius));
                    Mat f = new Mat();
                    VectorOfKeyPoint kp = new VectorOfKeyPoint();

                    switch (Config.algorithmType)
                    {
                        case Config.Algorithm.KAZE:
                            Config.kaze.DetectAndCompute(inputImage.Copy(rectangle).Resize(size, size, Inter.Area), null, kp, f, false);
                            break;
                        case Config.Algorithm.SIFT:
                            Config.sift.DetectAndCompute(inputImage.Copy(rectangle).Resize(size, size, Inter.Area), null, kp, f, false);
                            break;
                        case Config.Algorithm.SURF:
                            Config.surf.DetectAndCompute(inputImage.Copy(rectangle).Resize(size, size, Inter.Area), null, kp, f, false);
                            break;
                        default:
                            break;

                    }

                    numerSignCandidats++;
                    //inputImageFiltered.Copy(rectangle).Save(l + ".jpg");

                    SignTraffic newSign = new SignTraffic { imgSign = inputImage.Copy(rectangle).Mat, TypeSign = SignType.CIRCLE, Rectangle = rectangle, imgSignGray = inputImage.Copy(rectangle).Convert<Gray, byte>(), imgNonResized = inputImage.Copy(rectangle).Mat, Features = f, KeyPoints = kp, circle = item };
                    goodCircleCandidates.Add(newSign);
                    CvInvoke.Resize(newSign.imgSign.Clone(), newSign.imgSign, new Size(size, size));

                    if (l > 5) break;
                    l++;


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Podczas wykrywania kształtów wystąpił błąd: " + ex.Message);
            }

            numerSignCandidatsEnd += goodCircleCandidates.Count;

            return goodCircleCandidates;
        }



        public List<SignTraffic> FindSignsCandidats(Image<Bgr, byte> inputImage, bool testMode = false) // Find candidats from colors or shapes
        {
            size = Config.imageSize;

            Stopwatch s = new Stopwatch();
            s.Start();

            

            numerSignCandidats = 0;
            numerSignCandidatsEnd = 0;
            detectedCandidats = new List<SignTraffic>();
            Image<Gray, byte>[] inpt = null;

            List<SignTraffic> detectedCandidats_ = new List<SignTraffic>();
 
            Task t0 = Task.Run(() => {
                inpt = FilterSign(inputImage, 5, 3, 1, 10, 10, true); // Nie ruszać
            });



            t0.Wait();
            s.Stop();
            Console.WriteLine("Filtered time milisecounds: " + s.ElapsedMilliseconds);

            Task t1 = null;
            Task t1_1 = null;
            List<SignTraffic> shapeCandidats = new List<SignTraffic>();
            List<SignTraffic> triangleCandidats = new List<SignTraffic>();
            if (Config.isShapeDetection)
            {
                var input = new Image<Bgr, byte>(inputImageFiltered.Bitmap);

                t1 = Task.Run(() => {
                    shapeCandidats = FindCirclesCandidats(input);
                });

                t1_1 = Task.Run(() => {
                    triangleCandidats = FindTrianglesCandidats(input);
                });

            }


            if (Config.isColorDetection)
            {
                // red mask
                Image<Gray, byte> redMask =
                    inpt[0].InRange(new Gray(0), new Gray(10))
                    .Or(inpt[0].InRange(new Gray(172), new Gray(255)))
                    .And(inpt[1].InRange(new Gray(132), new Gray(255)));

                // blue mask
                Image<Gray, byte> blueMask =
                    inpt[0].InRange(new Gray(100), new Gray(140))
                    .And(inpt[1].InRange(new Gray(185), new Gray(255)))
                    .And(inpt[2].InRange(new Gray(120), new Gray(255)));

                // yellow mask
                Image<Gray, byte> yellowMask =
                   inpt[0].InRange(new Gray(15), new Gray(100))
                   .And(inpt[1].InRange(new Gray(100), new Gray(255)))
                   .And(inpt[2].InRange(new Gray(120), new Gray(255)));


                Config.Masks = new List<Mat>() { redMask.Mat, yellowMask.Mat, blueMask.Mat };
                //testMode = true;

                if (testMode)// test mode
                {
                    iw = new ImageViewer();


                    iw.Image = redMask;
                    iw.Text = "Red binary";
                    iw.Show();
                    iw = new ImageViewer();
                    iw.Image = blueMask;
                    iw.Text = "Blue binary";
                    iw.Show();
                    iw = new ImageViewer();
                    iw.Image = yellowMask;
                    iw.Text = "Yellow binary";
                    iw.Show();
                }

                VectorOfVectorOfPoint countursR = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(redMask.Erode(1).Dilate(1).Canny(1, 1), countursR, null, RetrType.External, ChainApproxMethod.ChainApproxTc89Kcos);
                VectorOfVectorOfPoint countursB = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(blueMask.Erode(1).Dilate(1).Canny(1, 1), countursB, null, RetrType.External, ChainApproxMethod.ChainApproxTc89Kcos);
                VectorOfVectorOfPoint countursY = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(yellowMask.Erode(1).Dilate(1).Canny(1, 1), countursY, null, RetrType.External, ChainApproxMethod.ChainApproxTc89Kcos);

                if (Config.isRedChecked) detectedCandidats_.Add(new SignTraffic() { SignCounturs = countursR, SignMask = redMask.Copy(), TypeSign = SignType.RED });
                if (Config.isBlueChecked) detectedCandidats_.Add(new SignTraffic() { SignCounturs = countursB, SignMask = blueMask.Copy(), TypeSign = SignType.BLUE });
                if (Config.isYellowChecked) detectedCandidats_.Add(new SignTraffic() { SignCounturs = countursY, SignMask = yellowMask.Copy(), TypeSign = SignType.YELLOW });
            }

            if (Config.isColorDetection)
            {
                Task t2 = Task.Run(() =>
                {
                    detectedCandidats.AddRange(RemoveBadCandidates(new List<SignTraffic>() { detectedCandidats_[0] }));
                });
                Task t2_1 = Task.Run(() =>
                {
                    detectedCandidats.AddRange(RemoveBadCandidates(new List<SignTraffic>() { detectedCandidats_[1] }));
                });
                Task t2_2 = Task.Run(() =>
                {
                    detectedCandidats.AddRange(RemoveBadCandidates(new List<SignTraffic>() { detectedCandidats_[2] }));
                });
                t2.Wait();
                t2_1.Wait();
                t2_2.Wait();




            }
            Task t3 = Task.Run(() => {

                if(t1 != null && t1_1 != null)
                {
                    t1.Wait();
                    t1_1.Wait();
                    detectedCandidats.AddRange(shapeCandidats); // add optional shape candidats
                    detectedCandidats.AddRange(triangleCandidats); // add optional triangle candidats
                    Console.WriteLine("Dodatkowi kandydaci (wg. kształtu) zostali dodani");
                }

                DetectSignFromCandidats(detectedCandidats);

            });

            t3.Wait();

            return detectedCandidats;
        }

        public Image<Bgr, byte> drawRectangleSigns(List<SignTraffic> detectedSigns, Image<Bgr, byte> inputImage, bool testmode = false)
        {
            foreach (var item in detectedSigns)
            {
                switch (item.TypeSign)
                {
                    case SignType.RED:
                        CvInvoke.Rectangle(inputImage, item.Rectangle, new MCvScalar(0,0,255), 3); // Red
                        break;
                    case SignType.BLUE:
                        CvInvoke.Rectangle(inputImage, item.Rectangle, new MCvScalar(255), 3); // Blue
                        break;
                    case SignType.YELLOW:
                        CvInvoke.Rectangle(inputImage, item.Rectangle, new MCvScalar(30, 255, 255), 3); // Yellow         
                        break;
                    case SignType.CIRCLE:
                        inputImage.Draw(item.circle, new Bgr(Colors.Khaki.B, Colors.Khaki.G, Colors.Khaki.R), 3); // Circle ;
                        break;
                    case SignType.TRIANGLE:
                        inputImage.Draw(item.triangle, new Bgr(Colors.Khaki.B, Colors.Khaki.G, Colors.Khaki.R), 3); // Triangle ;
                        break;
                    default:
                        break;
                }



            }
            if (testmode)
            {
                var c = new ImageViewer();
                c.Image = inputImage;
                c.Show();
            }
            return inputImage;
        }

        

    }
}
