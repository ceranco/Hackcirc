using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NetworkGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Node.Node MyNode { get; } = new Node.Node();

        public string MissionStatus { get; set; } = "Active";

        public int MessageDestination { get; set; } = -1;

        public MainWindow()
        {
            InitializeComponent();
            UpdateGraph(MyNode.NeighbourNodes);
            DisableRadioButton();

            MyNode.MessageReceived += MessageReceived;
            MyNode.MessageRelayed += MessageRelayed;
            MyNode.MessageAcknowledged += MessageAcknowledged;
            MyNode.MessageSent += MessageSent;
            MyNode.BroadcastMessageReceived += BroadcastMessageReceived;
        }

        private void DisableRadioButton()
        {
            RadioButton GetMyRadioButton()
            {
                switch (MyNode.Id)
                {
                    case 0:
                        return Dest0;
                    case 1:
                        return Dest1;
                    case 2:
                        return Dest2;
                    case 3:
                        return Dest3;
                    case 4:
                        return Dest4;
                    case 5:
                        return Dest5;
                    default:
                        return null;
                }
            }
            var myButton = GetMyRadioButton();
            if (myButton != null)
            {
                myButton.IsEnabled = false;
            }
        }


        private void BroadcastMessageReceived(object sender, Node.Node.MessageArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var message = Encoding.Default.GetString(e.Data);
                InsertLabel(String.Format("Broadcast Message Received From {0}: '{1}'", e.Source, message), Brushes.Orange);
            });
        }

        private void MessageSent(object sender, Node.Node.MessageArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                string message;
                if (e.IsAcknowledgment)
                {
                    message = String.Format("Sending Acknowledgment: {0} -> {1}", e.Source, e.Destination);
                }
                else if (e.Destination == -1)
                {
                    message = "Sending Broadcast Message";
                }
                else
                {
                    message = String.Format("Message Sent: {0} -> {1}", e.Source, e.Destination);
                }
                InsertLabel(message, Brushes.NavajoWhite);
            });
        }

        private void MessageReceived(object sender, Node.Node.MessageArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var message = Encoding.Default.GetString(e.Data);
                InsertLabel(String.Format("Message Received: {0} -> {1}: '{2}'", e.Source, e.Destination, message), Brushes.LightGreen);
            });
        }

        private void MessageRelayed(object sender, Node.Node.MessageArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                InsertLabel(String.Format("Message Relayed: {0} -> {1} -> {2}", e.Source, MyNode.Id, e.Destination), Brushes.LightGoldenrodYellow);
            });
        }

        private void MessageAcknowledged(object sender, Node.Node.MessageArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                InsertLabel(String.Format("Message Acknowledged: {0} -> {1} -> {0}", e.Destination, e.Source), Brushes.LightSkyBlue);
            });
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
            update("E" + MyNode.Id, Brushes.Salmon);

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
        private void InsertLabel(string message, Brush backgroundColor)
        {
            PacketPanel.Children.Insert(0, new Label()
            {
                Content = message,
                Background = backgroundColor,
                Margin = new Thickness(1),
                FontSize = 17
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MyNode.Close();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            int GetDestination()
            {
                if (DestAll.IsChecked == true)
                {
                    return -1;
                }
                else if (Dest0.IsChecked == true)
                {
                    return 0;
                }
                else if (Dest1.IsChecked == true)
                {
                    return 1;
                }
                else if (Dest2.IsChecked == true)
                {
                    return 2;
                }
                else if (Dest3.IsChecked == true)
                {
                    return 3;
                }
                else if (Dest4.IsChecked == true)
                {
                    return 4;
                }
                else if (Dest5.IsChecked == true)
                {
                    return 5;
                }
                return -1;
            }

            var bytes = Encoding.ASCII.GetBytes(InputBox.Text);
            MyNode.SendInfo(bytes, bytes.Length, GetDestination());
        }
    }
}
