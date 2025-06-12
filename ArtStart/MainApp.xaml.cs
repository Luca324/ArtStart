using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace ArtStart
{
    public partial class MainApp : Window
    {
        private const string UserDataPath = "registration_data.json";

        public MainApp()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegWindow();
            regWindow.Show();
            this.Close();
        }

        private void AuthBtn_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(UserDataPath))
            {
                var json = File.ReadAllText(UserDataPath);
                var data = JsonConvert.DeserializeObject<RegistrationData>(json);

                if (data != null && data.IsAuthenticated)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                    return;
                }
            }

            var authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class RegistrationData
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}