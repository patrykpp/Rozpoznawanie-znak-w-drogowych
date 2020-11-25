using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Rozpoznawanie_znaków___Praca_dyplomowa
{
    public abstract class Sign
    {
        public int Id;
        public string Name;
        public Mat imgSign;
        public string LocationSign;

    }

}


