using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ArtStart;

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
        }

        private void LoadQuestions()
        {
            // Пример 10 вопросов
            questions = new List<Question>
            {
                new Question { Text = "Столица Франции?", Options = new List<string>{ "Париж", "Лондон", "Берлин" }, CorrectIndex = 0 },
                new Question { Text = "2 + 2?", Options = new List<string>{ "3", "4", "5" }, CorrectIndex = 1 },
                // Добавь ещё 8 вопросов
                // ...
            };
        }

        private void GenerateUI()
        {
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];

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
                    int questionIndex = i;
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
                            return; // Уже выбран, не менять

                        question.SelectedIndex = optionIndex;
                        bool isCorrect = optionIndex == question.CorrectIndex;

                        if (isCorrect) correctAnswers++;

                        // Подсветка
                        foreach (RadioButton rb in radioGroup.Children)
                        {
                            int rbIndex = radioGroup.Children.IndexOf(rb);
                            if (rbIndex == question.CorrectIndex)
                                rb.Background = Brushes.LightGreen;
                            else if (rb.IsChecked == true)
                                rb.Background = Brushes.IndianRed;
                        }

                        // Показываем результат если последний вопрос
                        if (questions.All(q => q.SelectedIndex != null))
                        {
                            ResultTextBlock.Text = $"Результат: {correctAnswers}/{questions.Count}";
                        }
                    };

                    radioGroup.Children.Add(radio);
                }

                stack.Children.Add(radioGroup);
                groupBox.Content = stack;
                QuestionsPanel.Children.Add(groupBox);
            }
        }
    }
}