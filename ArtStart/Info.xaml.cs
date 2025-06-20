
using System.Windows;

namespace ArtStart
{
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();

            // Подписка на события кнопок
            MainWindow.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;
            Challenges.Click += Utils.Navigation_Click;
        }

    }
}
