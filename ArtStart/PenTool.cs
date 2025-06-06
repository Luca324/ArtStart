using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class PenTool : Tool
{
    private Point? previousPoint;

    public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
    {
        previousPoint = e.GetPosition(canvas);
    }

    public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && previousPoint.HasValue)
        {
            var currentPoint = e.GetPosition(canvas);
            var line = new Line
            {
                X1 = previousPoint.Value.X,
                Y1 = previousPoint.Value.Y,
                X2 = currentPoint.X,
                Y2 = currentPoint.Y,
                Stroke = new SolidColorBrush(Color),
                StrokeThickness = Thickness
            };
            canvas.Children.Add(line);
            previousPoint = currentPoint;
        }
    }

    public override void OnMouseUp(Canvas canvas, MouseButtonEventArgs e)
    {
        previousPoint = null;
    }
}