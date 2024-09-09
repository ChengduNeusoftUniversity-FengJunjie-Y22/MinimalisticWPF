using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    /// <summary>
    /// 为ViewModel的设计提供一个模板
    /// <para>特性:</para>
    /// <para>1.已接入INotifyPropertyChanged,使用OnPropertyChanged(nameof(Property))通知UI更新</para>
    /// <para>2.已接入IConditionalTransfer,可使用状态机系统提供的条件切换</para>
    /// <para>3.已实现ModelBase的所有属性,你只需在此基础上扩展更多属性</para>
    /// </summary>
    /// <typeparam name="T1">继承ViewModelBase的具体类型</typeparam>
    /// <typeparam name="T2">继承ModelBase的具体类型</typeparam>
    public abstract class ViewModelBase<T1, T2> : StateViewModelBase<T1>
        where T1 : class
        where T2 : ModelBase, new()
    {
        public T2 Model = new T2();

        public virtual Brush FixedTransparent
        {
            get => Model.FixedTransparent;
            set { }
        }
        public virtual Thickness FixedNoThickness
        {
            get => Model.FixedNoThickness;
            set { }
        }
        public virtual CornerRadius FixedNoCornerRadius
        {
            get => Model.FixedNoCornerRadius;
            set { }
        }
        public virtual string Text
        {
            get => Model.Text;
            set
            {
                Model.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public virtual Brush TextBrush
        {
            get => Model.TextBrush;
            set
            {
                Model.TextBrush = value;
                OnPropertyChanged(nameof(TextBrush));
            }
        }
        public virtual double TextSize
        {
            get => Model.TextSize;
            set
            {
                Model.TextSize = value;
                OnPropertyChanged(nameof(TextSize));
            }
        }
        public virtual double FontSizeConvertRate
        {
            get => Model.FontSizeConvertRate;
            set
            {
                Model.FontSizeConvertRate = value;
                TextSize = Height * value;
                OnPropertyChanged(nameof(FontSizeConvertRate));
            }
        }
        public virtual Thickness EdgeThickness
        {
            get => Model.EdgeThickness;
            set
            {
                Model.EdgeThickness = value;
                OnPropertyChanged(nameof(EdgeThickness));
            }
        }
        public virtual Brush EdgeBrush
        {
            get => Model.EdgeBrush;
            set
            {
                Model.EdgeBrush = value;
                OnPropertyChanged(nameof(EdgeBrush));
            }
        }
        public virtual CornerRadius CornerRadius
        {
            get => Model.CornerRadius;
            set
            {
                Model.CornerRadius = value;
                OnPropertyChanged(nameof(CornerRadius));
            }
        }
        public virtual Brush BackBrush
        {
            get => Model.BackBrush;
            set
            {
                Model.BackBrush = value;
                OnPropertyChanged(nameof(BackBrush));
            }
        }
        public virtual Brush HoverBackground
        {
            get => Model.HoverBackground;
            set
            {
                Model.HoverBackground = value;
                OnPropertyChanged(nameof(HoverBackground));
            }
        }
        public virtual Brush HoverTextBrush
        {
            get => Model.HoverTextBrush;
            set
            {
                Model.HoverTextBrush = value;
                OnPropertyChanged(nameof(HoverTextBrush));
            }
        }
        public virtual Brush HoverEdgeBrush
        {
            get => Model.HoverEdgeBrush;
            set
            {
                Model.HoverEdgeBrush = value;
                OnPropertyChanged(nameof(HoverEdgeBrush));
            }
        }
        public virtual double HoverBackgroundOpacity
        {
            get => Model.HoverBackgroundOpacity;
            set
            {
                Model.HoverBackgroundOpacity = value;
                OnPropertyChanged(nameof(HoverBackgroundOpacity));
            }
        }
        public virtual double HoverGlobalOpacity
        {
            get => Model.HoverGlobalOpacity;
            set
            {
                Model.HoverGlobalOpacity = value;
                OnPropertyChanged(nameof(HoverGlobalOpacity));
            }
        }
        public virtual double Height
        {
            get => Model.Height;
            set
            {
                Model.Height = value;
                TextSize = value * FontSizeConvertRate;
                OnPropertyChanged(nameof(Height));
            }
        }
        public virtual double Width
        {
            get => Model.Width;
            set
            {
                Model.Width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
    }
}
