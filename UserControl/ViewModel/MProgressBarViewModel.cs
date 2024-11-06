using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    public class MProgressBarViewModel : ViewModelBase<MProgressBarViewModel, MProgressBarModel>
    {
        public MProgressBarViewModel() { this.AsGlobalTheme(); }

        public override double Width
        {
            get => Model.Width;
            set
            {
                Model.Width = value;
                Height = Shape == ProgressShapes.Ring ? value : Thickness;
                OnPropertyChanged(nameof(Width));
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
