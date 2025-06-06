using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public abstract class Tool
{
    public Color Color { get; set; } = Colors.Black;
    public int Thickness { get; set; } = 2;

    public virtual void OnMouseDown(Canvas canvas, MouseButtonEventArgs e) { }
    public virtual void OnMouseMove(Canvas canvas, MouseEventArgs e) { }
    public virtual void OnMouseUp(Canvas canvas, MouseButtonEventArgs e) { }
}