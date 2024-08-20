﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class MButtonModel
    {
        public MButtonModel() { }

        public Brush FixedTransparent { get; set; } = Brushes.Transparent;

        public double Height { get; set; } = 80;

        public double Width { get; set; } = 280;

        public double FontSizeConvertRate { get; set; } = 0.7;

        public double FontSize { get; set; } = 56;

        public string Text { get; set; } = "MButton";

        public Brush Foreground { get; set; } = Brushes.White;

        public CornerRadius CornerRadius { get; set; } = new CornerRadius(10);

        public Thickness FixedBorderThickness { get; set; } = new Thickness(1);

        public Brush FixedBorderBrush { get; set; } = Brushes.White;

        public Brush ActualBackground { get; set; } = Brushes.Transparent;

        public double ActualBackgroundOpacity { get; set; } = 1;

        public bool IsMouseOver { get; set; } = false;
    }
}
