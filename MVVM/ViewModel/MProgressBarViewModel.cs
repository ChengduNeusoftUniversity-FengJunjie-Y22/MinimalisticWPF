using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    public class MProgressBarViewModel : StateViewModelBase<MProgressBarViewModel>
    {
        public MProgressBarViewModel() { }

        public MProgressBarModel Model { get; set; } = new MProgressBarModel();

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
                if (Shape == ProgressShapes.Ring && Width != value)
                {
                    Width = value;
                }
                OnPropertyChanged(nameof(Height));
            }
        }
        public double Width
        {
            get => Model.Width;
            set
            {
                Model.Width = value;
                FontSize = value * Model.FontSizeConvertRate;
                if (Shape == ProgressShapes.Ring && Height != value)
                {
                    Height = value;
                }
                StripValue = value * Value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public string Text
        {
            get => Model.Text;
            set
            {
                Model.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public double FontSizeConvertRate
        {
            get => Model.FontSizeConvertRate;
            set
            {
                Model.FontSizeConvertRate = value;
                FontSize = Model.Width * value;
                OnPropertyChanged(nameof(FontSizeConvertRate));
            }
        }
        public double FontSize
        {
            get => Model.Width * Model.FontSizeConvertRate;
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

        public double Value
        {
            get => Model.Value;
            set
            {
                if (value >= 0 && value <= 1)
                {
                    Model.Value = value;
                    FillEndAngle = StartAngle + (EndAngle - StartAngle) * value;
                    StripValue = Width * Value;
                    Text = ((int)(value * 100)).ToString() + "%";
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        public double StripValue
        {
            get => Model.StripValue;
            set
            {
                Model.StripValue = value;
                OnPropertyChanged(nameof(StripValue));
            }
        }

        public double StartAngle
        {
            get => Model.StartAngle;
            set
            {
                Model.StartAngle = value;
                FillEndAngle = StartAngle + (EndAngle - StartAngle) * Value;
                OnPropertyChanged(nameof(StartAngle));
            }
        }
        public double EndAngle
        {
            get => Model.BaseEndAngle;
            set
            {
                Model.BaseEndAngle = value;
                FillEndAngle = StartAngle + (EndAngle - StartAngle) * Value;
                OnPropertyChanged(nameof(EndAngle));
            }
        }

        public double FillEndAngle
        {
            get => Model.FillEndAngle;
            set
            {
                if (value >= StartAngle && value <= EndAngle)
                {
                    Model.FillEndAngle = value;
                    OnPropertyChanged(nameof(FillEndAngle));
                }
            }
        }

        public Brush FillBrush
        {
            get => Model.FillBrush;
            set
            {
                Model.FillBrush = value;
                OnPropertyChanged(nameof(FillBrush));
            }
        }
        public Brush BaseColor
        {
            get => Model.BaseColor;
            set
            {
                Model.BaseColor = value;
                OnPropertyChanged(nameof(BaseColor));
            }
        }

        public double Thickness
        {
            get => Model.Thickness;
            set
            {
                Model.Thickness = value;
                if (Shape == ProgressShapes.Strip)
                {
                    Height = value;
                }
                OnPropertyChanged(nameof(Thickness));
            }
        }

        public ProgressShapes Shape
        {
            get => Model.Type;
            set
            {
                Model.Type = value;
                RingOpacity = value == ProgressShapes.Ring ? 1 : 0;
                StripOpacity = value == ProgressShapes.Strip ? 1 : 0;
                Height = value == ProgressShapes.Strip ? Thickness : Width;
                OnPropertyChanged(nameof(Type));
            }
        }

        public double RingOpacity
        {
            get => Model.RingOpacity;
            set
            {
                Model.RingOpacity = value;
                OnPropertyChanged(nameof(RingOpacity));
            }
        }

        public double StripOpacity
        {
            get => Model.StripOpacity;
            set
            {
                Model.StripOpacity = value;
                OnPropertyChanged(nameof(StripOpacity));
            }
        }
    }
}
