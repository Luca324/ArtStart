using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart
{
    public class PenTool : Tool
    {
        private Polyline currentPolyline;

        public override Shape CreateShape(Color color, double thickness)
        {
            currentPolyline = new Polyline
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = thickness,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            return currentPolyline;
        }

        public override void OnMouseDown(Shape shape, Point startPoint)
        {
            if (shape is Polyline polyline)
            {
                polyline.Points.Add(startPoint);
            }
        }

        public override void OnMouseMove(Shape shape, Point startPoint, Point currentPoint)
        {
            if (shape is Polyline polyline)
            {
                polyline.Points.Add(currentPoint);
            }
        }
    }
}