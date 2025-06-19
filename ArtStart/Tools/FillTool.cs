using ArtStart.Tools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArtStart.Tools
{
    public class FillTool : Tool
    {
        public override Shape CreateShape(Color color, double thickness) => null;

        public override void OnMouseDown(Shape shape, Point startPoint) { }

        public override void OnMouseMove(Shape shape, Point startPoint, Point currentPoint) { }

        public void FloodFill(Canvas canvas, Point startPoint, Color targetColor, Color replacementColor)
        {
            if (targetColor == replacementColor) return;

            var bitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(canvas);
            var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
            bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0);

            int x = (int)startPoint.X;
            int y = (int)startPoint.Y;

            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                return;

            int index = (y * bitmap.PixelWidth + x) * 4;
            Color pixelColor = Color.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]);

            if (pixelColor != targetColor) return;

            var queue = new Queue<Point>();
            queue.Enqueue(startPoint);

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();
                x = (int)point.X;
                y = (int)point.Y;

                if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                    continue;

                index = (y * bitmap.PixelWidth + x) * 4;
                pixelColor = Color.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]);

                if (pixelColor != targetColor)
                    continue;

                var rect = new Rectangle
                {
                    Width = 1,
                    Height = 1,
                    Fill = new SolidColorBrush(replacementColor)
                };

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                canvas.Children.Add(rect);

                pixels[index] = replacementColor.B;
                pixels[index + 1] = replacementColor.G;
                pixels[index + 2] = replacementColor.R;
                pixels[index + 3] = replacementColor.A;

                queue.Enqueue(new Point(x + 1, y));
                queue.Enqueue(new Point(x - 1, y));
                queue.Enqueue(new Point(x, y + 1));
                queue.Enqueue(new Point(x, y - 1));
            }
        }
    }
}