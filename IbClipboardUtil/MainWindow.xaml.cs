using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
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
using ICSharpCode.AvalonEdit.Highlighting;
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
            IHighlightingDefinition highlighting;
            using(Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("IbClipboardUtil.Highlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    highlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            InitializeComponent();
            textEditor.SyntaxHighlighting = highlighting;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder str = new StringBuilder();

            IbClipboard.Open();
            foreach (var format in IbClipboard.GetFormats())
            {
                byte[] data = format.GetData();

                str.Append(
$@"{(uint)format, -5}  {format.GetName()} {{
{(data == null ? "" :
BitConverter.ToString(data, 0, Math.Min(data.Length, 4096)).Replace("-", " ")
)}
}}
");
            }
            IbClipboard.Close();

            textEditor.Text = str.ToString();
        }
    }
}
