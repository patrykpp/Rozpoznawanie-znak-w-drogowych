using System;
using System.Collections.Generic;
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

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
   
    public partial class Error : Page
    {
        public Error()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {

           
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var pg1 = new Page1();
            this.NavigationService.Navigate(pg1);

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var pg1 = new Page1();

            this.NavigationService.Navigate(pg1);

            RoutedEventArgs newEventArgs = new RoutedEventArgs(Button.ClickEvent);
            pg1.btn_OpenFileFrame.RaiseEvent(newEventArgs);
        }
    }
}
