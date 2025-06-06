using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart
{
    public abstract class Tool
    {
        public abstract Shape CreateShape(Color color, double thickness);
        public abstract void OnMouseDown(Shape shape, Point startPoint);
        public abstract void OnMouseMove(Shape shape, Point startPoint, Point currentPoint);
    }
}