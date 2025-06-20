using ArtStart.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ArtStart
{
    internal class Utils
    {
        public static void Navigation_Click(object sender, RoutedEventArgs e)
        {
            string targetButtonName = ((FrameworkElement)sender).Name;

            Console.WriteLine($"targetButtonName: {targetButtonName}");
            OpenWindow(targetButtonName);
        }

        public static void LogOut(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("logging out");

            var userData = UserDataModel.LoadUsers();

            userData.IsAuthenticated = false;
            userData.CurrentUser = null;
            Session.CurrentUser = null;
            UserDataModel.SaveUsers(userData);

            OpenWindow("MainApp");

            // закрываем все окна 
            foreach (Window window in App.Current.Windows)
            {
                var title = window.Name;

                if (title == "ChallengesWindow" || title == "ColorMixWindow" || title == "PaintWindow" || title == "MainWindowWindow")
                {
                    Console.WriteLine($"closing {title}");
                    window.Close();
                }
            }

        }

        public static void OpenWindow(string targetButtonName)
        {
            Window newWindow = null;
            switch (targetButtonName)
            {
                case "MainApp":
                    newWindow = new MainApp();
                    break;
                case "MainWindow":
                    newWindow = new MainWindow();
                    break;
                case "Challenges":
                    newWindow = new Challenges();
                    break;
                case "Challenges2":
                    newWindow = new Challenges2();
                    break;
                case "Challenges3":
                    newWindow = new Challenges3();
                    break;
                case "Paint":
                    newWindow = new Paint();
                    break;
                case "ColorMix":
                    newWindow = new ColorMix();
                    break;
                case "Info":
                    newWindow = new InfoWindow();
                    break;
            }

            if (newWindow != null)
            {
                newWindow.Show();

                // Закрыть все остальные окна, кроме только что открытого
                foreach (Window window in App.Current.Windows)
                {
                    if (window != newWindow)
                    {
                        window.Close();
                    }
                }
            }
        }

    }

}

