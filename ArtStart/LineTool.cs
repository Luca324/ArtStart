using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart
{
    public class LineTool : Tool
    {
        public override Shape CreateShape(Color color, double thickness)
        {
            return new Line
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = thickness,
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = 0
            };
        }

        public override void OnMouseDown(Shape shape, Point startPoint)
        {
            if (shape is Line line)
            {
                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = startPoint.X;
                line.Y2 = startPoint.Y;
            }
        }

        public override void OnMouseMove(Shape shape, Point startPoint, Point currentPoint)
        {
            if (shape is Line line)
            {
                line.X2 = currentPoint.X;
                line.Y2 = currentPoint.Y;
            }
        }
    }
}