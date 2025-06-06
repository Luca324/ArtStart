using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

public class TextElement
{
    public event Action EnterPressed;
    public TextBlock Element { get; private set; }

    private bool isLocked = false;

    public TextElement(string font, int size, Color color)
    {
        Element = new TextBlock
        {
            Text = "Новый текст",
            FontFamily = new FontFamily(font),
            FontSize = size,
            Foreground = new SolidColorBrush(color),
            IsManipulationEnabled = true
        };

        Element.MouseLeftButtonDown += (s, e) =>
        {
            if (!isLocked)
                DragDrop.DoDragDrop(Element, Element, DragDropEffects.Move);
        };

        Element.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Enter)
                EnterPressed?.Invoke();
        };
    }

    public void Lock()
    {
        isLocked = true;
        Element.IsHitTestVisible = false;
    }
}