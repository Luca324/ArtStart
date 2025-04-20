using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace april
{
    public class EllipseTool : Tool
    {
        public override Shape CreateShape(Color color, double thickness)
        {
            return new Ellipse
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = thickness,
                Fill = Brushes.Transparent
            };
        }

        public override void OnMouseDown(Shape shape, Point startPoint)
        {
            if (shape is Ellipse ellipse)
            {
                Canvas.SetLeft(ellipse, startPoint.X);
                Canvas.SetTop(ellipse, startPoint.Y);
                ellipse.Width = 0;
                ellipse.Height = 0;
            }
        }

        public override void OnMouseMove(Shape shape, Point startPoint, Point currentPoint)
        {
            if (shape is Ellipse ellipse)
            {
                double width = currentPoint.X - startPoint.X;
                double height = currentPoint.Y - startPoint.Y;

                if (width < 0)
                {
                    Canvas.SetLeft(ellipse, currentPoint.X);
                    width = -width;
                }
                else
                {
                    Canvas.SetLeft(ellipse, startPoint.X);
                }

                if (height < 0)
                {
                    Canvas.SetTop(ellipse, currentPoint.Y);
                    height = -height;
                }
                else
                {
                    Canvas.SetTop(ellipse, startPoint.Y);
                }

                ellipse.Width = width;
                ellipse.Height = height;
            }
        }
    }
}