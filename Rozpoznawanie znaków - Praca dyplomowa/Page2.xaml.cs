using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using WMPLib;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
   {
        public LogWindow logWindow = new LogWindow();
        DataGrid lb = new DataGrid();
        SignDetector sq;
        public Page2(DataGrid lb_)
        {
            lb = lb_;
            InitializeComponent();
            if(sliderMethodSerach != null ) sliderMethodSerach.Value = Config.sliderMethodSerachValue;
        }


        

        void Start()
        {
            sq = new SignDetector();
            Image<Bgr,byte> outputImage = null;
            lb.Items.Insert(0, (""));
            lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Program rozpoczyna działanie :)"));

           


            Stopwatch sw = new Stopwatch();
            sw.Start();



            if (Config.ofd.FileName != "" && Config.isVideo == false) sq.FindSignsCandidats(new Image<Bgr, byte>(Config.ofd.FileName), false);
            else
            {


               

                Config.captureVideo = new VideoCapture(Config.ofd.FileName);
                sq.FindSignsCandidats(Config.captureVideo.QueryFrame().ToImage<Bgr, byte>());
            }

            Config.numberOfCandidats = sq.numerSignCandidatsEnd;
            outputImage = sq.drawRectangleSigns(sq.outputSigns, sq.inputImageFiltered);


            var page3 = new Page3(this.lb, outputImage, sq.outputSigns);
            this.NavigationService.Navigate(page3);




            sw.Stop();
            
            lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Czas wykonania " + sw.ElapsedMilliseconds + "milisekund"));
            page3.imageControl.Source = Config.ConvertBitmapToBitmapImage(Config.DrawTextOnImage(outputImage, "Czas wykonania: " + sw.ElapsedMilliseconds + " ms -" + " Rozpoznane znaki: " + String.Join(" , ", sq.outputSigns.Select(i => i.Name).ToList())).Bitmap);


            if(Config.ofd.FileName != "" && Config.isVideo == true)
            {
                var player = new WindowsMediaPlayer();
                var clip = player.newMedia(Config.ofd.FileName);
                page3.label_TimeTotal.Content =  "00:00:00" + "/" + TimeSpan.FromSeconds(clip.duration).ToString();
            }
            else
            {
                page3.sliderVideo.Visibility = Visibility.Hidden;
                page3.label_TimeTotal.Visibility = Visibility.Hidden;
            }

            Config.candidats = sq.detectedCandidats;

           


        }

        public void AddLog()
        {
            foreach (var item in sq.outputSigns)
            {
                lb.Items.Insert(0, "Punkty: " + item.Score);
                lb.Items.Insert(0, "Znak: " + item.Name);
                lb.UpdateLayout();

            }
        }


        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1(lb));
            Config.ofd = new Microsoft.Win32.OpenFileDialog();
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Config.sliderMethodSerachValue = (int)sliderMethodSerach.Value;

                Start();
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + "  Znaleziono " + sq.numerSignCandidats + " kandydatów do rozpozania"));
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Odrzucono " + (sq.numerSignCandidats - sq.numerSignCandidatsEnd) + " kandydatów"));
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Pozostało " + sq.numerSignCandidatsEnd + " kandydatów"));
                AddLog();
                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Program ukonczył rozpoznawanie :)"));
                lb.ScrollIntoView(lb.Items[0]);
                Config.UpdateLog(lb);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Wystąpił błąd sprawdź - Optymalizacje Rozmiaru: " + ex.Message, "Błąd !");
            }
        }

        private void sliderMethodSerach_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(sliderMethodSerach.Value == 1)
            {
                Config.isShapeDetection = true;
                Config.isColorDetection = false;
            }
            if (sliderMethodSerach.Value == 3)
            {
                Config.isColorDetection = true;
                Config.isShapeDetection = false;
            }
            if (sliderMethodSerach.Value == 2)
            {
                Config.isColorDetection = true;
                Config.isShapeDetection = true;
            }

            sliderMethodSerach.Value = (int)sliderMethodSerach.Value;

        }
    }
}
