using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart.Tools
{
    public class StarTool : Tool
    {
        public override Shape CreateShape(Color color, double thickness)
        {
            return new Polygon
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = thickness,
                Fill = Brushes.Transparent
            };
        }

        public override void OnMouseDown(Shape shape, Point startPoint)
        {
            if (shape is Polygon polygon)
            {
                Canvas.SetLeft(polygon, startPoint.X);
                Canvas.SetTop(polygon, startPoint.Y);
            }
        }

        public override void OnMouseMove(Shape shape, Point startPoint, Point currentPoint)
        {
            if (shape is Polygon polygon)
            {
                double width = currentPoint.X - startPoint.X;
                double height = currentPoint.Y - startPoint.Y;
                double radius = Math.Min(Math.Abs(width), Math.Abs(height)) / 2;
                polygon.Points = CreateStarPoints(5, radius, radius / 2, startPoint);
            }
        }

        private PointCollection CreateStarPoints(int numPoints, double outerRadius, double innerRadius, Point center)
        {
            PointCollection points = new PointCollection();
            double angle = 0;
            double angleIncrement = Math.PI / numPoints;

            for (int i = 0; i < 2 * numPoints; i++)
            {
                double r = (i % 2 == 0) ? outerRadius : innerRadius;
                double x = center.X + r * Math.Sin(angle);
                double y = center.Y + r * Math.Cos(angle);
                points.Add(new Point(x, y));
                angle += angleIncrement;
            }

            return points;
        }
    }
}