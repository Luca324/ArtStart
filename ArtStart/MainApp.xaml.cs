using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace ArtStart
{
    public partial class MainApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Проверяем файл регистрации
            if (File.Exists("registration_data.json"))
            {
                var json = File.ReadAllText("registration_data.json");
                var data = JsonConvert.DeserializeObject<RegistrationData>(json);

                if (data != null && data.IsAuthenticated)
                {
                    // Пользователь авторизован - открываем главное окно
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    // Пользователь не авторизован - открываем окно входа
                    var authWindow = new AuthWindow();
                    authWindow.Show();
                }
            }
            else
            {
                // Файла нет - открываем окно регистрации
                var regWindow = new RegWindow();
                regWindow.Show();
            }
        }

    }

    public class RegistrationData
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("registration_data.json"))
            {
                var json = File.ReadAllText("registration_data.json");
                var data = JsonConvert.DeserializeObject<RegistrationData>(json);

                if (data != null)
                {
                    data.IsAuthenticated = false;
                    File.WriteAllText("registration_data.json", JsonConvert.SerializeObject(data));
                }
            }

            var authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }
    }