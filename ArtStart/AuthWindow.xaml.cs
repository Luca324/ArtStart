using System.IO;
using System.Linq;
using ArtStart.Models;
using Newtonsoft.Json;
using System.Windows;

namespace ArtStart
{
    public partial class AuthWindow : Window
    {

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

            var userData = UserDataModel.LoadUsers();

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
            userData.IsAuthenticated = true;
            userData.CurrentUser = user;
            Session.CurrentUser = user;
            UserDataModel.SaveUsers(userData);


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

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}