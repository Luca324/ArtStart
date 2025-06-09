using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArtStart
{
    public partial class Challenges2 : Window
    {
        private List<Question> questions;
        private int correctAnswers = 0;

        public Challenges2()
        {
            InitializeComponent();
            LoadQuestions();
            GenerateUI();
        }

        private void LoadQuestions()
        {
            questions = new List<Question>
            {
                new Question { Text = "Что такое перспектива в искусстве?", Options = new List<string>{ "Смешение цветов", "Создание объема", "Изображение глубины на плоскости", "Контрастная композиция" }, CorrectIndex = 2 },
                new Question { Text = "Какая техника используется в акварели для создания плавных переходов?", Options = new List<string>{ "Лессировка", "Сухая кисть", "Размывка", "Импасто" }, CorrectIndex = 2 },
                new Question { Text = "Что обозначает термин 'композиция' в живописи?", Options = new List<string>{ "Цветовая гамма", "Расположение элементов", "Техника нанесения краски", "Тип холста" }, CorrectIndex = 1 },
                new Question { Text = "Что означает термин 'мазок'?", Options = new List<string>{ "Слой краски", "Форма кисти", "Отдельное движение кисти", "Цветовой контраст" }, CorrectIndex = 2 },
                new Question { Text = "Что такое 'грунт' в живописи?", Options = new List<string>{ "Основной цвет картины", "Поверхность холста", "Подготовительный слой", "Основа для карандаша" }, CorrectIndex = 2 },
                new Question { Text = "Что обозначает термин 'контур'?", Options = new List<string>{ "Заполнение цвета", "Тень", "Линия, ограничивающая форму", "Имитация света" }, CorrectIndex = 2 },
                new Question { Text = "Какой материал чаще всего используется в графике?", Options = new List<string>{ "Масло", "Акварель", "Уголь", "Темпера" }, CorrectIndex = 2 },
                new Question { Text = "Что такое 'натюрморт'?", Options = new List<string>{ "Пейзаж", "Портрет", "Изображение предметов", "Жанровая сцена" }, CorrectIndex = 2 },
            };
        }

        private void GenerateUI()
        {
            QuestionsPanel.Children.Clear();
            correctAnswers = 0;
            ResultTextBlock.Text = "";
            RetryButton.Visibility = Visibility.Collapsed;
            NextLevelButton.Visibility = Visibility.Collapsed;

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
                            ResultTextBlock.Text = $"Результат: {correctAnswers}/{questions.Count}";

                            if (correctAnswers == questions.Count)
                            {
                                NextLevelButton.Visibility = Visibility.Visible;
                            }
                            else
                            {
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
            GenerateUI(); // сброс интерфейса
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(); // замените на ваше главное окно
            main.Show();
            this.Close();
        }

        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            var level3 = new Challenges3(); // убедитесь, что окно Challenges3 создано
            level3.Show();
            this.Close();
        }
    }
}
