using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class RectangleTool : Tool
{
    private Point startPoint;
    private Rectangle currentRect;

    public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
    {
        startPoint = e.GetPosition(canvas);
        currentRect = new Rectangle
        {
            Stroke = new SolidColorBrush(Color),
            StrokeThickness = Thickness,
            Fill = Brushes.Transparent
        };
        Canvas.SetLeft(currentRect, startPoint.X);
        Canvas.SetTop(currentRect, startPoint.Y);
        canvas.Children.Add(currentRect);
    }

    public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && currentRect != null)
        {
            var point = e.GetPosition(canvas);
            double width = point.X - startPoint.X;
            double height = point.Y - startPoint.Y;
            currentRect.Width = Math.Abs(width);
            currentRect.Height = Math.Abs(height);
            Canvas.SetLeft(currentRect, width < 0 ? point.X : startPoint.X);
            Canvas.SetTop(currentRect, height < 0 ? point.Y : startPoint.Y);
        }
    }

    public override void OnMouseUp(Canvas canvas, MouseButtonEventArgs e)
    {
        currentRect = null;
    }
}