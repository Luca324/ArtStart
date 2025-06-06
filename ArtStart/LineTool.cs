using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class LineTool : Tool
{
    private Point? startPoint;
    private Line currentLine;

    public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
    {
        startPoint = e.GetPosition(canvas);
        currentLine = new Line
        {
            X1 = startPoint.Value.X,
            Y1 = startPoint.Value.Y,
            Stroke = new SolidColorBrush(Color),
            StrokeThickness = Thickness
        };
        canvas.Children.Add(currentLine);
    }

    public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && currentLine != null)
        {
            var point = e.GetPosition(canvas);
            currentLine.X2 = point.X;
            currentLine.Y2 = point.Y;
        }
    }

    public override void OnMouseUp(Canvas canvas, MouseButtonEventArgs e)
    {
        currentLine = null;
    }
}