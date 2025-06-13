using System.Windows;
using Scrtwpns.Mixbox;
using System.Windows.Media;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ArtStart
{
    public partial class ColorMix : Window
    {
        private const string PALETTES_PATH = @"../../palettes.json";
        private System.Windows.Media.Color currentColor = new System.Windows.Media.Color();
        private Boolean currentColorExists = false;

        public ColorMix()
        {
            InitializeComponent();

            Challenges.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;
            LogOut.Click += Utils.LogOut;

            var data = getPalettesData();
            renderPalettes(data);
        }


        private void MixBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что оба цвета выбраны
            if (ColorPicker1.SelectedColor.HasValue && ColorPicker2.SelectedColor.HasValue)
            {
                // Преобразуем WPF-цвета в ARGB
                var color1 = ToDrawingColor(ColorPicker1.SelectedColor.Value);
                var color2 = ToDrawingColor(ColorPicker2.SelectedColor.Value);

                // Смешиваем цвета (50/50)
                int mixedArgb = Mixbox.Lerp(color1.ToArgb(), color2.ToArgb(), 0.5f);
                var mixedColor = ToMediaColor(System.Drawing.Color.FromArgb(mixedArgb));

                currentColor = mixedColor;
                Console.WriteLine($"new color:{mixedColor}");
                // Устанавливаем фон кнопки
                result.Background = new SolidColorBrush(mixedColor);
                currentColorExists = true;
            }
        }

        private void CreateNewPalette_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewPaletteName.Text)) return;
            Console.WriteLine(NewPaletteName.Text);

            const string PALETTES_PATH = @"../../palettes.json";

            // Чтение файла
            var data = getPalettesData();


            // создание новой палитры
            Palette newPalette = new Palette();

            newPalette.Name = NewPaletteName.Text;
            newPalette.Colors = new List<string>();
            data.Palettes.Add(newPalette);


            savePalettesData(data);

            // обновляем список палитр
            renderPalettes(data);

            NewPaletteName.Text = "";
        }

        private void renderPalettes(FileModel data)
        {
            Palettes.Children.Clear();

            foreach (var palette in data.Palettes)
            {
                WrapPanel panel = new WrapPanel();
                Button button = new Button();
                button.Content = "+";
                button.Click += (sender, e) =>
                {
                    Console.WriteLine($"palette name: {palette.Name}");
                    AddCurrentColor(palette.Name);
                };

                TextBlock block = new TextBlock();
                block.Text = palette.Name;

                WrapPanel colors = new WrapPanel();
                foreach (var color in palette.Colors)
                {
                    TextBlock colorBlock = new TextBlock();
                    colorBlock.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
                    colorBlock.Width = 20;
                    colorBlock.Height = 20;
                    colors.Children.Add(colorBlock);

                }

                panel.Children.Add(block);
                panel.Children.Add(colors);
                panel.Children.Add(button);
                Palettes.Children.Add(panel);
            }

        }

        private void AddCurrentColor(string palette)
        {
            if (!currentColorExists) return;

            var data = getPalettesData();

            var targetPalette = data.Palettes.Find(p => p.Name == palette);

            // если цвет уже есть
            if (targetPalette.Colors.Contains(currentColor.ToString())) return;

            targetPalette.Colors.Add(currentColor.ToString());
            savePalettesData(data);

            renderPalettes(data);
        }

        public static FileModel getPalettesData()
        {

            // Чтение файла
            string json = File.Exists(PALETTES_PATH)
    ? File.ReadAllText(PALETTES_PATH)
    : "{palettes:[]}";

            // Десериализация строки в объект
            return JsonConvert.DeserializeObject<FileModel>(json);
        }
        public static void savePalettesData(FileModel data)
        {
            // обновляем список палитр
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(PALETTES_PATH, json);
        }

        // Вспомогательные методы для конвертации цветов
        private System.Drawing.Color ToDrawingColor(Color mediaColor)
        {
            return System.Drawing.Color.FromArgb(
                mediaColor.A,
                mediaColor.R,
                mediaColor.G,
                mediaColor.B);
        }

        private Color ToMediaColor(System.Drawing.Color drawingColor)
        {
            return Color.FromArgb(
                drawingColor.A,
                drawingColor.R,
                drawingColor.G,
                drawingColor.B);
        }
            }


    public class UserModel
    {
        // список палитр
        [JsonProperty("palettes")]
        public List<Palette> Palettes { get; set; }

    }

    public class FileModel
    {
        // список палитр
        [JsonProperty("palettes")]
        public List<Palette> Palettes { get; set; }

    }

    public class Palette
    {
        public string Name { get; set; }  // Теперь это свойство, а не поле!
        public List<string> Colors { get; set; }  // И это тоже свойство
    }

}
