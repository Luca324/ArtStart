using april;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace april
{
    public partial class Paint : Window
    {
        private Tool currentTool;
        private Point startPoint;
        private Shape currentShape;
        private Color currentColor = Colors.Black;
        private double currentThickness = 1;
        private int sprayDensity = 10;

        public Paint()
        {
            InitializeComponent();
            currentTool = new PenTool();
            colorsComboBox.SelectedIndex = 0;
            thicknessComboBox.SelectedIndex = 0;
            toolsComboBox.SelectedIndex = 0;
            densityComboBox.SelectedIndex = 1;


            Challenges.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;
        }
        private void ToolsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (toolsComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Tag.ToString())
                {
                    case "Pen":
                        currentTool = new PenTool();
                        sprayDensityPanel.Visibility = Visibility.Collapsed;
                        break;
                    case "Line":
                        currentTool = new LineTool();
                        sprayDensityPanel.Visibility = Visibility.Collapsed;
                        break;
                    case "Rectangle":
                        currentTool = new RectangleTool();
                        sprayDensityPanel.Visibility = Visibility.Collapsed;
                        break;
                    case "Ellipse":
                        currentTool = new EllipseTool();
                        sprayDensityPanel.Visibility = Visibility.Collapsed;
                        break;
                    case "Spray":
                        currentTool = new SprayTool();
                        sprayDensityPanel.Visibility = Visibility.Visible;
                        break;
                    case "Eraser":
                        currentTool = new EraserTool();
                        sprayDensityPanel.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void ColorsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colorsComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Background is SolidColorBrush brush)
            {
                currentColor = brush.Color;
            }
        }

        private void ThicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (thicknessComboBox.SelectedItem is ComboBoxItem selectedItem && double.TryParse(selectedItem.Content.ToString(), out double thickness))
            {
                currentThickness = thickness;
            }
        }

        private void DensityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (densityComboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int density))
            {
                sprayDensity = density;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    Image image = new Image { Source = bitmap };
                    drawingCanvas.Children.Clear();
                    drawingCanvas.Children.Add(image);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                        (int)drawingCanvas.ActualWidth,
                        (int)drawingCanvas.ActualHeight,
                        96d, 96d, PixelFormats.Pbgra32);

                    drawingCanvas.Measure(new Size((int)drawingCanvas.ActualWidth, (int)drawingCanvas.ActualHeight));
                    drawingCanvas.Arrange(new Rect(new Size((int)drawingCanvas.ActualWidth, (int)drawingCanvas.ActualHeight)));

                    renderBitmap.Render(drawingCanvas);

                    BitmapEncoder encoder = saveFileDialog.FileName.EndsWith(".png") ?
                        new PngBitmapEncoder() : (BitmapEncoder)new JpegBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (var fileStream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(drawingCanvas);

            if (currentTool is SprayTool sprayTool)
            {
                sprayTool.SetParameters(currentColor, currentThickness, sprayDensity);
                sprayTool.OnMouseDown(drawingCanvas, startPoint);
            }
            else
            {
                currentShape = currentTool.CreateShape(currentColor, currentThickness);
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
                var currentPoint = e.GetPosition(drawingCanvas);

                if (currentTool is SprayTool sprayTool)
                {
                    sprayTool.OnMouseMove(drawingCanvas, currentPoint);
                }
                else if (currentShape != null)
                {
                    currentTool.OnMouseMove(currentShape, startPoint, currentPoint);
                }
            }
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentShape = null;
        }
    }
}