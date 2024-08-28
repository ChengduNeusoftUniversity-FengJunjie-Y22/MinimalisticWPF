using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class MButtonViewModel : StateViewModelBase<MButtonViewModel>
    {
        public MButtonViewModel() { }

        public static State Start = State.FromObject(new MButtonViewModel())
            .SetName("defualt")
            .SetProperty(x => x.ActualBackgroundOpacity, 1)
            .ToState();
        public static State MouseIn = State.FromObject(new MButtonViewModel())
            .SetName("mouseInside")
            .SetProperty(x => x.ActualBackgroundOpacity, 0.1)
            .ToState();

        public string Text
        {
            get => Model.Text;
            set
            {
                Model.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public MButtonModel Model { get; set; } = new MButtonModel();

        public Brush FixedTransparent
        {
            get => Model.FixedTransparent;
            set
            {
                OnPropertyChanged(nameof(FixedTransparent));
            }
        }

        public double Height
        {
            get => Model.Height;
            set
            {
                Model.Height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public double Width
        {
            get => Model.Width;
            set
            {
                Model.Width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public double FontSizeConvertRate
        {
            get => Model.FontSizeConvertRate;
            set
            {
                Model.FontSizeConvertRate = value;
                OnPropertyChanged(nameof(FontSizeConvertRate));
            }
        }

        public double FontSize
        {
            get => Model.Height * Model.FontSizeConvertRate;
            set
            {
                Model.FontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        public Brush Foreground
        {
            get => Model.Foreground;
            set
            {
                Model.Foreground = value;
                OnPropertyChanged(nameof(Foreground));
            }
        }

        public CornerRadius CornerRadius
        {
            get => Model.CornerRadius;
            set
            {
                Model.CornerRadius = value;
                OnPropertyChanged(nameof(CornerRadius));
            }
        }

        public Thickness FixedBorderThickness
        {
            get => Model.FixedBorderThickness;
            set
            {
                Model.FixedBorderThickness = value;
                OnPropertyChanged(nameof(FixedBorderThickness));
            }
        }

        public Brush FixedBorderBrush
        {
            get => Model.FixedBorderBrush;
            set
            {
                Model.FixedBorderBrush = value;
                OnPropertyChanged(nameof(FixedBorderBrush));
            }
        }

        public Brush ActualBackground
        {
            get => Model.ActualBackground;
            set
            {
                Model.ActualBackground = value;
                OnPropertyChanged(nameof(ActualBackground));
            }
        }

        public double ActualBackgroundOpacity
        {
            get => Model.ActualBackgroundOpacity;
            set
            {
                Model.ActualBackgroundOpacity = value;
                OnPropertyChanged(nameof(ActualBackgroundOpacity));
            }
        }
    }
}
