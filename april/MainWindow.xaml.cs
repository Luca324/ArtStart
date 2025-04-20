using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace april
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

        //private void Navigation_Click(object sender, RoutedEventArgs e)
        //{
        //    string senderName = ((FrameworkElement)sender).Name;

        //    Console.WriteLine($"sender: {sender}\n senderName: {senderName}");

        //    if (senderName == "Challenges")
        //    {
        //        var window = new Challenges();
        //        window.Show();
        //    }
        //    else if (senderName == "Paint")
        //    {
        //        var window = new Paint();
        //        window.Show();
        //    }
        //    else if (senderName == "ColorMix")
        //    {
        //        var window = new ColorMix();
        //        window.Show();

        //    }
        //    this.Close();
        //}


    }
}
