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
        public ColorMix()
        {
            InitializeComponent();
            Challenges.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;


            renderPalettesFromJSON();
        }


        private void button1_Click(object sender, RoutedEventArgs e)
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

                // Устанавливаем фон кнопки
                result.Background = new SolidColorBrush(mixedColor);
            }
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

        private void CreateNewPalette_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(NewPaletteName.Text);

            const string filepath = @"../../data.json";

            // Чтение файла
            var json = File.ReadAllText(filepath);

            // Десериализация строки в объект
            var data = JsonConvert.DeserializeObject<FileModel>(json);

            // Изменение данных (пример)
            data.StringValue = "new value";
            data.IntValue++;
            data.ListValue.Add(NewPaletteName.Text);

            // создание новой палитры
            Palette newPalette = new Palette();

            newPalette.Name = NewPaletteName.Text;
            newPalette.Colors = new List<string>();
            data.Palettes.Add(newPalette);


            // Сериализация объекта в строку
            json = JsonConvert.SerializeObject(data);

            // Сохранение строки в файл
            File.WriteAllText(filepath, json);

            // обновляем список палитр
            renderPalettesFromJSON();

        }

        private void renderPalettesFromJSON()
        {
            Palettes.Children.Clear();
            const string filepath = @"../../data.json";

            // Чтение файла
            var json = File.ReadAllText(filepath);

            // Десериализация строки в объект
            var data = JsonConvert.DeserializeObject<FileModel>(json);

            foreach (var palette in data.Palettes)
            {
                Console.WriteLine(palette.Name);
                TextBlock block = new TextBlock();
                block.Text = palette.Name;
                Palettes.Children.Add(block);
            }

        }


    }
    public class FileModel
    {
        // примеры разных типов полей
        [JsonProperty("stringValue")]
        public string StringValue { get; set; }

        [JsonProperty("numberValue")]
        public int IntValue { get; set; }

        [JsonProperty("listValue")]
        public List<object> ListValue { get; set; }

        // список палитр
        [JsonProperty("palettes")]
        public List<Palette> Palettes { get; set; }

    }

    public class Palette
    {
        public string Name;
        public List<string> Colors;
    }

}
