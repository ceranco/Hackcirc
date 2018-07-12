﻿using System;
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
                else if(Dest1.IsChecked == true)
                {
                    return 1;
                }
                else if(Dest2.IsChecked == true)
                {
                    return 2;
                }
                else if(Dest3.IsChecked == true)
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
