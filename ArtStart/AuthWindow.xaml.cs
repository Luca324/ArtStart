using System.IO;
using System.Linq;
using ArtStart.Models;
using Newtonsoft.Json;
using System.Windows;

namespace ArtStart
{
    public partial class AuthWindow : Window
    {
        private const string USER_DATA_PATH = @"../../registration_data.json";

        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "Заполните все поля.";
                return;
            }

            var userData = LoadUsers();

            var user = userData.Users.FirstOrDefault(u => u.Login == login);

            if (user == null)
            {
                ErrorText.Text = "Пользователь не найден.";
                return;
            }

            if (user.Password != password)
            {
                ErrorText.Text = "Неверный пароль.";
                return;
            }

            // Успешный вход
            user.IsAuthenticated = true;
            SaveUsers(userData);

            OpenMainWindow();
        }

        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegWindow();
            regWindow.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private UserDataModel LoadUsers()
        {
            if (!File.Exists(USER_DATA_PATH))
            {
                File.WriteAllText(USER_DATA_PATH, JsonConvert.SerializeObject(new UserDataModel()));
            }

            string json = File.ReadAllText(USER_DATA_PATH);
            return JsonConvert.DeserializeObject<UserDataModel>(json);
        }

        private void SaveUsers(UserDataModel data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(USER_DATA_PATH, json);
        }

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}