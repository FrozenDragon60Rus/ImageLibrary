using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System;

namespace ImageDB
{
    internal class MarkerButton : Button
    {
        public string Marker { get; }
        public int Id { get; }
        internal MarkerButton(string name, int num, string marker, RoutedEventHandler AddEvent, MouseButtonEventHandler RemoveEvent) {
            Marker = marker;
            Name = name + "_button";
            Content = name;
            Background = Brushes.White;
            FontSize = 16f;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Width = 250;
            Height = 30;
            Id = num;
            Margin = new(5,
                        (5 + Height) * (num - 1),
                         0,
                         0);
            Click += AddEvent;
            MouseRightButtonUp += RemoveEvent;
        }
    }
}
