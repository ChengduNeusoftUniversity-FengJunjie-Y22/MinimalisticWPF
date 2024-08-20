﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    public partial class MButton : UserControl
    {
        public MButton()
        {
            InitializeComponent();

            MButtonViewModel MO = new MButtonViewModel() { ActualBackgroundOpacity = 0.4 };

            State Source = new State("default", DataContext as MButtonViewModel);
            State MouseOver = new State("mouseover", MO);

            Machine = new StateMachine<MButtonViewModel>(ViewModel, Source, MouseOver);
        }

        private StateMachine<MButtonViewModel> Machine { get; set; }

        private void BackgroundBorder_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void BackgroundBorder_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void BackgroundBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("mouseover", 0.2);
        }

        private void BackgroundBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("default", 0.2);
        }
    }
}