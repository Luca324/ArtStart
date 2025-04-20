using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart
{
    public class EraserTool : Tool
    {
        private Polyline currentEraser;

        public override Shape CreateShape(Color color, double thickness)
        {
            currentEraser = new Polyline
            {
                Stroke = new SolidColorBrush(Colors.WhiteSmoke),
                StrokeThickness = thickness * 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            return currentEraser;
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