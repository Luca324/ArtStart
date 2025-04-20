using System.Windows;

namespace april
{
    public partial class ColorMix : Window
    {
        public ColorMix()
        {
            InitializeComponent();
            Challenges.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;

        }

    }
}
