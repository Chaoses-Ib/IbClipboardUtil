using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using Ib.Windows.DataExchange;
using IbClipboard = Ib.Windows.DataExchange.Clipboard;

namespace IbClipboardUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            texta.Text = "";
            IbClipboard.Open();
            foreach (var format in IbClipboard.GetFormats())
            {
                byte[] data = format.GetData();

                texta.Text += 
$@"{(uint)format}{"\t"}{format.GetName()}
{(data == null ? ""  :
BitConverter.ToString(data, 0, Math.Min(data.Length, 4096)).Replace("-", " ")
)}

";
            }
            IbClipboard.Close();

            
        }
    }
}
