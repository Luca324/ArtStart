using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class EraserTool : Tool
{
    public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
    {
        UIElement element = canvas.InputHitTest(e.GetPosition(canvas)) as UIElement;
        if (element != null && !(element is TextBlock))
        {
            canvas.Children.Remove(element);
        }
    }

    public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            UIElement element = canvas.InputHitTest(e.GetPosition(canvas)) as UIElement;
            if (element != null && !(element is TextBlock))
            {
                canvas.Children.Remove(element);
            }
        }
    }
}