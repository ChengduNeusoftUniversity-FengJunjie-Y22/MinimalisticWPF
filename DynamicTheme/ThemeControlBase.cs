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
    public abstract class ThemeControlBase : UserControl
    {
        /// <summary>
        /// 边角圆滑度
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ThemeControlBase), new PropertyMetadata(new CornerRadius(6)));

        /// <summary>
        /// 边界框体厚度
        /// </summary>
        public Thickness EdgeThickness
        {
            get { return (Thickness)GetValue(EdgeThicknessProperty); }
            set { SetValue(EdgeThicknessProperty, value); }
        }
        public static readonly DependencyProperty EdgeThicknessProperty =
            DependencyProperty.Register("EdgeThickness", typeof(Thickness), typeof(ThemeControlBase), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// 边界框体颜色
        /// </summary>
        public Brush EdgeBrush
        {
            get { return (Brush)GetValue(EdgeBrushProperty); }
            set { SetValue(EdgeBrushProperty, value); }
        }
        public static readonly DependencyProperty EdgeBrushProperty =
            DependencyProperty.Register("EdgeBrush", typeof(Brush), typeof(ThemeControlBase), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 文本自适应率
        /// </summary>
        public double TextAdaptationRatio
        {
            get { return (double)GetValue(TextAdaptationRatioProperty); }
            set { SetValue(TextAdaptationRatioProperty, value); }
        }
        public static readonly DependencyProperty TextAdaptationRatioProperty =
            DependencyProperty.Register("TextAdaptationRatio", typeof(double), typeof(ThemeControlBase), new PropertyMetadata(0));


    }
}
