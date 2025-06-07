using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace ArtStart
{
    public partial class Paint : Window
    {
        private const string PALLETES_PATH = @"../../data.json";

        private Tool currentTool;
        private Shape currentShape;
        private Point startPoint;
        private TextBlock movingTextBlock = null;
        private Canvas textCanvas = new Canvas();

        private Stack<UIElement> undoStack = new Stack<UIElement>();
        private List<UIElement> selectedElements = new List<UIElement>();

        private Color selectedColor = Colors.Black;
        private double selectedThickness = 5;

        private SprayTool sprayTool = new SprayTool();

        public Paint()
        {
            InitializeComponent();
            InitializeTools();
            InitializeFonts();
            thicknessSlider.Value = selectedThickness;
            this.KeyDown += Paint_KeyDown;
            drawingCanvas.Children.Add(textCanvas);

            InitializeComponent();
            Challenges.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;

            RenderPalettesFromJSON();
        }

        private void RenderPalettesFromJSON()
        {
            // Чтение файла (замените на ваш путь)
            var json = File.ReadAllText(PALLETES_PATH);
            var data = JsonConvert.DeserializeObject<FileModel>(json);

            // Проверка данных перед привязкой
            if (data?.Palettes != null)
            {
                foreach (var palette in data.Palettes)
                {
                    Console.WriteLine($"Palette: {palette.Name}, Colors: {palette.Colors.Count}");
                }

                PaletteCombo.ItemsSource = data.Palettes;
            }
            else
            {
                MessageBox.Show("Ошибка загрузки палитр");
            }
        }

        private void InitializeTools()
        {
            toolsComboBox.SelectedIndex = 0;
        }
        private void PaletteCombo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Border border && border.DataContext is string hexColor)
            {
                selectedColor = (Color)ColorConverter.ConvertFromString(hexColor);

                // Закрываем ComboBox
                PaletteCombo.IsDropDownOpen = false;
                e.Handled = true;
            }
        }
        private void ToolsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string toolName = ((ComboBoxItem)toolsComboBox.SelectedItem)?.Tag?.ToString();
            switch (toolName)
            {
                case "Pen": currentTool = new PenTool(); break;
                case "Line": currentTool = new LineTool(); break;
                case "Rectangle": currentTool = new RectangleTool(); break;
                case "Ellipse": currentTool = new EllipseTool(); break;
                case "Spray": currentTool = sprayTool; break;
                case "Eraser": currentTool = new EraserTool(); break;
                case "Fill": currentTool = new FillTool(); break;
                case "Star": currentTool = new StarTool(); break;
            }
        }

        

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColorPicker.SelectedColor.HasValue)
            {
                selectedColor = ColorPicker.SelectedColor.Value;
                
            }

        }
        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (thicknessValueText != null)
                thicknessValueText.Text = ((int)thicknessSlider.Value).ToString();
            selectedThickness = thicknessSlider.Value;
            if (sprayTool != null)
                sprayTool.SetParameters(selectedColor, selectedThickness, 20);
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(drawingCanvas);
            if (currentTool is SprayTool spray)
                spray.OnMouseDown(drawingCanvas, startPoint);
            else if (currentTool is FillTool fill)
            {
                var pixelColor = GetPixelColor(startPoint);
                fill.FloodFill(drawingCanvas, startPoint, pixelColor, selectedColor);
            }
            else
            {
                currentShape = currentTool.CreateShape(selectedColor, selectedThickness);
                if (currentShape != null)
                {
                    drawingCanvas.Children.Add(currentShape);
                    currentTool.OnMouseDown(currentShape, startPoint);
                }
            }
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(drawingCanvas);
                if (currentTool is SprayTool spray)
                    spray.OnMouseMove(drawingCanvas, currentPoint);
                else if (currentShape != null)
                    currentTool.OnMouseMove(currentShape, startPoint, currentPoint);
            }
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentShape != null)
            {
                undoStack.Push(currentShape);
                currentShape = null;
            }
        }

        private Color GetPixelColor(Point point)
        {
            var bitmap = new RenderTargetBitmap((int)drawingCanvas.ActualWidth, (int)drawingCanvas.ActualHeight, 96, 96, PixelFormats.Default);
            bitmap.Render(drawingCanvas);
            var pixels = new byte[4];
            bitmap.CopyPixels(new Int32Rect((int)point.X, (int)point.Y, 1, 1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            string fontFamilyName = ((ComboBoxItem)fontsComboBox.SelectedItem)?.Content?.ToString() ?? "Arial";
            movingTextBlock = new TextBlock
            {
                Text = "New Text",
                FontFamily = new FontFamily(fontFamilyName),
                FontSize = 20,
                Foreground = new SolidColorBrush(selectedColor),
                IsManipulationEnabled = true,
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            TranslateTransform transform = new TranslateTransform(100, 100);
            movingTextBlock.RenderTransform = transform;
            Canvas.SetLeft(movingTextBlock, 100);
            Canvas.SetTop(movingTextBlock, 100);

            textCanvas.Children.Add(movingTextBlock);

            movingTextBlock.MouseLeftButtonDown += (s, args) =>
            {
                textCanvas.CaptureMouse();
                startPoint = args.GetPosition(textCanvas);
            };

            movingTextBlock.MouseMove += (s, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed && movingTextBlock != null)
                {
                    Point currentPoint = args.GetPosition(textCanvas);
                    double offsetX = currentPoint.X - startPoint.X;
                    double offsetY = currentPoint.Y - startPoint.Y;
                    var t = (TranslateTransform)movingTextBlock.RenderTransform;
                    t.X += offsetX;
                    t.Y += offsetY;
                    startPoint = currentPoint;
                }
            };

            movingTextBlock.KeyDown += (s, args) =>
            {
                if (args.Key == Key.Enter)
                {
                    // Переводим в статичный текст
                    var staticText = new TextBlock
                    {
                        Text = movingTextBlock.Text,
                        FontFamily = movingTextBlock.FontFamily,
                        FontSize = movingTextBlock.FontSize,
                        Foreground = movingTextBlock.Foreground,
                        IsHitTestVisible = false
                    };
                    Canvas.SetLeft(staticText, Canvas.GetLeft(movingTextBlock));
                    Canvas.SetTop(staticText, Canvas.GetTop(movingTextBlock));
                    drawingCanvas.Children.Add(staticText);
                    textCanvas.Children.Remove(movingTextBlock);
                    movingTextBlock = null;
                    textCanvas.ReleaseMouseCapture();
                }
            };
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image files (*.png)|*.png";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(dialog.FileName));
                    Image image = new Image { Source = bitmap };
                    drawingCanvas.Children.Clear();
                    drawingCanvas.Children.Add(image);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Сохранить как PNG"
            };

            if (dialog.ShowDialog() == true)
            {
                // Получаем размеры холста
                int width = (int)drawingCanvas.ActualWidth;
                int height = (int)drawingCanvas.ActualHeight;

                // Асинхронное сохранение файла
                await Task.Run(() =>
                {
                    // Рендерим Canvas в RenderTargetBitmap
                    var renderBitmap = new RenderTargetBitmap(
                        width, height,
                        96d, 96d,
                        PixelFormats.Pbgra32
                    );

                    // Так как drawingCanvas — это UI-элемент, обращаемся к нему через Dispatcher
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        drawingCanvas.Measure(new Size(width, height));
                        drawingCanvas.Arrange(new Rect(0, 0, width, height));
                        renderBitmap.Render(drawingCanvas);
                    });

                    // Кодируем в PNG
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    // Сохраняем файл
                    using (var fileStream = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                });
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
            textCanvas.Children.Clear();
        }

        private void InitializeFonts()
        {
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Arial" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Times New Roman" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Courier New" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Verdana" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Comic Sans MS" });
            fontsComboBox.SelectedIndex = 0;
        }

        private void FontsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно использовать для обновления текущего шрифта при добавлении текста
        }

        private void Paint_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                if (undoStack.Count > 0)
                {
                    UIElement element = undoStack.Pop();
                    if (element is Shape shape)
                        drawingCanvas.Children.Remove(shape);
                    else if (element is UIElement uiEl)
                        drawingCanvas.Children.Remove(uiEl);
                }
            }

            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                foreach (var child in drawingCanvas.Children)
                {
                    if (child is Shape shape)
                    {
                        shape.MouseLeftButtonDown += (s, args) =>
                        {
                            if (!selectedElements.Contains(shape))
                                selectedElements.Add(shape);
                        };
                    }
                }
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                Clipboard.Clear();
                foreach (var el in selectedElements)
                {
                    if (el is Shape shape)
                        Clipboard.SetData($"Shape_{shape.GetType().Name}", shape.Clone());
                }
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                foreach (var format in Clipboard.GetDataObject().GetFormats())
                {
                    if (format.StartsWith("Shape_"))
                    {
                        if (Clipboard.GetData(format) is Shape shape)
                        {
                            var cloned = shape.Clone() as Shape;
                            Canvas.SetLeft(cloned, Canvas.GetLeft(cloned) + 10);
                            Canvas.SetTop(cloned, Canvas.GetTop(cloned) + 10);
                            drawingCanvas.Children.Add(cloned);
                        }
                    }
                }
            }
        }
    }
}