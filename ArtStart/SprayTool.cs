using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class SprayTool : Tool
{
    private Random rand = new Random();

    public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Point pos = e.GetPosition(canvas);
            for (int i = 0; i < Thickness * 2; i++)
            {
                double offsetX = rand.NextDouble() * Thickness - Thickness / 2;
                double offsetY = rand.NextDouble() * Thickness - Thickness / 2;
                Ellipse dot = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = new SolidColorBrush(Color),
                    Stroke = Brushes.Transparent
                };
                Canvas.SetLeft(dot, pos.X + offsetX);
                Canvas.SetTop(dot, pos.Y + offsetY);
                canvas.Children.Add(dot);
            }
        }
    }
}