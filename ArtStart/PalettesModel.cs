using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ArtStart.Models;

namespace ArtStart
{
    internal class PalettesModel
    {
        private const string PALETTES_PATH = @"../../palettes.json";

        public static PalettesFileModel getPalettesData()
        {

            if (!File.Exists(PALETTES_PATH))
            {
                File.WriteAllText(PALETTES_PATH, JsonConvert.SerializeObject(new PalettesFileModel()));
            }

            string json = File.ReadAllText(PALETTES_PATH);

            // Десериализация строки в объект
            return JsonConvert.DeserializeObject<PalettesFileModel>(json);
        }
        public static void savePalettesData(PalettesFileModel data)
        {
            // обновляем список палитр
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(PALETTES_PATH, json);
        }

    }
    public class UserPalettesModel
    {
        public string Login { get; set; }

        // список палитр
        [JsonProperty("palettes")]
        public List<Palette> Palettes { get; set; }

        public UserPalettesModel(string login = "")
        {
            Login = login;
            Palettes = new List<Palette>();
        }

    }

    public class PalettesFileModel
    {
        // список пользователей и их палитр
        [JsonProperty("users")]
        public List<UserPalettesModel> Users { get; set; }

        public PalettesFileModel()
        {
            Users = new List<UserPalettesModel>();
        }

    }

    public class Palette
    {
        public string Name { get; set; } 
        public List<string> Colors { get; set; }  // И это тоже свойство
    }

}
