using Emgu.CV;
using Emgu.CV.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page2 page2;
        public string itemDrop;
        DataGrid lb = new DataGrid();
        public Page1(DataGrid lb_)
        {
            InitializeComponent();
            lb = lb_;
            SmartWelcome();

        }

        public Page1()
        {
            InitializeComponent();

        }

        private void btn_OpenFileFrame_Click(object sender, RoutedEventArgs e)
        {
            if (Config.listModelSigns.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Wygląda na to ze w bazie znaków nie zadnych plików. Może coś dodasz :) ?", "Ostrzeżenie");

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

                        page2 = new Page2(lb);
                        page2.btn_imageBox.Source = Config.ConvertBitmapToBitmapImage(capture.QueryFrame().Bitmap);
                        page2.img_VideoIcon.Visibility = Visibility.Visible;


                        this.NavigationService.Navigate(page2);
                        lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Wybrano element do rozpozania"));

                    }
                    else
                    {
                        Config.isVideo = false;
                        page2 = new Page2(lb);
                        page2.btn_imageBox.Source = new BitmapImage(new Uri(Config.ofd.FileName));
                        this.NavigationService.Navigate(page2);
                        lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Wybrano element do rozpozania"));
                    }

                }

                else
                {
                    var page2 = new Error();
                    this.NavigationService.Navigate(page2);

                }


            }
        }

        

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void SmartWelcome()
        {
            DateTime time = DateTime.Now;
            Random r = new Random();

            int p = r.Next(5);

            if (time.Hour > 18 && time.Hour > 4 && p == 2)
            {
                welcomeLabel.Content = "Dobry Wieczór :)";


            }
            if (time.Hour > 6 && time.Hour > 18 && p == 2)
            {
                welcomeLabel.Content = "Dzień Dobry :)";
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {

           
                try
                {
                    string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                if (file != null)
                {
                    var ex = new FileInfo(file[0]).Extension;

                    if (Directory.Exists(file[0].ToString()) != true)

                    {
                        if (ex == ".jpg" || ex == ".JPG" || ex == ".JPEG" || ex == ".tif" || ex == ".png" || ex == ".gif" || ex == ".jpeg" || ex == ".bmp" || ex == ".mp4")
                        {

                            if (ex == ".mp4")
                            {
                                Config.isVideo = true;
                                itemDrop = file[0].ToString();
                                ImageViewer viewer = new ImageViewer();
                                Emgu.CV.VideoCapture capture = new VideoCapture(file[0].ToString());

                                page2 = new Page2(lb);
                                Config.ofd.FileName = itemDrop;
                                page2.btn_imageBox.Source = Config.ConvertBitmapToBitmapImage(capture.QueryFrame().Bitmap);
                                page2.img_VideoIcon.Visibility = Visibility.Visible;

                                this.NavigationService.Navigate(page2);
                                lb.Items.Insert(0, (DateTime.Now.ToShortTimeString() + " Wybrano element do rozpozania"));
                            }
                            else
                            {
                                Config.isVideo = false;
                                itemDrop = file[0].ToString();
                                page2 = new Page2(lb);
                                Config.ofd.FileName = itemDrop;
                                page2.btn_imageBox.Source = new BitmapImage(new Uri(itemDrop));
                                this.NavigationService.Navigate(page2);
                            }
                        }
                        else
                        {
                            var page2 = new Error();
                            this.NavigationService.Navigate(page2);

                        }

                    }
                }
                else
                {
                    var page2 = new Error();
                    this.NavigationService.Navigate(page2);

                }
            }
                catch (Exception ex)
                {
                    Console.WriteLine("Error DragDrop: " + ex.Message);
                }
            
            
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy; 
            else
                e.Effects = DragDropEffects.None; 

        }

        
    }
}
