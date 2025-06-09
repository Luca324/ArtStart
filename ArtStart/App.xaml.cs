using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using ArtStart.Models;

namespace ArtStart
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string USER_DATA_PATH = @"../../registration_data.json";
            if (!File.Exists(USER_DATA_PATH))
            {
                File.WriteAllText(USER_DATA_PATH, JsonConvert.SerializeObject(new Models.UserDataModel()));
            }

            string json = File.ReadAllText(USER_DATA_PATH);
            var userData = JsonConvert.DeserializeObject<Models.UserDataModel>(json);

            bool isAuthenticated = userData.Users.Any(u => u.IsAuthenticated);

            if (isAuthenticated)
            {
                MainWindow = new MainWindow();
            }
            else
            {
                MainWindow = new AuthWindow();
            }

            MainWindow.Show();
        }
    }
}