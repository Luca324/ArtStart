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
        private const string PALETTES_PATH = @"../../palettes.json";

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

            Challenges.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;
            LogOut.Click += Utils.LogOut;

            RenderPalettesFromJSON();
        }

        private void RenderPalettesFromJSON()
        {

            string json = File.Exists(PALETTES_PATH) 
    ? File.ReadAllText(PALETTES_PATH)
    : "{palettes:[]}";
            var data = JsonConvert.DeserializeObject<PalettesFileModel>(json);
            var user = data.Users.FirstOrDefault(u => u.Login == Session.CurrentUser.Login);


            if (user?.Palettes != null)
            {
                foreach (var palette in user.Palettes)
                {
                    Console.WriteLine($"Palette: {palette.Name}, Colors: {palette.Colors.Count}");
                }

                PaletteCombo.ItemsSource = user.Palettes;
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
            if (e.LeftButton != MouseButtonState.Pressed || currentShape == null && !(currentTool is SprayTool))
                return;

            Point currentPoint = e.GetPosition(drawingCanvas);

            // Проверка: находится ли текущая точка внутри Canvas
            if (currentPoint.X < 0 || currentPoint.Y < 0 ||
                currentPoint.X > drawingCanvas.ActualWidth ||
                currentPoint.Y > drawingCanvas.ActualHeight)
            {
                return; // вышли за границы — не рисуем
            }

            if (currentTool is SprayTool spray)
            {
                spray.OnMouseMove(drawingCanvas, currentPoint);
            }
            else if (currentShape != null)
            {
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
        private TextBox editableTextBox = null;
        private bool isAddingText = false;
        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            isAddingText = true;
            Cursor = Cursors.IBeam;
            drawingCanvas.Cursor = Cursors.IBeam;

            // Подписываемся на клик по Canvas
            drawingCanvas.MouseDown -= DrawingCanvas_TextStart_MouseDown;
            drawingCanvas.MouseDown += DrawingCanvas_TextStart_MouseDown;
        }
        private void DrawingCanvas_TextStart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isAddingText) return;

            Point clickPoint = e.GetPosition(drawingCanvas);

            // Создаем TextBox
            editableTextBox = new TextBox
            {
                Text = "",
                FontFamily = new FontFamily(((ComboBoxItem)fontsComboBox.SelectedItem)?.Content?.ToString() ?? "Arial"),
                FontSize = (int)thicknessSlider.Value,
                Foreground = new SolidColorBrush(selectedColor),
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Width = 200,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                AcceptsReturn = false
            };

            TranslateTransform transform = new TranslateTransform(clickPoint.X, clickPoint.Y);
            editableTextBox.RenderTransform = transform;

            textCanvas.Children.Add(editableTextBox);
            editableTextBox.Focus();

            // Подписываемся на Enter и Esc
            editableTextBox.KeyDown -= EditableTextBox_KeyDown; // Защита от дублирования
            editableTextBox.KeyDown += EditableTextBox_KeyDown;

            isAddingText = false;
            Cursor = Cursors.Arrow;
            drawingCanvas.Cursor = Cursors.Arrow;
        }
        private void EditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveEditableText();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                textCanvas.Children.Remove(editableTextBox);
                editableTextBox = null;
                e.Handled = true;
            }
        }
        private void SaveEditableText()
        {
            if (editableTextBox == null) return;

            string text = editableTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                textCanvas.Children.Remove(editableTextBox);
                editableTextBox = null;
                return;
            }

            var staticText = new TextBlock
            {
                Text = text,
                FontFamily = editableTextBox.FontFamily,
                FontSize = editableTextBox.FontSize,
                Foreground = editableTextBox.Foreground
            };

            double left = ((TranslateTransform)editableTextBox.RenderTransform).X;
            double top = ((TranslateTransform)editableTextBox.RenderTransform).Y;

            Canvas.SetLeft(staticText, left);
            Canvas.SetTop(staticText, top);

            drawingCanvas.Children.Add(staticText);
            textCanvas.Children.Remove(editableTextBox);
            editableTextBox = null;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image Files (*.png, *.jpg)|*.png;*.jpg";
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Диалог сохранения файла
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JPEG Image|*.jpg",
                    Title = "Сохранить как JPG"
                };

                if (dialog.ShowDialog() != true)
                    return;

                // Получаем размеры Canvas
                int width = (int)Math.Ceiling(drawingCanvas.ActualWidth);
                int height = (int)Math.Ceiling(drawingCanvas.ActualHeight);

                if (width <= 0 || height <= 0)
                {
                    MessageBox.Show("Невозможно сохранить изображение: холст имеет нулевой размер.");
                    return;
                }

                // Рендерим Canvas в RenderTargetBitmap
                var renderBitmap = new RenderTargetBitmap(
                    width, height,
                    96d, 96d,
                    PixelFormats.Pbgra32
                );

                drawingCanvas.Measure(new Size(width, height));
                drawingCanvas.Arrange(new Rect(0, 0, width, height));
                renderBitmap.Render(drawingCanvas);

                // Кодируем в JPG
                var encoder = new JpegBitmapEncoder { QualityLevel = 90 }; // качество 90%
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                // Сохраняем файл
                using (var fileStream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                MessageBox.Show("Изображение успешно сохранено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
           
        }

        private void Paint_KeyDown(object sender, KeyEventArgs e)
        {
            // Отмена последнего действия
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

            
            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.None)
            {
                foreach (var child in drawingCanvas.Children)
                {
                    if (child is Shape shape)
                    {
                        if (!shape.IsMouseCaptured)
                        {
                            shape.MouseLeftButtonDown -= Shape_MouseLeftButtonDown_Select; // Чтобы не накапливались события
                            shape.MouseLeftButtonDown += Shape_MouseLeftButtonDown_Select;
                        }
                    }
                }
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                Clipboard.Clear();
                foreach (var el in selectedElements)
                {
                    if (el is Shape shape && shape.Parent != null)
                    {
                        Clipboard.SetData($"Shape_{shape.GetType().Name}", shape.Clone());
                    }
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
                            if (cloned != null)
                            {
                                Canvas.SetLeft(cloned, Canvas.GetLeft(cloned) + 10);
                                Canvas.SetTop(cloned, Canvas.GetTop(cloned) + 10);
                                drawingCanvas.Children.Add(cloned);
                            }
                        }
                    }
                }
            }
        }

        private void Shape_MouseLeftButtonDown_Select(object sender, MouseButtonEventArgs e)
        {
            if (sender is Shape shape && !selectedElements.Contains(shape))
            {
                selectedElements.Add(shape);
            }
        }
    }
    
}