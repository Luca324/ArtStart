using System.Windows;

namespace ArtStart
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Подписываемся на события
            Challenges.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;
            Info.Click += Utils.Navigation_Click;
            LogOut.Click += Utils.LogOut;

            // Дополнительно: закрытие окна по клику
            Challenges.Click += CloseThis;
            ColorMix.Click += CloseThis;
            Paint.Click += CloseThis;
            Info.Click += CloseThis;
        }

        private void CloseThis(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}