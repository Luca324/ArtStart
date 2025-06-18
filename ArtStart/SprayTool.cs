using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtStart
{
    public class SprayTool : Tool
    {
        private Color sprayColor;
        private double sprayRadius;
        private int sprayDensity;
        private Random random = new Random();

        public void SetParameters(Color color, double thickness, int density)
        {
            sprayColor = color;
            sprayRadius = thickness;
            sprayDensity = density;
        }

        public override Shape CreateShape(Color color, double thickness)
        {
            return null; // Not used
        }

        public override void OnMouseDown(Shape shape, Point startPoint)
        {
        }

        public void OnMouseDown(Canvas canvas, Point startPoint)
        {
            Spray(canvas, startPoint);
        }

        public void OnMouseMove(Canvas canvas, Point currentPoint)
        {
            Spray(canvas, currentPoint);
        }

        public override void OnMouseMove(Shape shape, Point startPoint, Point currentPoint)
        {
        }

        private void Spray(Canvas canvas, Point center)
        {
            int densityMultiplier = 1;

            if (sprayRadius < 10)
            {
                densityMultiplier = 1; // Оставляем как есть
            }
            else if (sprayRadius >= 10 && sprayRadius < 20)
            {
                densityMultiplier = 3; // Увеличиваем плотность в 3 раза
            }
            else if (sprayRadius >= 20 && sprayRadius <= 50)
            {
                densityMultiplier = 10; // Увеличиваем в 5 раз
            }

            int totalDots = sprayDensity * densityMultiplier;

            for (int i = 0; i < totalDots; i++)
            {
                double angle = random.NextDouble() * 2 * Math.PI;
                double distance = random.NextDouble() * sprayRadius;
                double x = center.X + distance * Math.Cos(angle);
                double y = center.Y + distance * Math.Sin(angle);

                Ellipse dot = new Ellipse
                {
                    Width = 1,
                    Height = 1,
                    Fill = new SolidColorBrush(sprayColor)
                };

                Canvas.SetLeft(dot, x - 0.5);
                Canvas.SetTop(dot, y - 0.5);
                canvas.Children.Add(dot);
            }
        }
    }
}