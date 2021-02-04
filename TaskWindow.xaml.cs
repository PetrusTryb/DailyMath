using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DailyMatemaks
{
    /// <summary>
    /// Logika interakcji dla klasy TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        private List<Question> questions = new List<Question>();
        private int currentQuestionIndex = 0;
        private DispatcherTimer currentTimer = new DispatcherTimer(DispatcherPriority.Send);
        private int timeoutIn = 0;
        private string quizStorage = "history/"+((int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString()+".csv";
        public TaskWindow()
        {
            InitializeComponent();
            Random rnd = new Random();
            try
            {
                for (int i = 0; i < Properties.Settings.Default.EasyQuestions; i++)
                {
                    this.questions.Add(new Question(rnd, "EASY", quizStorage));
                }
                for (int i = 0; i < Properties.Settings.Default.MediumQuestions; i++)
                {
                    this.questions.Add(new Question(rnd, "MEDIUM", quizStorage));
                }
                for (int i = 0; i < Properties.Settings.Default.HardQuestions; i++)
                {
                    this.questions.Add(new Question(rnd, "HARD", quizStorage));
                }
                currentTimer.Tick += ProgressBarTimer_Elapsed;
                currentTimer.Start();
                NextQuestion();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Nie udało się wygenerować zestawu pytań do quizu.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProgressBarTimer_Elapsed(object sender, EventArgs e)
        {
            if (timeoutIn % 1000 == 0)
                timeIndicator.Value = timeoutIn;
            if (timeoutIn <= 0)
            {
                questions[currentQuestionIndex - 1].saveAnswer(int.MinValue);
                NextQuestion();
            }
            timeoutIn -= 100;
        }

        private void NextQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                Close();
                return;
            }
            string expression = questions[currentQuestionIndex].getExpression();
            QuestionLabel.Content = "Ile to " + expression + "?";
            Answer.Clear();
            int time = questions[currentQuestionIndex].getTimeout();
            timeoutIn = time * 1000;
            currentTimer.Interval = TimeSpan.FromMilliseconds(100);
            timeIndicator.Maximum = timeoutIn;
            timeIndicator.Value = timeoutIn;
            currentTimer.Start();
            currentQuestionIndex++;
        }

        private void Answer_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (questions[currentQuestionIndex - 1].checkAnswer(Convert.ToInt32(Answer.Text)))
                {
                    questions[currentQuestionIndex - 1].saveAnswer(Convert.ToInt32(Answer.Text), timeoutIn);
                    currentTimer.Stop();
                    NextQuestion();
                }
            }
            catch (FormatException) { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            currentTimer.Stop();
        }
    }
}
