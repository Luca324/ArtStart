using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class EllipseTool : Tool
{
    private Point startPoint;
    private Ellipse currentEllipse;

    public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
    {
        startPoint = e.GetPosition(canvas);
        currentEllipse = new Ellipse
        {
            Stroke = new SolidColorBrush(Color),
            StrokeThickness = Thickness,
            Fill = Brushes.Transparent
        };
        Canvas.SetLeft(currentEllipse, startPoint.X);
        Canvas.SetTop(currentEllipse, startPoint.Y);
        canvas.Children.Add(currentEllipse);
    }

    public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && currentEllipse != null)
        {
            var point = e.GetPosition(canvas);
            double width = point.X - startPoint.X;
            double height = point.Y - startPoint.Y;

            currentEllipse.Width = Math.Abs(width);
            currentEllipse.Height = Math.Abs(height);

            Canvas.SetLeft(currentEllipse, width < 0 ? point.X : startPoint.X);
            Canvas.SetTop(currentEllipse, height < 0 ? point.Y : startPoint.Y);
        }
    }

    public override void OnMouseUp(Canvas canvas, MouseButtonEventArgs e)
    {
        currentEllipse = null;
    }
}