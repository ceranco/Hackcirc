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

namespace NetworkGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Queue<string> _packetQueue = new Queue<string>(10);
        public string ID { get; set; } = "1";

        public string MissionStatus { get; set; } = "Active";

        public MainWindow()
        {
            InitializeComponent();

            _packetQueue.Enqueue("Test Message");


            PacketTextBox.AppendText("Test Message1\n");
            PacketTextBox.SelectAll();
            PacketTextBox.SelectionBrush = Color;
            int firstCharOfLineIndex = PacketTextBox.();
            int currentLine = richTextBox1.GetLineFromCharIndex(firstCharOfLineIndex);
            this.myRichTextBox.Select(firstCharOfLineIndex, currentLine);
            this.myRichTextBox.SelectionBackColor = Color.Aqua;
            this.myRichTextBox.Select(0, 0);

            PacketTextBox.AppendText("Test Message2\n");
        }
    }
}
