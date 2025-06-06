using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class FillTool : Tool
{
    private Point? startPoint;
    private Rectangle fillRectangle;

    public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
    {
        // Получаем координаты клика
        var clickPoint = e.GetPosition(canvas);

        // Проверяем, на каком элементе был клик
        UIElement target = canvas.InputHitTest(clickPoint) as UIElement;

        if (target is Shape shape)
        {
            // Заливаем фигуру выбранным цветом
            shape.Fill = new SolidColorBrush(Color);
        }
        else
        {
            // Если клик вне фигуры — создаём новую заливку
            CreateFillOverlay(canvas, clickPoint);
        }
    }

    private void CreateFillOverlay(Canvas canvas, Point point)
    {
        // Создаём полупрозрачный прямоугольник для визуализации заливки
        fillRectangle = new Rectangle
        {
            Width = 0,
            Height = 0,
            Fill = new SolidColorBrush(Color),
            Opacity = 0.5
        };

        Canvas.SetLeft(fillRectangle, point.X);
        Canvas.SetTop(fillRectangle, point.Y);
        canvas.Children.Add(fillRectangle);
        startPoint = point;
    }

    public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && fillRectangle != null)
        {
            var currentPoint = e.GetPosition(canvas);
            double width = currentPoint.X - startPoint.Value.X;
            double height = currentPoint.Y - startPoint.Value.Y;

            fillRectangle.Width = Math.Abs(width);
            fillRectangle.Height = Math.Abs(height);

            Canvas.SetLeft(fillRectangle, width < 0 ? currentPoint.X : startPoint.Value.X);
            Canvas.SetTop(fillRectangle, height < 0 ? currentPoint.Y : startPoint.Value.Y);
        }
    }

    public override void OnMouseUp(Canvas canvas, MouseButtonEventArgs e)
    {
        if (fillRectangle != null)
        {
            // Убираем прозрачность после отпускания мыши
            fillRectangle.Opacity = 1.0;
            fillRectangle = null;
        }
    }
}