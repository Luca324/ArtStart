using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart
{
    public static class ShapeExtensions
    {
        public static Shape Clone(this Shape original)
        {
            if (original is Line line)
            {
                return new Line()
                {
                    X1 = line.X1,
                    Y1 = line.Y1,
                    X2 = line.X2,
                    Y2 = line.Y2,
                    Stroke = line.Stroke,
                    StrokeThickness = line.StrokeThickness
                };
            }
            else if (original is Rectangle rect)
            {
                return new Rectangle()
                {
                    Width = rect.Width,
                    Height = rect.Height,
                    Stroke = rect.Stroke,
                    StrokeThickness = rect.StrokeThickness,
                    Fill = rect.Fill
                };
            }
            else if (original is Ellipse ellipse)
            {
                return new Ellipse()
                {
                    Width = ellipse.Width,
                    Height = ellipse.Height,
                    Stroke = ellipse.Stroke,
                    StrokeThickness = ellipse.StrokeThickness,
                    Fill = ellipse.Fill
                };
            }
            else if (original is Polygon poly)
            {
                return new Polygon()
                {
                    Points = new PointCollection(poly.Points),
                    Stroke = poly.Stroke,
                    StrokeThickness = poly.StrokeThickness,
                    Fill = poly.Fill
                };
            }
            return null;
        }
    }
}