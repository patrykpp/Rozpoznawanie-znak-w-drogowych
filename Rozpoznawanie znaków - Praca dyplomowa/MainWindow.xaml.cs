using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Emgu.CV.OCR;
using Emgu.CV.ML;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Data;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Collections.Specialized;
using Emgu.CV.Util;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualBasic.Devices;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       
        Page2 page2;
        Microsoft.Win32.OpenFileDialog ofd = Config.ofd;
        int size = Config.imageSize;





        public void LoadPreferences(bool isDefaults)
        {
            try
            {
                StreamReader sr = !isDefaults ?  (new StreamReader(Directory.GetCurrentDirectory() + @"\config.rx")) : (new StreamReader(Directory.GetCurrentDirectory() + @"\defaults.rx"));
                var prefs = Regex.Split(sr.ReadToEnd(), ";" + Environment.NewLine);
                int l = 0;
                foreach (var pref in prefs)
                {
                    var p = Regex.Split(pref, " = ");
                    Console.WriteLine("Trwa ustwaianie parametru: " + p[0]);

                    switch (l)
                    {
                        case 0:
                            if(slider_Algorytm != null) slider_Algorytm.Value = Double.Parse(p[1]); // 1-3 typ algorytmu
                            break;
                        case 1:
                            if (slider_uniquenessThreshold != null) slider_uniquenessThreshold.Value = Double.Parse(p[1], System.Globalization.CultureInfo.InvariantCulture); // 0,1 - 0,9 próg unikalnosci
                            break;
                        case 2:
                            if (slider_VideoSpeed != null) slider_VideoSpeed.Value = Double.Parse(p[1], System.Globalization.CultureInfo.InvariantCulture); // 0 - 5 przetwarzanie klatek video
                            break;
                        case 3:
                            if (slider_PSNR != null) slider_PSNR.Value = Double.Parse(p[1], System.Globalization.CultureInfo.InvariantCulture); // psnr
                            break;
                        case 4:
                            if (filterBox != null) filterBox.IsChecked = (p[1] == "1") ? true : false; // auto filtracja
                            break;
                        case 5:
                            if (checkBox_yellow != null) checkBox_yellow.IsChecked = (p[1] == "1") ? true : false; // żółta maska
                            break;
                        case 6:
                            if (checkBox_blue != null) checkBox_blue.IsChecked = (p[1] == "1") ? true : false; // niebieska maska
                            break;
                        case 7:
                            if (checkBox_red != null) checkBox_red.IsChecked = (p[1] == "1") ? true : false; // czerwona maska
                            break;
                        case 8:
                             Config.sliderMethodSerachValue = int.Parse(p[1]); // psnr
                            break;
                        case 9:
                            if(sliderResize != null) sliderResize.Value = int.Parse(p[1]);
                            Config.imageSize = (int)sliderResize.Value;
                            AddModelSigns();
                            break;
                        default:
                            break;
                    }

                    l++;

                    Console.WriteLine("Ukonczono pomyslnie odczyt ustawień");
                    sr.Close();
                }
                
            }
            catch (IOException ex)
            {
                Console.WriteLine("Wystąpił problem z odczytem ustawień - kod błędu: " + ex.Message);
            }
        }


        public void SavePreferences()
        {
            try
            {
                int countPref = 10;
                string configText = "";

                for (int i = 0; i < countPref; i++)
                {
                    switch (i)
                    {
                        case 0:
                            configText += "algorythmType = " + slider_Algorytm.Value.ToString() + ";" + Environment.NewLine;
                            break;
                        case 1:
                            configText += "uniqThreshold = " + slider_uniquenessThreshold.Value.ToString().Replace(',','.') + ";" + Environment.NewLine;
                            break;
                        case 2:
                            configText += "videoSpeedDetection = " + slider_VideoSpeed.Value.ToString().Replace(',', '.') + ";" + Environment.NewLine;
                            break;
                        case 3:
                            configText += "psnrIndex = " + slider_PSNR.Value.ToString().Replace(',', '.') + ";" + Environment.NewLine;
                            break;
                        case 4:
                            configText += "autoFiltering = " + ((bool)filterBox.IsChecked ? "1" : "0") + ";" + Environment.NewLine;
                            break;
                        case 5:
                            configText += "maskYellow = " + ((bool)checkBox_red.IsChecked ? "1" : "0") + ";" + Environment.NewLine;
                            break;
                        case 6:
                            configText += "maskBlue = " + ((bool)checkBox_red.IsChecked ? "1" : "0") + ";" + Environment.NewLine;
                            break;
                        case 7:
                            configText += "maskRed = " + ((bool)checkBox_red.IsChecked ? "1" : "0") + ";" + Environment.NewLine;
                            break;
                        case 8:
                            configText += "methodSerachValue = " + Config.sliderMethodSerachValue + ";" + Environment.NewLine;
                            break;
                        case 9:
                            configText += "imageSize = " + Config.imageSize + ";" + Environment.NewLine;
                            break;
                        default:
                            break;
                    }

                }

                File.WriteAllText("config.rx", configText);
                Console.WriteLine("Ukonczono pomyslnie zapis ustawień");
                

            }
            catch (IOException ex)
            {
                Console.WriteLine("Podczas zapisu ustawień wystąpił błąd - kod błędu: " + ex.Message);
            }
        }




        public void AddElementToModelSigns()
        {

            ofd = new Microsoft.Win32.OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                
                var ex = new FileInfo(ofd.FileName);

                if (ex.Extension == ".jpg" || ex.Extension == ".JPG"|| ex.Extension == ".png" || ex.Extension == ".jpeg" || ex.Extension == ".gif" || ex.Extension == ".bmp")
                {
                    try
                    {
                        File.Copy(ofd.FileName, Directory.GetCurrentDirectory() + @"\Signs\" + ex.Name);
                        AddNewLog(DateTime.Now.ToShortTimeString() + " Dodano nowy model do bazy znaków w programie.");
                    }
                    catch (Exception ex_)
                    {
                        System.Windows.MessageBox.Show(ex_.Message);
                    }
                }
                else System.Windows.MessageBox.Show("Nieprawidłowy format pliku !", "Błąd");

            }


        }


        public void UpdatePSNR()
        {
            if (slider_PSNR != null && lb_PSNR != null)
            {
                lb_PSNR.Content = String.Format("{0:0.##}", slider_PSNR.Value);
                Config.PSNR = Convert.ToDouble(lb_PSNR.Content);
            }

        }

        public void AddNewLog(string log)
        {
            logBox.Items.Insert(0, (log + "\n"));

        }


        public void AddModelSigns()
        {

            try
            {

                int l = 1;
                FileInfo fi;
                SURF_ surf = new SURF_();

                var size = Config.imageSize;
                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Signs") == true)
                {

                    var signs = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Signs");
                    lb_BaseElements.Content = "Liczba elementów: " + signs.Length;


                    Config.listModelSigns = new List<SignTraffic>();
                    DataTable dt = new DataTable();
                    DataRow dr;
                    dt.Columns.Add("id", typeof(string));
                    dt.Columns.Add("name", typeof(string));
                    dt.Columns.Add("image", typeof(BitmapImage));

                    foreach (var item in signs)
                    {

                        Image<Bgr, byte> img = new Image<Bgr, byte>(item);
                        if (img.Size != new System.Drawing.Size(size, size)) CvInvoke.Resize(img, img, new System.Drawing.Size(size, size));
                        byte[] buffer = System.IO.File.ReadAllBytes(item);
                        MemoryStream ms = new MemoryStream(buffer);
                        BitmapImage img_ = new BitmapImage();
                        img_.BeginInit();
                        img_.StreamSource = ms;
                        img_.EndInit();
                        img_.Freeze();
                        fi = new FileInfo(item);
                        dr = dt.NewRow();
                        dr[0] = l;
                        dr[1] = fi.Name;
                        dr[2] = img_;
                        dt.Rows.Add(dr);
                        Config.listModelSigns.Add(new SignTraffic { Id = l, Name = fi.Name.Split('.')[0], LocationSign = item, imgSign = img.Mat, imgSignGray = img.Convert<Gray, byte>(), imgNonResized = new Image<Bgr, byte>(item).Mat });
                        surf.UpdateLastModelDescriptors();
                        l++;

                    }




                    dgModelSigns.ItemsSource = dt.DefaultView;

                    AddNewLog(DateTime.Now.ToShortTimeString() + " Odswieżono bazę znaków w programie.");

                }
                else
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Signs");
                    AddNewLog(DateTime.Now.ToShortTimeString() + " Utworzono brakujący folder [Signs]");

                }


            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Wystąpił błąd podczas wczytywania plików do bazy danych. Sprawdz czy folder Signs zawiera prawiłowe pliki: " + ex.Message);
            }

        }


       


        public MainWindow()
        {
            InitializeComponent();
            AddModelSigns();
            Random r = new Random();
            int test = r.Next(0, Config.smartWelcowe.Count);
            var page = new Page1(this.logBox);
            page.welcomeLabel.Content = Config.smartWelcowe[test];
            frame.NavigationService.Navigate(page);
            UpdatePSNR();


            LoadPreferences(false);
            lb_OS.Content = new ComputerInfo().OSFullName;


        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {


            SignDetector sq = new SignDetector();

            if(ofd.FileName != "") sq.FindSignsCandidats(new Image<Bgr, byte>(ofd.FileName), true);


        }

        private void btnFolderBaza_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Signs"))
            {
                System.Diagnostics.Process.Start("explorer.exe", Directory.GetCurrentDirectory() + @"\Signs");
            }
        }

        private void btnDodajElement_Click(object sender, RoutedEventArgs e)
        {
            AddElementToModelSigns();
            AddModelSigns();
        }

        private void btnUsunElement_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var index = dgModelSigns.SelectedIndex;

                if (index != -1)
                {
                    new FileInfo(Config.listModelSigns[index].LocationSign).Delete();
                    AddModelSigns();
                }

            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Integration.WindowsFormsHost host =
                new System.Windows.Forms.Integration.WindowsFormsHost();
            

            MaskedTextBox mtbDate = new MaskedTextBox("00/00/0000");

            host.Child = mtbDate;

            
            this.grid .Children.Add(host);
        }


        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {

            if(Config.listModelSigns.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Wygląda na to ze w bazie znaków nie ma zadnych plików. Może coś dodasz :) ?", "Ostrzeżenie");
                MaintabControl.SelectedIndex = 1;
            }

            if (Config.ofd.ShowDialog() == true)
            {
                var ex = new FileInfo(Config.ofd.FileName).Extension;


                if (ex == ".jpg" || ex == ".JPG" || ex == ".JPEG" || ex == ".tif" || ex == ".png" || ex == ".gif" || ex == ".jpeg" || ex == ".bmp" || ex == ".mp4")
                {

                    if (ex == ".mp4")
                    {
                        Config.isVideo = true;

                        ImageViewer viewer = new ImageViewer();
                        Emgu.CV.VideoCapture capture = new VideoCapture(Config.ofd.FileName);

                        page2 = new Page2(logBox);
                        page2.btn_imageBox.Source = Config.ConvertBitmapToBitmapImage(capture.QueryFrame().Bitmap);
                        page2.img_VideoIcon.Visibility = Visibility.Visible;

                        frame.NavigationService.Navigate(page2);
                        logBox.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Wybrano element do rozpozania"));

                    }
                    else
                    {
                        Config.isVideo = false;
                        page2 = new Page2(logBox);
                        page2.btn_imageBox.Source = new BitmapImage(new Uri(Config.ofd.FileName));
                        frame.NavigationService.Navigate(page2);
                        logBox.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Wybrano element do rozpozania"));
                    }




                }

                else
                {
                    var page2 = new Error();
                    frame.NavigationService.Navigate(page2);


                }


            }






            }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ByMe_Click(object sender, RoutedEventArgs e)
        {
            ByMe by = new ByMe();
            MaintabControl.SelectedIndex = 0;
            frame.NavigationService.Navigate(by);

        }

        private void Main_Click_1(object sender, RoutedEventArgs e)
        {
            MaintabControl.SelectedIndex = 0;
        }

        private void Base_Click_2(object sender, RoutedEventArgs e)
        {
            MaintabControl.SelectedIndex = 1;

        }

        private void Preferences_Click_3(object sender, RoutedEventArgs e)
        {
            MaintabControl.SelectedIndex = 2;

        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            logBox.Items.Clear();
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //Environment.Exit(0);
                System.Windows.Application.Current.Shutdown();
                SavePreferences();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void imgView_Click(object sender, RoutedEventArgs e)
        {
            if (Config.ofd.FileName != "" && Config.isVideo == false)
            {
                ImageViewer iv = new ImageViewer();
                iv.Text = new FileInfo(Config.ofd.FileName).Name + " - Kliknij prawym przyciskiem myszy aby skorzystać z dostępnych filtrów, funkcji";
                iv.Image = new Image<Bgr, byte>(Config.ofd.FileName);
                //CvInvoke.PutText(iv.Image, "ABCDEFG", new System.Drawing.Point(100,100), FontFace.HersheyComplexSmall, 1d, new Bgr(System.Drawing.Color.Red).MCvScalar);
                iv.AutoSize = true;
                iv.Show();

            }

        }

        private void btn_refresh_Click_1(object sender, RoutedEventArgs e)
        {
            AddModelSigns();
        }

        private void MaintabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MaintabControl.SelectedIndex)
            {
                case 0:
                    lb_window.Content = "Ekran Główny";
                    break;
                case 1:
                    lb_window.Content = "Baza Znaków";
                    break;
                case 2:
                    lb_window.Content = "Ustawienia";
                    break;
                default:
                    break;
            }
        }

        private void slider_Algorytm_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider_Algorytm != null)
            {
                
                slider_Algorytm.Value = (int)slider_Algorytm.Value;

                int choice = (int)slider_Algorytm.Value;
                Config.algorithmType = (Config.Algorithm)choice - 1;
                lb_uniques.Content = String.Format("{0:0.##}", slider_uniquenessThreshold.Value);
                Config.uniquenessThreshold = Convert.ToDouble(lb_uniques.Content);

                AddModelSigns();

            }
        }

        private void plus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lb_uniques != null && (slider_uniquenessThreshold.Value - 0.05) >= 0) lb_uniques.Content = String.Format("{0:0.##}", slider_uniquenessThreshold.Value = slider_uniquenessThreshold.Value - 0.05);
            if (lb_uniques != null) Config.uniquenessThreshold = Convert.ToDouble(lb_uniques.Content);
        }

        private void minus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lb_uniques != null && (slider_uniquenessThreshold.Value + 0.05) <= 1) lb_uniques.Content = String.Format("{0:0.##}", slider_uniquenessThreshold.Value = slider_uniquenessThreshold.Value + 0.05);
            if (lb_uniques != null) Config.uniquenessThreshold = Convert.ToDouble(lb_uniques.Content);

        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void checkBox_red_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox_red.IsChecked) Config.isRedChecked = true;
            else Config.isRedChecked = false;
        }

        private void checkBox_blue_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox_blue.IsChecked) Config.isBlueChecked = true;
            else Config.isBlueChecked = false;
        }

        private void checkBox_yellow_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox_yellow.IsChecked) Config.isYellowChecked = true;
            else Config.isYellowChecked = false;
        }

        private void checkBox_yellow_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void slider_PSNR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePSNR();
        }

        private void slider_uniquenessThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(slider_uniquenessThreshold != null && lb_uniques != null)
            {
                lb_uniques.Content = slider_uniquenessThreshold.Value.ToString("0.##");
                Config.uniquenessThreshold = slider_uniquenessThreshold.Value;
            }

        }

      
        private void filterBox_Click(object sender, RoutedEventArgs e)
        {
            if (filterBox != null)
            {
                if ((bool)filterBox.IsChecked) Config.isFiltering = true;
                else Config.isFiltering = false;
            }

        }

        private void slider_VideoSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider_VideoSpeed != null && lb_VideoSpeed != null)
            {
                lb_VideoSpeed.Content = String.Format("{0:0.##}", slider_VideoSpeed.Value);
                Config.videoFrameSeconds = Convert.ToDouble(lb_VideoSpeed.Content);
            }
        }

        private void isLearningCheck_Click(object sender, RoutedEventArgs e)
        {
            if (isLearningCheck != null)
            {
                if ((bool)isLearningCheck.IsChecked) Config.LearningMode = true;
                else Config.LearningMode = false;
            }
        }

        private void Mask_Click(object sender, RoutedEventArgs e)
        {
            int l = 0;
            if (Config.Masks != null)
            {
                foreach (var item in Config.Masks)
                {
                    ImageViewer i = new ImageViewer();
                    i.Image = item;


                    if(l == 0) i.Text = "Maska czerwona";
                    if (l == 1) i.Text = "Maska żółta";
                    if (l == 2) i.Text = "Maska niebieska";
                    i.Show();

                    l++;
                }
            }
        }

        private void psnr_minus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (psnr_minus != null && (slider_PSNR.Value - 0.5) >= 0) lb_PSNR.Content = String.Format("{0:0.##}", slider_PSNR.Value = slider_PSNR.Value - 0.5);
        }

        private void psnr_plus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (psnr_minus != null && (slider_PSNR.Value + 0.5) <= 10) lb_PSNR.Content = String.Format("{0:0.##}", slider_PSNR.Value = slider_PSNR.Value + 0.5);
        }

        private void minus_VideoSpeed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (minus_VideoSpeed != null && (slider_VideoSpeed.Value - 0.2) >= 0) slider_VideoSpeed.Value = slider_VideoSpeed.Value - 0.2;
            

        }

        private void plus_VideoSpeed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (plus_VideoSpeed != null && (slider_VideoSpeed.Value + 0.2) <= 5) slider_VideoSpeed.Value = slider_VideoSpeed.Value + 0.2;
            
        }

        private void buttonDefaults_Click(object sender, RoutedEventArgs e)
        {
            LoadPreferences(true);
        }

        private void btn_algorytmMinus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (btn_algorytmMinus != null && (slider_Algorytm.Value - 1) >= 1) slider_Algorytm.Value = slider_Algorytm.Value - 1;


        }

        private void btn_algorytmPlus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (btn_algorytmPlus != null && (slider_Algorytm.Value + 1) <= 3) slider_Algorytm.Value = slider_Algorytm.Value + 1;

        }

        private void sliderResize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderResize != null && (label_Resize2 != null))
            {
                Config.imageSize = (int)sliderResize.Value;
                label_Resize2.Content = String.Format("{0:0.##}", "Rozmiar obrazu to: " +  ((int)sliderResize.Value).ToString() + "x" + ((int)sliderResize.Value).ToString());

            }

        }

        private void sliderResize_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            size = Config.imageSize = (int)sliderResize.Value;
            AddModelSigns();

        }
    }
}



















