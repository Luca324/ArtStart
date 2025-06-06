using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Forms; // Для ColorDialog (из сборки System.Windows.Forms)
using System.IO;
using System.Windows.Media.Imaging; // Для SaveImage_Click (RenderTargetBitmap, PngBitmapEncoder)
using System.Windows.Documents; // Если используется RichTextBox или TextBlock

namespace ArtStart
{
    public partial class Paint : Window
    {
        private Tool currentTool;
        private TextBox activeTextBox = null;

        public System.Drawing.Color SelectedColor { get; set; } = System.Drawing.Color.Black;

        public Paint()
        {
            InitializeComponent();
            drawingCanvas.Focus();

            toolComboBox.SelectionChanged += (s, e) => ChangeTool();
            thicknessSlider.ValueChanged += (s, e) => UpdateToolProperties();
            drawingCanvas.KeyDown += Canvas_KeyDown;
        }

        private void ChangeTool()
        {
            switch (toolComboBox.SelectedIndex)
            {
                case 0: currentTool = new PenTool(); break;
                case 1: currentTool = new LineTool(); break;
                case 2: currentTool = new RectangleTool(); break;
                case 3: currentTool = new EllipseTool(); break;
                case 4: currentTool = new EraserTool(); break;
                case 5: currentTool = new SprayTool(); break;
                case 6: currentTool = new FillTool(); break;
                case 7: currentTool = new StarTool(); break;
                default: currentTool = new PenTool(); break;
            }
            UpdateToolProperties();
        }

        private void UpdateToolProperties()
        {
            if (currentTool != null)
            {
                currentTool.Color = SelectedColor.ToMediaColor();
                currentTool.Thickness = (int)thicknessSlider.Value;
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, что клик внутри холста
            Point position = e.GetPosition(drawingCanvas);
            if (IsPointInCanvas(position))
            {
                currentTool.OnMouseDown(drawingCanvas, e);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(drawingCanvas);
                if (IsPointInCanvas(position))
                {
                    currentTool.OnMouseMove(drawingCanvas, e);
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentTool.OnMouseUp(drawingCanvas, e);
        }

        private bool IsPointInCanvas(Point point)
        {
            return point.X >= 0 && point.Y >= 0 &&
                   point.X <= drawingCanvas.ActualWidth &&
                   point.Y <= drawingCanvas.ActualHeight;
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Drawing";
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG Files (*.png)|*.png";

            if (dialog.ShowDialog() == true)
            {
                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)drawingCanvas.ActualWidth,
                    (int)drawingCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                bitmap.Render(drawingCanvas);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (var stream = File.Create(dialog.FileName))
                {
                    encoder.Save(stream);
                }
            }
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                BitmapImage image = new BitmapImage(new Uri(dialog.FileName));
                Image uiImage = new Image { Source = image };
                Canvas.SetLeft(uiImage, 0);
                Canvas.SetTop(uiImage, 0);
                drawingCanvas.Children.Add(uiImage);
            }
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
        }

        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            activeTextBox = new TextBox
            {
                Width = 150,
                Height = 30,
                FontFamily = new FontFamily(fontComboBox.Text),
                FontSize = thicknessSlider.Value,
                Foreground = new SolidColorBrush(SelectedColor.ToMediaColor()),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Gray,
                AcceptsReturn = false
            };

            activeTextBox.KeyDown += (s, args) =>
            {
                if (args.Key == Key.Enter)
                {
                    FixText(activeTextBox);
                }
            };

            drawingCanvas.Children.Add(activeTextBox);
            drawingCanvas.MouseLeftButtonDown += PlaceTextOnCanvas;
        }

        private void PlaceTextOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (activeTextBox != null)
            {
                Point position = e.GetPosition(drawingCanvas);
                if (IsPointInCanvas(position))
                {
                    Canvas.SetLeft(activeTextBox, position.X);
                    Canvas.SetTop(activeTextBox, position.Y);
                    drawingCanvas.MouseLeftButtonDown -= PlaceTextOnCanvas;
                }
            }
        }

        private void FixText(TextBox textBox)
        {
            if (textBox != null)
            {
                drawingCanvas.MouseLeftButtonDown -= PlaceTextOnCanvas;

                TextBlock textBlock = new TextBlock
                {
                    Text = textBox.Text,
                    FontFamily = textBox.FontFamily,
                    FontSize = textBox.FontSize,
                    Foreground = textBox.Foreground
                };

                Canvas.SetLeft(textBlock, Canvas.GetLeft(textBox));
                Canvas.SetTop(textBlock, Canvas.GetTop(textBox));

                drawingCanvas.Children.Remove(textBox);
                drawingCanvas.Children.Add(textBlock);

                activeTextBox = null;
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SelectedColor = dialog.Color;
                UpdateToolProperties();
            }
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && activeTextBox != null)
            {
                FixText(activeTextBox);
            }
        }
    }

    public static class ColorExtensions
    {
        public static Color ToMediaColor(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}