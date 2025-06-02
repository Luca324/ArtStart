using System.Windows;
using Scrtwpns.Mixbox;
using System.Drawing;

namespace ArtStart
{
    public partial class ColorMix : Window
    {
        public ColorMix()
        {
            InitializeComponent();
            Challenges.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;
           
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Color color1 = Color.FromArgb(0, 33, 133);  // blue
            Color color2 = Color.FromArgb(252, 211, 0); // yellow
            float t = 0.5f;                             // mixing ratio

            Color color = Color.FromArgb(Mixbox.Lerp(color1.ToArgb(), color2.ToArgb(), t));
            System.Windows.Media.Color mixColor2 = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            text1.Text = color.ToString();
            text2.Text = mixColor2.ToString();
            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(mixColor2);
            text3.Text = mixColor2.ToString();
            button1.Background = brush;
            System.Console.WriteLine("colorMix:", color);
        }
    }
}
