using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WMPLib;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page3 : Page
   {
        System.Windows.Controls.DataGrid lb = new System.Windows.Controls.DataGrid();
        Image<Bgr, byte> inputImage;
        List<SignTraffic> sq;
        Stopwatch sw = new Stopwatch();
        Task t2 = null;

        SignDetector sq_ = new SignDetector();
        ImageViewer iw = new ImageViewer();

        int addedMilisecounds = 0;


        System.Windows.Forms.Timer timerVideo = new System.Windows.Forms.Timer();
        int FPS = 30;
        Emgu.CV.UI.ImageViewer iwVideo = new Emgu.CV.UI.ImageViewer();

        
        Mat frame = null;
        private void TimerVideo_Tick(object sender, EventArgs e)
        {
            int Currentmiliseconds = (int)sw.Elapsed.TotalMilliseconds + addedMilisecounds;


            frame = Config.captureVideo.QueryFrame();
            iwVideo.Image = frame;
            label_Time.Content = String.Format("Czas: {0:hh\\:mm\\:ss}", new TimeSpan(0,0,0,0,Currentmiliseconds));
            if (((int)(sw.ElapsedMilliseconds / 1000) % 2) != 0) img_control.Visibility = Visibility.Visible;
            else img_control.Visibility = Visibility.Hidden;



            if (!Config.onEnter)
            {
                var totalVideoTime = TimeSpan.Parse(label_TimeTotal.Content.ToString().Split('/')[1]);

                var timerVideo_ = Convert.ToInt32((Currentmiliseconds * 100) / totalVideoTime.TotalMilliseconds);


                sliderVideo.Value =  timerVideo_;
                label_TimeTotal.Content = String.Format("{0:hh\\:mm\\:ss}", new TimeSpan(0,0,0,0, Currentmiliseconds)) + "/" + String.Format("{0:hh\\:mm\\:ss}", totalVideoTime);
            }
            
            if(Currentmiliseconds >= TimeSpan.Parse(label_TimeTotal.Content.ToString().Split('/')[1]).TotalMilliseconds)
            {
                StopVideo();
            }       

            if (t2 == null)
            {
                t2 = Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(Config.videoFrameSeconds));





                    Start(frame);
                    t2 = null;
                });
            }
        }




        void Start(Mat signFrame)
       {

           

                Image<Bgr, byte> outputImage = null;
                Stopwatch sw = new Stopwatch();
                sw.Start();

            sq_.FindSignsCandidats(new Image<Bgr, byte>(signFrame.Bitmap), false);
            Config.numberOfCandidats = sq_.numerSignCandidatsEnd;
            outputImage = sq_.drawRectangleSigns(sq_.outputSigns, sq_.inputImageFiltered);
            Config.candidats = sq_.detectedCandidats;





            System.Windows.Application.Current.Dispatcher.InvokeAsync(
           new Action(() =>
           {

               lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Program rozpoczyna działanie :)"));
               DataTable dt = new DataTable();
               DataRow dr;
               dt.Columns.Add("Rozpoznane znaki", typeof(BitmapImage));
               int l = 0;
               bool isEnd = false;
               foreach (var item in sq_.outputSigns.OrderByDescending(i => i.Score).ToList())
               {
                   dr = dt.NewRow();
                   var sign = Config.listModelSigns.Where(i => i.Name == item.Name).ToList().Last().imgSign;
                   dr[0] = Config.ConvertBitmapToBitmapImage(sign.Bitmap);
                   dt.Rows.InsertAt(dr, 0);



                   if (Config.LearningMode)
                   {
                       l = 0;

                       while (!isEnd)
                       {
                           
                               int index = item.Name.IndexOf(" (");
                               if (index != -1)
                               {
                               if (!File.Exists(Directory.GetCurrentDirectory() + @"\Signs\" + item.Name.Substring(0, index) + " (" + l + ")" + ".png"))
                               {
                                   item.imgSign.Bitmap.Save(Directory.GetCurrentDirectory() + @"\Signs\" + item.Name.Substring(0, index) + " (" + l + ")" + ".png");
                                   isEnd = true;

                               }
                               else
                               {
                                   l++;
                               }

                           }
                               else
                               {
                                   item.imgSign.Bitmap.Save(Directory.GetCurrentDirectory() + @"\Signs\" + item.Name + " (" + l + ")" + ".png", ImageFormat.Png);
                                   isEnd = true;
                           }

                       }

                       isEnd = false;
                   }
                   
               }

               sw.Stop();





               if (sq_.outputSigns.Count != 0)
               {

                   this.SignBox.ItemsSource = dt.DefaultView;
                   SignBox.UpdateLayout();
                   imageControl.Source = Config.ConvertBitmapToBitmapImage(outputImage.Bitmap);
                   label_countSigns.Content = sq_.outputSigns.Count.ToString();
                   iwVideo.Image = outputImage.Mat;
                   sq = sq_.outputSigns;
                   


               }
               label_countCandidats.Content = Config.numberOfCandidats.ToString();
               lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Czas wykonania " + sw.ElapsedMilliseconds + "milisekund"));
               lb.Items.Insert(0, (""));
           }));

                
            

        }


        public Page3(System.Windows.Controls.DataGrid lb_, Image<Bgr, byte> inputImage, List<SignTraffic> sq)
        {
            lb = lb_;
            this.inputImage = inputImage;
            this.sq = sq;

            InitializeComponent();

            if (Config.isVideo)
            {
                timerVideo.Tick += new EventHandler(TimerVideo_Tick);
                timerVideo.Interval = 1000 / FPS;

            }







            if (label_countSigns != null) label_countSigns.Content = sq.Count.ToString();
            if (label_countCandidats != null) label_countCandidats.Content = Config.numberOfCandidats.ToString();
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add("Rozpoznane znaki", typeof(BitmapImage));

            foreach (var item in sq.OrderByDescending(i => i.Score).ToList())
            {
                dr = dt.NewRow();
                var sign = Config.listModelSigns.Where(i => i.Name == item.Name).ToList().Last().imgSign;
                dr[0] = Config.ConvertBitmapToBitmapImage(sign.Bitmap);
                dt.Rows.InsertAt(dr,0);
            }

            this.SignBox.ItemsSource = dt.DefaultView;
            Config.candidats = sq_.detectedCandidats;
            SignBox.UpdateLayout();


        }


        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            timerVideo.Stop();
            this.NavigationService.Navigate(new Page1(lb));
            Config.ofd = new Microsoft.Win32.OpenFileDialog();


          

        }


        private void btn_lupa_Click(object sender, RoutedEventArgs e)
        {



            if (Config.isVideo)
            {
                iw.MaximizeBox = true;



                if (imageControl.Dispatcher.CheckAccess())
                {
                    iw = new ImageViewer();
                    var bmp = Config.ConvertBitmapImageToBitmap((imageControl.Source as BitmapImage));
                    iw.Image = new Image<Bgr, byte>(bmp);
                    iw.Text = "Przewiń w góre/dół aby powiększyć/pomniejszyć";
                    iw.Show();

                }
                else
                {

                    imageControl.Dispatcher.BeginInvoke(
               DispatcherPriority.Background,
               new Action(() =>
               {
                   var bmp = Config.ConvertBitmapImageToBitmap((imageControl.Source as BitmapImage));
                   iw.Image = new Image<Bgr, byte>(bmp);
                   iw.Text = "Przewiń w góre/dół aby powiększyć/pomniejszyć";
                   iw.Show();
               }));


                }


            }
            else
            {
                iw = new ImageViewer();
                iw.Image = inputImage;
                iw.Text = "Przewiń w góre/dół aby powiększyć/pomniejszyć";
                iw.Show();
            }

        }



        public void StopVideo()
        {

            if (Config.isVideo)
            {
                var totalVideoTime = TimeSpan.Parse(label_TimeTotal.Content.ToString().Split('/')[1]);


                iwVideo.Close();
                iwVideo = new ImageViewer();
                iwVideo.Text = "Video";

                sw.Stop();
                sw = new Stopwatch();
                timerVideo.Stop();
                label_Time.Content = "Czas: 00:00:00";
                timerVideo = new System.Windows.Forms.Timer();
                timerVideo.Tick += new EventHandler(TimerVideo_Tick);
                timerVideo.Interval = 1000 / FPS;
                Config.captureVideo = new VideoCapture(Config.ofd.FileName);

                sliderVideo.Value = 0;

                label_TimeTotal.Content = "00:00:00/" + totalVideoTime;

                addedMilisecounds = 0;
                if (File.Exists((Directory.GetCurrentDirectory() + @"\Video_Colors\stop.png"))) img_control.Visibility = Visibility.Visible; img_control.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Video_Colors\stop.png"));
            }

        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            StopVideo();
        }

        private void btn_pause__Click(object sender, RoutedEventArgs e)
        {
            if (Config.isVideo)
            {

                timerVideo.Stop();
                sw.Stop();

                if (File.Exists((Directory.GetCurrentDirectory() + @"\Video_Colors\pause.png"))) img_control.Visibility = Visibility.Visible; img_control.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Video_Colors\pause.png"));
            }

        
    }



        private void btn_AllSigns_Click(object sender, RoutedEventArgs e)
        {
            if (sq != null)
            {
                foreach (var item in sq.OrderByDescending(i => i.Score).ToList())
                {
                    ImageViewer i = new ImageViewer();
                    i.Image = item.imgSign;
                    i.Text = item.Name + " - Punkty:" + item.Score;
                    i.Show();

                }
            }


        }

        private void btn_play__Click(object sender, RoutedEventArgs e)
        {
            if (Config.isVideo)
            {

               

                if (!iwVideo.Visible && timerVideo.Enabled)
                {
                    timerVideo.Stop();
                    iwVideo = new ImageViewer();
                    iwVideo.Text = "Video";
                    iwVideo.Show();
                    timerVideo.Start();
                }
                else
                {
                    sw.Start();
                    iwVideo.Text = "Video";
                    iwVideo.Show();

                    Start(Config.captureVideo.QueryFrame());


                }
                if (!timerVideo.Enabled) timerVideo.Start();
                if (File.Exists((Directory.GetCurrentDirectory() + @"\Video_Colors\play.png"))) img_control.Visibility = Visibility.Visible; img_control.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Video_Colors\play.png"));

            }
        }

        private void btn_AllCandidats_Click(object sender, RoutedEventArgs e)
        {
            if (Config.candidats != null)
            {
                foreach (var item in Config.candidats)
                {
                    ImageViewer i = new ImageViewer();
                    i.Image = item.imgSign;
                    i.Text = "Kandydat";
                    i.Show();

                }
            }

        }

        public void AddLog()
        {
            foreach (var item in sq_.outputSigns)
            {
                lb.Items.Insert(0, "Punkty: " + item.Score);
                lb.Items.Insert(0, "Znak: " + item.Name);
                lb.UpdateLayout();

            }
        }

        private void button_refresh_Click(object sender, RoutedEventArgs e)
        {

            try
            { 


            if (Config.isVideo == false)
            {

                sq_ = new SignDetector();
                Image<Bgr, byte> outputImage = null;
                lb.Items.Insert(0, (""));
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Program rozpoczyna działanie :)"));




                Stopwatch sw = new Stopwatch();
                sw.Start();



                if (Config.ofd.FileName != "" && Config.isVideo == false) sq_.FindSignsCandidats(new Image<Bgr, byte>(Config.ofd.FileName), false);
                else
                {
                    Config.captureVideo = new VideoCapture(Config.ofd.FileName);
                    sq_.FindSignsCandidats(Config.captureVideo.QueryFrame().ToImage<Bgr, byte>());
                }

                Config.numberOfCandidats = sq_.numerSignCandidatsEnd;
                outputImage = sq_.drawRectangleSigns(sq_.outputSigns, sq_.inputImageFiltered);


                var page3 = new Page3(this.lb, outputImage, sq_.outputSigns);
                this.NavigationService.Navigate(page3);

                page3.imageControl.Source = Config.ConvertBitmapToBitmapImage(Config.DrawTextOnImage(outputImage, "Czas wykonania: " + sw.ElapsedMilliseconds + " ms -" + " Rozpoznane znaki: " + String.Join(" , ", sq.Select(i => i.Name).ToList())).Bitmap);

                page3.sliderVideo.Visibility = Visibility.Hidden;
                page3.label_TimeTotal.Visibility = Visibility.Hidden;

                Config.candidats = sq_.detectedCandidats;

                sw.Stop();
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Czas wykonania " + sw.ElapsedMilliseconds + "milisekund"));


                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + "  Znaleziono " + sq_.numerSignCandidats + " kandydatów do rozpozania"));
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Odrzucono " + (sq_.numerSignCandidats - sq_.numerSignCandidatsEnd) + " kandydatów"));
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Pozostało " + sq_.numerSignCandidatsEnd + " kandydatów"));
                AddLog();
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Program ukonczył rozpoznawanie :)"));
                lb.ScrollIntoView(lb.Items[0]);
                Config.UpdateLog(lb);
            }


            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Wprowadzone ustawienia sa nieprawidłowe. Sprawdź - Optymalizacje Rozmiaru: " + ex.Message, "Błąd !");
            }
        }

      

        private void sliderVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (Config.onEnter)
            {
                var totalTime = TimeSpan.Parse(label_TimeTotal.Content.ToString().Split('/')[1]);
                Config.captureVideo.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosMsec, (int)(totalTime.TotalMilliseconds * sliderVideo.Value / 100));
                addedMilisecounds = (int)(totalTime.TotalMilliseconds * sliderVideo.Value / 100);
                sw.Reset();
                sw.Start();
            }
        }

      

        private void sliderVideo_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Config.onEnter = false;

        }

        private void sliderVideo_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Config.onEnter = true;

        }

        private void button_maches_Click(object sender, RoutedEventArgs e)
        {
            int l = 0;
            Random cr = new Random();
            ImageViewer iw = new ImageViewer();
            var imgMaches = new Mat();
            foreach (var item in sq.OrderByDescending(i => i.Score).ToList())
            {
                imgMaches = new Mat();
                iw = new ImageViewer();
                Emgu.CV.Features2D.Features2DToolbox.DrawMatches(Config.listModelSigns.Where(i => i.Name == item.Name).ToList().Last().imgSign, Config.listModelSigns.Where(i => i.Name == item.Name).ToList().Last().KeyPoints, item.imgSign, item.KeyPoints, item.Match, imgMaches, new MCvScalar(cr.Next(0,255), cr.Next(0, 255), cr.Next(0, 255),4), new MCvScalar(cr.Next(0, 255), cr.Next(0, 255), cr.Next(0, 255),4),null, Emgu.CV.Features2D.Features2DToolbox.KeypointDrawType.Default);
                iw.Text = "Znalezione dopasowania" + " - Punkty: " + item.Score + " - Maska: " + item.TypeSign.ToString() + " - Keypoints: " + item.KeyPoints.Size + " - Algorytm: " + Config.algorithmType.ToString() + " - Typ znaku: " + item.TypeSign.ToString();
                iw.Image = imgMaches;
               // System.Drawing.Rectangle rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, Config.listModelSigns.Where(i => i.Name == item.Name).ToList().Last().imgSign.Size);
               // PointF[] points_ = new PointF[]
               //{
               //       new PointF(rect.Left, rect.Bottom),
               //       new PointF(rect.Right, rect.Bottom),
               //       new PointF(rect.Right, rect.Top),
               //       new PointF(rect.Left, rect.Top)
               //};
               // points_ = CvInvoke.PerspectiveTransform(points_, item.Homography);
               // System.Drawing.Point[] points = Array.ConvertAll<PointF, System.Drawing.Point>(points_, System.Drawing.Point.Round);
               // using (VectorOfPoint vp_ = new VectorOfPoint(points))
               // {
               //     CvInvoke.Polylines(imgMaches, vp_, true, new MCvScalar(cr.Next(0, 255), cr.Next(0, 255), cr.Next(0, 255), 5));
               // }
                iw.Show();

            }
        }
    }
}
