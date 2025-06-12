using System;
using System.Windows;
using System.Windows.Controls;

namespace ArtStart
{
    internal class Utils
    {
        public static void Navigation_Click(object sender, RoutedEventArgs e)
        {
            string targetButtonName = ((FrameworkElement)sender).Name;

            Console.WriteLine($"targetButtonName: {targetButtonName}");

            var alreadyOpened = false;
            foreach (Window window in App.Current.Windows)
            {
                var title = window.Title;
                Console.WriteLine($"title: {title}");
                 
                if (title == targetButtonName)
                {
                    window.Activate();
                    alreadyOpened = true;
                    break;
                }
            }
            if (alreadyOpened == false)
            {
                Window window = null;
                switch (targetButtonName)
                {
                    case "Challenges":
                        window = new Challenges();
                        break;
                    case "Paint":
                        window = new Paint();
                        break;
                    case "ColorMix":
                        window = new ColorMix();
                        break;
                   
                }
                window.Show();

            }
        }
    }

}

