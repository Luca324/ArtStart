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
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace ArtStart
{
    public partial class Paint : Window
    {
        // Путь к файлу JSON с палитрами
        private const string PALETTES_PATH = @"../../palettes.json";

        // Текущий выбранный инструмент
        private Tool currentTool;

        // Фигура, которая рисуется в текущем действии
        private Shape currentShape;

        // Начальная точка мыши при рисовании
        private Point startPoint;

        // Блок текста, который можно перемещать
        private TextBlock movingTextBlock = null;

        // Вспомогательный Canvas для временного хранения текстовых полей
        private Canvas textCanvas = new Canvas();

        // Стек для отмены действий (Ctrl+Z)
        private Stack<UIElement> undoStack = new Stack<UIElement>();

        // Список выделенных элементов (для копирования/вставки)
        private List<UIElement> selectedElements = new List<UIElement>();

        // Цвет, выбранный пользователем
        private Color selectedColor = Colors.Black;

        // Толщина линий/инструментов
        private double selectedThickness = 5;

        // Объект спрея (распыления краски)
        private SprayTool sprayTool = new SprayTool();

        // Конструктор — инициализация окна и событий
        public Paint()
        {
            InitializeComponent();
            InitializeTools();               // Установить начальный инструмент
            InitializeFonts();                 // Заполнить список шрифтов
            thicknessSlider.Value = selectedThickness; // Установить слайдер толщины
            this.KeyDown += Paint_KeyDown;     // Поддержка горячих клавиш
            drawingCanvas.Children.Add(textCanvas); // Добавить текстовый canvas

            // Обработчики нажатия на кнопки
            Challenges.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;
            MainWindow.Click += Utils.Navigation_Click;
            LogOut.Click += Utils.LogOut;

            RenderPalettesFromJSON();         // Загрузить палитры из файла
        }

        // Загружает палитры из JSON-файла и устанавливает в ComboBox
        private void RenderPalettesFromJSON()
        {
            string json = File.Exists(PALETTES_PATH) ? File.ReadAllText(PALETTES_PATH) : "{palettes:[]}";
            var data = JsonConvert.DeserializeObject<PalettesFileModel>(json);
            var user = data.Users.FirstOrDefault(u => u.Login == Session.CurrentUser.Login);

            if (user?.Palettes != null)
            {
                PaletteCombo.ItemsSource = user.Palettes;
            }
            else
            {
                MessageBox.Show("Ошибка загрузки палитр");
            }
        }

        // Установить начальный инструмент (по умолчанию первый в списке)
        private void InitializeTools()
        {
            toolsComboBox.SelectedIndex = 0;
        }

        // Выбор цвета из палитры по клику
        private void PaletteCombo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Border border && border.DataContext is string hexColor)
            {
                selectedColor = (Color)ColorConverter.ConvertFromString(hexColor);
                PaletteCombo.IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        // Смена инструмента через выпадающий список
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

        // Обновление выбранного цвета при изменении в пикере
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColorPicker.SelectedColor.HasValue)
            {
                selectedColor = ColorPicker.SelectedColor.Value;
            }
        }

        // Обновление толщины линии при изменении слайдера
        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (thicknessValueText != null)
                thicknessValueText.Text = ((int)thicknessSlider.Value).ToString();
            selectedThickness = thicknessSlider.Value;
            if (sprayTool != null)
                sprayTool.SetParameters(selectedColor, selectedThickness, 20);
        }

        // Начало рисования при нажатии мыши
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

        // Продолжение рисования при движении мыши
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || currentShape == null && !(currentTool is SprayTool))
                return;

            Point currentPoint = e.GetPosition(drawingCanvas);

            if (currentPoint.X < 0 || currentPoint.Y < 0 ||
                currentPoint.X > drawingCanvas.ActualWidth ||
                currentPoint.Y > drawingCanvas.ActualHeight)
            {
                return;
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

        // Завершение рисования при отпускании мыши
        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentShape != null)
            {
                undoStack.Push(currentShape);
                currentShape = null;
            }
        }

        // Получает цвет пикселя под курсором (для заливки)
        private Color GetPixelColor(Point point)
        {
            var bitmap = new RenderTargetBitmap((int)drawingCanvas.ActualWidth, (int)drawingCanvas.ActualHeight, 96, 96, PixelFormats.Default);
            bitmap.Render(drawingCanvas);
            var pixels = new byte[4];
            bitmap.CopyPixels(new Int32Rect((int)point.X, (int)point.Y, 1, 1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        // TextBox для ввода текста
        private TextBox editableTextBox = null;

        // Флаг, указывающий, что пользователь хочет добавить текст
        private bool isAddingText = false;

        // Активировать режим добавления текста
        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            isAddingText = true;
            Cursor = Cursors.IBeam;
            drawingCanvas.Cursor = Cursors.IBeam;

            drawingCanvas.MouseDown -= DrawingCanvas_TextStart_MouseDown;
            drawingCanvas.MouseDown += DrawingCanvas_TextStart_MouseDown;
        }

        // Создание текстового поля при клике на холсте
        private void DrawingCanvas_TextStart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isAddingText) return;
            Point clickPoint = e.GetPosition(drawingCanvas);

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

            editableTextBox.KeyDown -= EditableTextBox_KeyDown;
            editableTextBox.KeyDown += EditableTextBox_KeyDown;

            isAddingText = false;
            Cursor = Cursors.Arrow;
            drawingCanvas.Cursor = Cursors.Arrow;
        }

        // Обработка Enter (сохранить) 
        private void EditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveEditableText();
                e.Handled = true;
            }
           
        }

        // Сохранение текста как TextBlock на основном Canvas
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

        // Открытие изображения
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
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

        // Сохранение холста как JPG
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog { Filter = "JPEG Image|*.jpg", Title = "Сохранить как JPG" };
                if (dialog.ShowDialog() != true) return;

                int width = (int)Math.Ceiling(drawingCanvas.ActualWidth);
                int height = (int)Math.Ceiling(drawingCanvas.ActualHeight);
                if (width <= 0 || height <= 0)
                {
                    MessageBox.Show("Невозможно сохранить изображение: холст имеет нулевой размер.");
                    return;
                }

                var renderBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);
                drawingCanvas.Measure(new Size(width, height));
                drawingCanvas.Arrange(new Rect(0, 0, width, height));
                renderBitmap.Render(drawingCanvas);

                var encoder = new JpegBitmapEncoder { QualityLevel = 90 };
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

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

        // Очистка холста
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
            textCanvas.Children.Clear();
        }

        // Инициализация списка шрифтов
        private void InitializeFonts()
        {
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Arial" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Times New Roman" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Courier New" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Verdana" });
            fontsComboBox.Items.Add(new ComboBoxItem { Content = "Comic Sans MS" });
            fontsComboBox.SelectedIndex = 0;
        }

        // Изменение шрифта (пока не реализовано)
        private void FontsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        // Обработка горячих клавиш (Ctrl+Z)
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

            
        }

        // Выбор фигуры при клике
        private void Shape_MouseLeftButtonDown_Select(object sender, MouseButtonEventArgs e)
        {
            if (sender is Shape shape && !selectedElements.Contains(shape))
            {
                selectedElements.Add(shape);
            }
        }
    }
}