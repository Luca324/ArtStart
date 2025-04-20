using System;
using System.Windows;

namespace april
{
    /// <summary>
    /// Логика взаимодействия для Challenges.xaml
    /// </summary>
    public partial class Challenges : Window
    {
        public Challenges()
        {
            InitializeComponent();

            ColorMix.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;
        }

    }
}
