using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class StarTool : Tool
{
    private Polygon star;

    public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
    {
        var center = e.GetPosition(canvas);
        star = new Polygon
        {
            Points = new PointCollection
            {
                new Point(center.X, center.Y - 30),
                new Point(center.X + 9.27f, center.Y - 9.27f),
                new Point(center.X + 30, center.Y),
                new Point(center.X + 9.27f, center.Y + 9.27f),
                new Point(center.X, center.Y + 30),
                new Point(center.X - 9.27f, center.Y + 9.27f),
                new Point(center.X - 30, center.Y),
                new Point(center.X + 9.27f, center.Y - 9.27f)
            },
            Stroke = new SolidColorBrush(Color),
            StrokeThickness = Thickness,
            Fill = new SolidColorBrush(Color)
        };
        canvas.Children.Add(star);
    }
}