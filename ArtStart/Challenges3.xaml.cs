using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArtStart
{
    public partial class Challenges3 : Window
    {
        private List<Question> questions;
        private int correctAnswers = 0;

        public Challenges3()
        {
            InitializeComponent();
            LoadQuestions();
            GenerateUI();
            MainWindow.Click += Utils.Navigation_Click;
            Info.Click += Utils.Navigation_Click;
            ColorMix.Click += Utils.Navigation_Click;
            LogOut.Click += Utils.LogOut;
            Paint.Click += Utils.Navigation_Click;
        }

        private void LoadQuestions()
        {
            questions = new List<Question>
            {
                new Question { Text = "Что означает термин 'фактура' в живописи?", Options = new List<string>{ "Цветовое решение", "Плотность краски", "Поверхностные свойства материала", "Техника рисования" }, CorrectIndex = 2 },
                new Question { Text = "Какой из этих стилей относится к модернизму?", Options = new List<string>{ "Барокко", "Импрессионизм", "Рококо", "Готика" }, CorrectIndex = 1 },
                new Question { Text = "Что такое 'импасто'?", Options = new List<string>{ "Тонкий слой краски", "Пастельный цвет", "Толстый рельефный мазок", "Промежуточный цвет" }, CorrectIndex = 2 },
                new Question { Text = "Какой материал чаще всего используется в скульптуре?", Options = new List<string>{ "Холст", "Мрамор", "Бумага", "Акварель" }, CorrectIndex = 1 },
                new Question { Text = "Что такое 'палитра'?", Options = new List<string>{ "Смешение линий", "Набор кистей", "Доска для смешивания красок", "Цветовое пятно" }, CorrectIndex = 2 },
                new Question { Text = "Что такое 'светотень'?", Options = new List<string>{ "Тень от объекта", "Сочетание света и тени для создания объема", "Светлая часть рисунка", "Контурная линия" }, CorrectIndex = 1 },
                new Question { Text = "Какой стиль характеризуется строгой симметрией и идеальными пропорциями?", Options = new List<string>{ "Ренессанс", "Кубизм", "Сюрреализм", "Экспрессионизм" }, CorrectIndex = 0 },
                new Question { Text = "Кто является представителем кубизма?", Options = new List<string>{ "Моне", "Пикассо", "Ван Гог", "Рембрандт" }, CorrectIndex = 1 },
            };
        }

        private void GenerateUI()
        {
            QuestionsPanel.Children.Clear();
            correctAnswers = 0;
            ResultTextBlock.Text = "";
            RetryButton.Visibility = Visibility.Collapsed;

            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                question.SelectedIndex = null;

                var groupBox = new GroupBox
                {
                    Header = $"Вопрос {i + 1}: {question.Text}",
                    Margin = new Thickness(5)
                };

                var stack = new StackPanel();
                var radioGroup = new StackPanel();
                var groupName = $"Question{i}";

                for (int j = 0; j < question.Options.Count; j++)
                {
                    int optionIndex = j;

                    var radio = new RadioButton
                    {
                        Content = question.Options[j],
                        GroupName = groupName,
                        Margin = new Thickness(5)
                    };

                    radio.Checked += (s, e) =>
                    {
                        if (question.SelectedIndex != null)
                            return;

                        question.SelectedIndex = optionIndex;
                        bool isCorrect = optionIndex == question.CorrectIndex;

                        if (isCorrect)
                            correctAnswers++;

                        foreach (RadioButton rb in radioGroup.Children)
                        {
                            int rbIndex = radioGroup.Children.IndexOf(rb);
                            if (rbIndex == question.CorrectIndex)
                                rb.Background = Brushes.LightGreen;
                            else if (rb.IsChecked == true)
                                rb.Background = Brushes.IndianRed;
                        }

                        if (questions.All(q => q.SelectedIndex != null))
                        {
                            if (correctAnswers == questions.Count)
                            {
                                ResultTextBlock.Text = "Тесты успешно пройдены!";
                            }
                            else
                            {
                                ResultTextBlock.Text = $"Результат: {correctAnswers}/{questions.Count}";
                                RetryButton.Visibility = Visibility.Visible;
                            }
                        }
                    };

                    radioGroup.Children.Add(radio);
                }

                stack.Children.Add(radioGroup);
                groupBox.Content = stack;
                QuestionsPanel.Children.Add(groupBox);
            }
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateUI(); // сбросить тест
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(); // Замените на ваше главное окно
            main.Show();
            this.Close();
        }
    }
}
