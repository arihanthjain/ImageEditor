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
using System.Windows.Shapes;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for ViewExifData.xaml
    /// </summary>
    public partial class ViewExifData : Window
    {
        public ViewExifData(StringBuilder data)
        {
            InitializeComponent();
            Details.Content = data;
        }
    }
}
