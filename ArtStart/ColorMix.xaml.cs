using System.Windows;
using Scrtwpns.Mixbox;
using System.Windows.Media;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using ArtStart.Models;

namespace ArtStart
{
    public partial class ColorMix : Window
    {
        private System.Windows.Media.Color currentColor = new System.Windows.Media.Color();
        private string currentColorText = "";
        private string currentColorPalette = "";
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
        
        private void DeleteFromPaletteBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"removing color: {currentColorText}");

            var data = PalettesModel.getPalettesData();
            var user = data.Users.FirstOrDefault(u => u.Login == Session.CurrentUser.Login);

            var targetPalette = user.Palettes.Find(p => p.Name == currentColorPalette);

            Console.WriteLine($"palette before: {targetPalette.Colors}");
            targetPalette.Colors.Remove(currentColorText);

            Console.WriteLine($"palette after: {targetPalette.Colors}");
            PalettesModel.savePalettesData(data);

            renderPalettes(user.Palettes);
        }

        private void renderPalettes(List<Palette> palettes)
        {
            Palettes.Children.Clear();

            foreach (var palette in palettes)
            {
                DockPanel panel = new DockPanel();

                Button button = new Button();
                button.Content = "+";
                button.Click += (sender, e) =>
                {
                    Console.WriteLine($"palette name: {palette.Name}");
                    AddCurrentColor(palette.Name);
                };
                button.Style = (Style)this.FindResource("addColorBtn");

                TextBlock block = new TextBlock();
                block.Text = palette.Name;

                WrapPanel colors = new WrapPanel();
                foreach (var color in palette.Colors)
                {
                    Button colorBlock = new Button();
                    var brush  = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
                    colorBlock.Background = brush;
                    colorBlock.Width = 20;
                    colorBlock.Height = 20;
                    colorBlock.Click += (sender, e) =>
                    {
                        currentColorBlock.Background = brush;
                        //currentColorText = JsonConvert.SerializeObject(palette.Colors);
                        currentColorText = color; 
                        currentColorPalette = palette.Name;
                        Console.WriteLine($"current color: {currentColorText}");


                    };
                    setColorBlockStyle(colorBlock);
                    colors.Children.Add(colorBlock);

                }

                panel.Children.Add(button);
                panel.Children.Add(block);
                panel.Children.Add(colors);
                Palettes.Children.Add(panel);
            }

        }
        private void setColorBlockStyle(Button colorBlock)
        {
            // Создаем ControlTemplate
            var controlTemplate = new ControlTemplate(typeof(Button));

            // Создаем фабрику для визуального дерева
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "border";
            borderFactory.SetBinding(Border.BackgroundProperty,
                new Binding("Background") { RelativeSource = RelativeSource.TemplatedParent });

            // Устанавливаем визуальное дерево
            controlTemplate.VisualTree = borderFactory;

            // Создаем стиль
            colorBlock.Style = new Style(typeof(Button))
            {
                Setters =
        {
            new Setter(Button.BorderThicknessProperty, new Thickness(0)),
            new Setter(Button.BackgroundProperty, colorBlock.Background),
            new Setter(Button.TemplateProperty, controlTemplate)
        }
            };
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
