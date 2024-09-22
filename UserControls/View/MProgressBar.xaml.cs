using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace MinimalisticWPF
{
    public partial class MProgressBar : UserControl
    {
        public MProgressBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 描述从何处开始圆环
        /// </summary>
        public double StartAngle
        {
            get => ViewModel.StartAngle;
            set => ViewModel.StartAngle = value;
        }

        /// <summary>
        /// 描述从何处终止圆环
        /// </summary>
        public double EndAngle
        {
            get => ViewModel.EndAngle;
            set => ViewModel.EndAngle = value;
        }

        /// <summary>
        /// 描述是否使用镜像
        /// </summary>
        public bool IsReverse
        {
            set
            {
                var scale = new ScaleTransform()
                {
                    ScaleX = value ? -1 : 1
                };
                RevA.LayoutTransform = scale;
                RevB1.LayoutTransform = scale;
                RevB2.LayoutTransform = scale;
            }
        }

        /// <summary>
        /// 描述进度条厚度
        /// </summary>
        public double Thickness
        {
            get => ViewModel.Thickness;
            set => ViewModel.Thickness = value;
        }

        /// <summary>
        /// 描述进度条底色
        /// </summary>
        public Brush BottomBrush
        {
            get => ViewModel.BaseColor;
            set => ViewModel.BaseColor = value;
        }

        /// <summary>
        /// 描述进度填充色
        /// </summary>
        public Brush FillBrush
        {
            get => ViewModel.FillBrush;
            set => ViewModel.FillBrush = value;
        }

        /// <summary>
        /// 描述进度文本色
        /// </summary>
        public Brush TextBrush
        {
            get => ViewModel.TextBrush;
            set => ViewModel.TextBrush = value;
        }

        /// <summary>
        /// 描述进度文本大小适应率
        /// </summary>
        public double FontSizeRatio
        {
            get => ViewModel.FontSizeConvertRate;
            set => ViewModel.FontSizeConvertRate = value;
        }

        /// <summary>
        /// 描述进度条的形状
        /// </summary>
        public ProgressShapes Shape
        {
            get => ViewModel.Shape;
            set => ViewModel.Shape = value;
        }

        /// <summary>
        /// 描述进度百分比的值
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(MProgressBar), new PropertyMetadata(0.0, OnValueChanged));
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressBar = (MProgressBar)d;
            progressBar.ViewModel.Value = (double)e.NewValue;
        }

        /// <summary>
        /// 描述进度条的尺寸
        /// </summary>
        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double), typeof(MProgressBar), new PropertyMetadata(0.0, OnSizeChanged));
        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressBar = (MProgressBar)d;
            progressBar.ViewModel.Width = (double)e.NewValue;
        }
    }
}
