using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class XButton : UserControl
    {
        public XButton()
        {
            InitializeComponent();
        }

        #region 属性

        /// <summary>
        /// XButton 的 ViewModel层
        /// </summary>
        public XButtonViewModel XButtonViewModel
        {
            get => ViewModel;
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        public event RoutedEventHandler Click
        {
            add => ActualButton.Click += value;
            remove => ActualButton.Click -= value;
        }

        #endregion


        #region 依赖属性

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Thickness FixedBorderThickness
        {
            get => (Thickness)GetValue(FixedBorderThicknessProperty);
            set => SetValue(FixedBorderThicknessProperty, value);
        }

        public SolidColorBrush FixedBorderBrush
        {
            get => (SolidColorBrush)GetValue(FixedBorderBrushProperty);
            set => SetValue(FixedBorderBrushProperty, value);
        }

        public SolidColorBrush ActualBackground
        {
            get => (SolidColorBrush)GetValue(ActualBackgroundProperty);
            set => SetValue(ActualBackgroundProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text),
                typeof(string), typeof(XButton),
                new PropertyMetadata("XButton", OnTextChanged));

        public static readonly DependencyProperty FixedBorderBrushProperty =
            DependencyProperty.Register(nameof(FixedBorderBrush),
                typeof(SolidColorBrush), typeof(XButton),
                new PropertyMetadata(Brushes.White, OnFixedBorderBrushChanged));

        public static readonly DependencyProperty FixedBorderThicknessProperty =
            DependencyProperty.Register(nameof(FixedBorderThickness),
                typeof(Thickness), typeof(XButton),
                new PropertyMetadata(new Thickness(1), OnFixedBorderThicknessChanged));

        public static readonly DependencyProperty ActualBackgroundProperty =
            DependencyProperty.Register(nameof(ActualBackground),
        typeof(SolidColorBrush), typeof(XButton),
        new PropertyMetadata(Brushes.Transparent, OnActualBackgroundChanged));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius),
                typeof(CornerRadius), typeof(XButton),
                new PropertyMetadata(new CornerRadius(5), OnCornerRadiusChanged));

        private static void OnFixedBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (XButton)d;
            control.FixedBorder.BorderBrush = (SolidColorBrush)e.NewValue;
        }

        private static void OnActualBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (XButton)d;
            control.FixedBorder.Background = (SolidColorBrush)e.NewValue;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (XButton)d;
            control.ActualText.Text = (string)e.NewValue;
        }

        private static void OnFixedBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (XButton)d;
            control.FixedBorder.BorderThickness = (Thickness)e.NewValue;
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (XButton)d;
            control.FixedBorder.CornerRadius = (CornerRadius)e.NewValue;
        }

        #endregion
    }
}
