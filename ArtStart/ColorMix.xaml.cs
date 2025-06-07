using System.Windows;
using Scrtwpns.Mixbox;
using System.Windows.Media;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ArtStart
{
    public partial class ColorMix : Window
    {
        private const string PALLETES_PATH = @"../../data.json";
        private System.Windows.Media.Color currentColor = new System.Windows.Media.Color();
        private Boolean currentColorExists = false;

        public ColorMix()
        {
            InitializeComponent();
            Challenges.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;


            renderPalettesFromJSON();
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

            // Чтение файла
            var json = File.ReadAllText(PALLETES_PATH);

            // Десериализация строки в объект
            var data = JsonConvert.DeserializeObject<FileModel>(json);

            // создание новой палитры
            Palette newPalette = new Palette();

            newPalette.Name = NewPaletteName.Text;
            newPalette.Colors = new List<string>();
            data.Palettes.Add(newPalette);


            // Сериализация объекта в строку
            json = JsonConvert.SerializeObject(data);

            // Сохранение строки в файл
            File.WriteAllText(PALLETES_PATH, json);

            // обновляем список палитр
            renderPalettesFromJSON();

            NewPaletteName.Text = "";
        }

        private void renderPalettesFromJSON()
        {
            Palettes.Children.Clear();


            // Чтение файла
            var json = File.ReadAllText(PALLETES_PATH);

            // Десериализация строки в объект
            var data = JsonConvert.DeserializeObject<FileModel>(json);

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
            // тут проверка, что цвета еще нет в этой палитре, иначе тоже return

            Console.WriteLine($"palette, currentcolor, isthere color: {palette} {currentColor.ToString()}, {string.IsNullOrEmpty(currentColor.ToString())}");
            // Чтение файла
            var json = File.ReadAllText(PALLETES_PATH);

            // Десериализация строки в объект
            var data = JsonConvert.DeserializeObject<FileModel>(json);

            var targetPalette = data.Palettes.Find(p => p.Name == palette);
            Console.WriteLine($"existing colors:{targetPalette.Colors}\n contains: {!targetPalette.Colors.Contains(currentColor.ToString())}");
            if (targetPalette.Colors.Contains(currentColor.ToString())) return;

            targetPalette.Colors.Add(currentColor.ToString());

            // Сериализация объекта в строку
            json = JsonConvert.SerializeObject(data);

            // Сохранение строки в файл
            File.WriteAllText(PALLETES_PATH, json);

            // обновляем список палитр
            renderPalettesFromJSON();

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
