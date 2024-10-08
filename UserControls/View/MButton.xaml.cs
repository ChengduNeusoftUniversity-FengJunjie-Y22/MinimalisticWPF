﻿using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public partial class MButton : UserControl
    {
        public MButton()
        {
            InitializeComponent();
            this.StateMachineLoading(ViewModel);
        }

        private bool _iseol = false;
        public bool IsEdgeOpacityLocked
        {
            get => _iseol;
            set
            {
                if (_iseol != value)
                {
                    _iseol = value;
                    if (value)
                    {
                        FixedBorder.Opacity = 1;
                    }
                }
            }
        }

        public double WiseHeight
        {
            get => ViewModel.Height;
            set => ViewModel.Height = value;
        }

        public double WiseWidth
        {
            get => ViewModel.Width;
            set => ViewModel.Width = value;
        }

        public event MouseButtonEventHandler? Click
        {
            add { BackgroundBorder.PreviewMouseLeftButtonDown += value; }
            remove { BackgroundBorder.PreviewMouseLeftButtonDown -= value; }
        }

        public string Text
        {
            get => ViewModel.Text;
            set => ViewModel.Text = value;
        }

        public double FontSizeRatio
        {
            get => ViewModel.FontSizeConvertRate;
            set => ViewModel.FontSizeConvertRate = value;
        }

        public Brush TextBrush
        {
            get => ViewModel.TextBrush;
            set => ViewModel.TextBrush = value;
        }

        public Brush HoverBrush
        {
            get => ViewModel.HoverBackground;
            set => ViewModel.HoverBackground = value;
        }

        public Brush EdgeBrush
        {
            get => ViewModel.EdgeBrush;
            set => ViewModel.EdgeBrush = value;
        }

        public Thickness EdgeThickness
        {
            get => ViewModel.EdgeThickness;
            set => ViewModel.EdgeThickness = value;
        }

        public CornerRadius CornerRadius
        {
            get => ViewModel.CornerRadius;
            set => ViewModel.CornerRadius = value;
        }

        public double HoverOpacity
        {
            get => ViewModel.HoverBackgroundOpacity;
            internal set => ViewModel.HoverBackgroundOpacity = value;
        }

        private void BackgroundBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.IsMouseInside = true;
            if (IsEdgeOpacityLocked) return;
            this.FixedBorder.Transition()
                .SetProperty(x => x.Opacity, 1)
                .SetParams((x) => { x.Duration = 0.5; })
                .Start();
        }

        private void BackgroundBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.IsMouseInside = false;
            if (IsEdgeOpacityLocked) return;
            this.FixedBorder.Transition()
                .SetProperty(x => x.Opacity, 0)
                .SetParams((x) => { x.Duration = 0.5; })
                .Start();
        }
    }
}
