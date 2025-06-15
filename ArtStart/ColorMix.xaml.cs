using System.Windows;
using Scrtwpns.Mixbox;
using System.Windows.Media;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ArtStart
{
    public partial class ColorMix : Window
    {
        private System.Windows.Media.Color currentColor = new System.Windows.Media.Color();
        private Boolean currentColorExists = false;

        public ColorMix()
        {
            InitializeComponent();

            Challenges.Click += Utils.Navigation_Click;
            Paint.Click += Utils.Navigation_Click;
            LogOut.Click += Utils.LogOut;

            var data = PalettesModel.getPalettesData();
            var user = data.Users.Find(u => u.Login == Session.CurrentUser.Login);

            if (user != null) renderPalettes(user.Palettes);
        }


        private void MixColors(object sender, RoutedEventArgs e)
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
                var brush = new SolidColorBrush(mixedColor);
                result.Background = brush;
                currentColorBlock.Background = brush;
                currentColorExists = true;
            }
        }

        private void CreateNewPalette_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewPaletteName.Text)) return;
            Console.WriteLine(NewPaletteName.Text);

            // Чтение файла
            var data = PalettesModel.getPalettesData();

            var user = data.Users.FirstOrDefault(u => u.Login == Session.CurrentUser.Login);

            // создание новой палитры
            Palette newPalette = new Palette();

            newPalette.Name = NewPaletteName.Text;
            newPalette.Colors = new List<string>();
            user.Palettes.Add(newPalette);


            PalettesModel.savePalettesData(data);

            // обновляем список палитр
            renderPalettes(user.Palettes);

            NewPaletteName.Text = "";
        }

        private void renderPalettes(List<Palette> palettes)
        {
            Palettes.Children.Clear();

            foreach (var palette in palettes)
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

            var data = PalettesModel.getPalettesData();
            var user = data.Users.FirstOrDefault(u => u.Login == Session.CurrentUser.Login);

            var targetPalette = user.Palettes.Find(p => p.Name == palette);

            // если цвет уже есть
            if (targetPalette.Colors.Contains(currentColor.ToString())) return;

            targetPalette.Colors.Add(currentColor.ToString());
            PalettesModel.savePalettesData(data);

            renderPalettes(user.Palettes);

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

}
