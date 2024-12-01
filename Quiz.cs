using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamQuiz
{
    public class Quiz
    {
        public static void Start(string subject)
        {
            RunQuiz(Database.GetQuestions(subject));
        }

        public static void StartMixed()
        {
            var allQuestions = Database.GetAllQuestions();
            RunQuiz(allQuestions.OrderBy(_ => Guid.NewGuid()).Take(20).ToList());
        }

        private static void RunQuiz(List<Question> questions)
        {
            int score = 0;

            foreach (var question in questions)
            {
                Console.WriteLine(question.Text);
                for (int i = 0; i < question.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.Options[i]}");
                }

                Console.WriteLine("Введите ваш ответ: ");
                string[] userAnswers = Console.ReadLine()?.Split(',');

                if (question.CheckAnswer(userAnswers))
                {
                    score++;
                }
            }

            Console.WriteLine($"Ваш результат: {score} из {questions.Count}");
            //Console.WriteLine($"Сохраняем результаты для предмета: {questions.First().Subject}");
            Database.SaveResult(User.Current, questions.First().Subject, score);
        }
    }
}
