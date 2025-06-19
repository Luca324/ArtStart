using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArtStart
{
    public partial class Challenges : Window
    {
        private List<Question> questions;
        private int correctAnswers = 0;

        public Challenges()
        {
            InitializeComponent();
            LoadQuestions();
            GenerateUI();
            MainWindow.Click += Utils.Navigation_Click;
        }

        private void LoadQuestions()
        {
            questions = new List<Question>
            {
                new Question { Text = "Какой цвет считается основным?", Options = new List<string>{ "Зеленый", "Оранжевый", "Синий", "Фиолетовый" }, CorrectIndex = 2 },
                new Question { Text = "Как называются цвета, противоположные на цветовом круге?", Options = new List<string>{ "Аналогичные", "Холодные", "Теплые", "Комплиментарные" }, CorrectIndex = 3 },
                new Question { Text = "К каким цветам относится красный?", Options = new List<string>{ "Нейтральным", "Холодным", "Теплым", "Постельным" }, CorrectIndex = 2 },
                new Question { Text = "Как называется градация одного цвета от светлого к тёмному?", Options = new List<string>{ "Контраст", "Оттенок", "Тон", "Градиент" }, CorrectIndex = 3 },
                new Question { Text = "Что такое монохромная палитра?", Options = new List<string>{ "Цвета, стоящие рядом на цветовом круге", "Палитра из одного цвета с его оттенками", "Контрастные цвета", "Палитра из тёплых цветов" }, CorrectIndex = 1 },
                new Question { Text = "Что означает термин «насыщенность» цвета?", Options = new List<string>{ "Яркость и чистота цвета", "Теплота цвета", "Темнота цвета", "Прозрачность цвета" }, CorrectIndex = 0 },
                new Question { Text = "Какой цвет получается при смешивании всех трёх основных цветов?", Options = new List<string>{ "Белый", "Серый", "Третичный", "Зеленый" }, CorrectIndex = 2 },
                new Question { Text = "Какие из этих цветов являются дополнительными?", Options = new List<string>{ "Красный и зелёный", "Жёлтый и оранжевый", "Синий и голубой", "Фиолетовый и розовый" }, CorrectIndex = 0 },
            };
        }

        private void GenerateUI()
        {
            QuestionsPanel.Children.Clear(); // очищаем перед созданием
            correctAnswers = 0;
            ResultTextBlock.Text = "";
            NextLevelButton.Visibility = Visibility.Collapsed;
            RetryButton.Visibility = Visibility.Collapsed;

            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                question.SelectedIndex = null; // сбрасываем выбор

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

                        if (isCorrect) correctAnswers++;

                        foreach (RadioButton rb in radioGroup.Children)
                        {
                            int rbIndex = radioGroup.Children.IndexOf(rb);
                            if (rbIndex == question.CorrectIndex)
                                rb.Background = Brushes.LightGreen;
                            else if (rb.IsChecked == true)
                                rb.Background = Brushes.Red;
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
            GenerateUI();
        }

        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            new Challenges2().Show();
            this.Close();
        }
    }
}