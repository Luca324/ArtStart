using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArtStart
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Challenges.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;
          

            Challenges.Click += CloseThis;
            ColorMix.Click += CloseThis;
            Paint.Click += CloseThis;
            
        }
        private void CloseThis(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
