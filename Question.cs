using System;
using System.IO;

namespace DailyMatemaks
{
    class Question
    {
        private string storage;
        private enum levels_digits
        {
            EASY = 1,
            MEDIUM = 2,
            HARD = 3
        };

        private int a;
        private int b;
        private char op;
        private string level;
        private int expectedAnswer;
        public Question(Random rnd, string level, string storage)
        {
            this.storage = storage;
            int NumberOfDigits = (int)Enum.Parse(typeof(levels_digits), level);
            this.level = level;
            a = rnd.Next(1, (int)Math.Pow(10, NumberOfDigits));
            b = rnd.Next(1, (int)Math.Pow(10, NumberOfDigits));
            string AllowedOps = "";
            if (Properties.Settings.Default.AdditionAllowed)
                AllowedOps += '+';
            if (Properties.Settings.Default.SubstractionAllowed)
                AllowedOps += '-';
            if (Properties.Settings.Default.MultiplicationAllowed)
                AllowedOps += '*';
            if (AllowedOps.Length == 0)
                throw new Exception("Należy wybrać przynajmniej jedno działanie matematyczne.");
            op = AllowedOps[rnd.Next(0, AllowedOps.Length)];
            System.Data.DataTable table = new System.Data.DataTable();
            expectedAnswer = Convert.ToInt32(table.Compute(this.getExpression(), String.Empty));
        }
        public string getExpression()
        {
            return this.a.ToString() + this.op + this.b.ToString();
        }

        public int getTimeout()
        {
            switch (level)
            {
                case "EASY":
                    return Properties.Settings.Default.EasyTime;
                case "MEDIUM":
                    return Properties.Settings.Default.MediumTime;
                case "HARD":
                    return Properties.Settings.Default.HardTime;
                default:
                    throw new Exception("Nie można określić poziomu trudności.");
            }
        }

        public bool checkAnswer(int input)
        {
            return expectedAnswer == input;
        }

        public void saveAnswer(int answer, int timeBeforeEnd = 0)
        {
            using (StreamWriter sw = File.AppendText(storage))
            {
                sw.WriteLine(getExpression() + "," + expectedAnswer.ToString() + "," + answer.ToString() + "," + (getTimeout()*1000-timeBeforeEnd).ToString());
            }
        }
    }
}
