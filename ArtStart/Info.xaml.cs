
using System.Windows;

namespace ArtStart
{
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();

            // Подписка на события кнопок
            MainWindowBtn.Click += Utils.Navigation_Click;
            PaintBtn.Click += Utils.Navigation_Click;
            PaletteBtn.Click += Utils.Navigation_Click;
            ChallengeBtn.Click += Utils.Navigation_Click;

            // Закрытие текущего окна при переходе
            MainWindowBtn.Click += CloseThis;
            PaintBtn.Click += CloseThis;
            PaletteBtn.Click += CloseThis;
            ChallengeBtn.Click += CloseThis;
        }

        private void CloseThis(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
