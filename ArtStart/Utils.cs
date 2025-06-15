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
                var title = window.Title;

                if (title == "Challenges" || title == "ColorMix" || title == "Paint" || title == "MainWindow")
                {
                    Console.WriteLine($"closing {title}");
                    window.Close();
                }
            }

        }

        public static void OpenWindow(string targetButtonName)
        {
            var alreadyOpened = false;
            foreach (Window window in App.Current.Windows)
            {
                var title = window.Title;
                Console.WriteLine($"title: {title}");

                if (title == targetButtonName)
                {
                    window.Activate();
                    alreadyOpened = true;
                    break;
                }
            }
            if (alreadyOpened == false)
            {
                Window window = null;
                switch (targetButtonName)
                {
                    case "MainApp":
                        window = new MainApp();
                        break;
                    case "MainWindow":
                        window = new MainWindow();
                        break;
                    case "Challenges":
                        window = new Challenges();
                        break;
                    case "Paint":
                        window = new Paint();
                        break;
                    case "ColorMix":
                        window = new ColorMix();
                        break;

                }
                window.Show();

            }
        }
    }

}

