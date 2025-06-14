using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace ArtStart.Models
{


    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserDataModel
    {
        public const string USER_DATA_PATH = @"../../registration_data.json";
        public bool IsAuthenticated { get; set; }
        public User CurrentUser { get; set; }

        [JsonProperty("users")]
        public List<User> Users { get; set; } = new List<User>();
        public UserDataModel()
        {
            IsAuthenticated = false;
            CurrentUser = null;
        }
        public static UserDataModel LoadUsers()
        {
            if (!File.Exists(USER_DATA_PATH))
            {
                File.WriteAllText(USER_DATA_PATH, JsonConvert.SerializeObject(new UserDataModel()));
            }

            string json = File.ReadAllText(USER_DATA_PATH);
            Console.WriteLine("user data json:", json);
            return JsonConvert.DeserializeObject<UserDataModel>(json);
        }

        public static void SaveUsers(UserDataModel data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(UserDataModel.USER_DATA_PATH, json);
        }
    }
}

