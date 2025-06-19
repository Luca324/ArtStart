using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart.Tools
{
    public class RectangleTool : Tool
    {
        public override Shape CreateShape(Color color, double thickness)
        {
            return new Rectangle
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = thickness,
                Fill = Brushes.Transparent
            };
        }

        public override void OnMouseDown(Shape shape, Point startPoint)
        {
            if (shape is Rectangle rectangle)
            {
                Canvas.SetLeft(rectangle, startPoint.X);
                Canvas.SetTop(rectangle, startPoint.Y);
                rectangle.Width = 0;
                rectangle.Height = 0;
            }
        }

        public override void OnMouseMove(Shape shape, Point startPoint, Point currentPoint)
        {
            if (shape is Rectangle rectangle)
            {
                double width = currentPoint.X - startPoint.X;
                double height = currentPoint.Y - startPoint.Y;
                if (width < 0)
                {
                    Canvas.SetLeft(rectangle, currentPoint.X);
                    width = -width;
                }
                else
                {
                    Canvas.SetLeft(rectangle, startPoint.X);
                }
                if (height < 0)
                {
                    Canvas.SetTop(rectangle, currentPoint.Y);
                    height = -height;
                }
                else
                {
                    Canvas.SetTop(rectangle, startPoint.Y);
                }
                rectangle.Width = width;
                rectangle.Height = height;
            }
        }
    }
}