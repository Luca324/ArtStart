using System.IO;
using System.Linq;
using ArtStart.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace ArtStart
{
    public partial class RegWindow : Window
    {

        public RegWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();
            string email = EmailBox.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email))
            {
                ErrorText.Text = "Все поля обязательны к заполнению.";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorText.Text = "Пароли не совпадают.";
                return;
            }

            if (!IsValidEmail(email))
            {
                ErrorText.Text = "Некорректный email.";
                return;
            }

            var userData = UserDataModel.LoadUsers();

            if (userData.Users.Any(u => u.Login == login))
            {
                ErrorText.Text = "Пользователь с таким логином уже существует.";
                return;
            }

            User user = new User
            {
                Login = login,
                Password = password
            };
            // Добавляем нового пользователя
            userData.Users.Add(user);

            SaveUsers(userData);
            Session.CurrentUser = user;

            //UserPalettesModel создаем новый экземпляр (палитры пустые, а логин это юзер) , и записываем в файл
            var data = PalettesModel.getPalettesData();
            data.Users.Add(new UserPalettesModel(login));
            PalettesModel.savePalettesData(data);

            MessageBox.Show("Регистрация успешна!");
            OpenMainWindow();
        }

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            var authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SaveUsers(UserDataModel data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(UserDataModel.USER_DATA_PATH, json);
        }

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}