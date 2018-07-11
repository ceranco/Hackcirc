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
        public string ID { get; set; } = "1";
        public string MissionStatus { get; set; } = "Active";

        public MainWindow()
        {
            InitializeComponent();

            UpdateGraph(new List<int>() { 0, 2 });

            for (int i = 0; i < 30; i++)
            {
                InsertLabel((i % 2) == 0 ? Brushes.LightGreen : Brushes.LightSkyBlue, String.Format("This is message {0}!", i));
            }
        }

        /// <summary>
        /// Update the colors of the neighbours graph.
        /// </summary>
        /// <param name="neighbours">A list of the neighbours of this node.</param>
        private void UpdateGraph(List<int> neighbours)
        {
            void update(string ellipse, Brush color)
            {
                if (ellipse == "E0")
                {
                    ConnectivityGraph.E0.Fill = color;
                }
                else if (ellipse == "E1")
                {
                    ConnectivityGraph.E1.Fill = color;
                }
                else if (ellipse == "E2")
                {
                    ConnectivityGraph.E2.Fill = color;
                }
                else if (ellipse == "E3")
                {
                    ConnectivityGraph.E3.Fill = color;
                }
                else if (ellipse == "E4")
                {
                    ConnectivityGraph.E4.Fill = color;
                }
                else if (ellipse == "E5")
                {
                    ConnectivityGraph.E5.Fill = color;
                }

            }
            update("E" + ID, Brushes.Salmon);

            foreach (var id in neighbours)
            {
                update("E" + id, Brushes.LightSeaGreen);
            }
        }

        /// <summary>
        /// Inserts a new Label into PacketPanel.
        /// Use <see cref="Brushes.LightGreen"/> for messages for this ID.
        /// Use <see cref="Brushes.LightSkyBlue"/> for routed messages.
        /// </summary>
        /// <param name="backgroundColor">The color to set the background to.</param>
        /// <param name="message">The message to set in the label.</param>
        private void InsertLabel(Brush backgroundColor, string message)
        {
            PacketPanel.Children.Insert(0, new Label()
            {
                Content = message,
                Background = backgroundColor,
                Margin = new Thickness(1),
                FontSize = 17
            });
        }
    }
}
