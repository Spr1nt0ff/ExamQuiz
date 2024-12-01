using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ExamQuiz
{
    public static class Database
    {
        public static readonly string ResultsFile = "results.json";
        public static readonly string UsersFile = "users.json";
        public static readonly string QuestionsFile = "questions.json";

        public static List<Question> GetQuestions(string subject)
        {
            try
            {
                var allSubjects = JsonSerializer.Deserialize<List<SubjectData>>(File.ReadAllText(QuestionsFile));
                var subjectData = allSubjects?.FirstOrDefault(s => s.Subject == subject);
                if (subjectData == null)
                {
                    Console.WriteLine($"Не найдена информация по предмету: {subject}");
                    return new List<Question>();
                }

                foreach (var question in subjectData.Questions)
                {
                    question.Subject = subject;
                }

                return subjectData.Questions ?? new List<Question>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке вопросов: {ex.Message}");
                return new List<Question>();
            }
        }

        public static List<Question> GetAllQuestions()
        {
            try
            {
                var allSubjects = JsonSerializer.Deserialize<List<SubjectData>>(File.ReadAllText(QuestionsFile));
                var allQuestions = new List<Question>();
                foreach (var subjectData in allSubjects)
                {
                    allQuestions.AddRange(subjectData.Questions);
                }
                return allQuestions;
            }
            catch
            {
                return new List<Question>();
            }
        }

        // Сохранение результатов
        public static void SaveResult(UserData user, string subject, int score)
        {
            try
            {
                var results = JsonSerializer.Deserialize<List<Result>>(File.ReadAllText(ResultsFile)) ?? new List<Result>();


                results.Add(new Result
                {
                    User = user.Login,
                    Subject = subject,
                    Score = score
                });

                File.WriteAllText(ResultsFile, JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении результата: {ex.Message}");
            }
        }

        public static List<Result> GetUserResults(string login)
        {
            try
            {
                var results = JsonSerializer.Deserialize<List<Result>>(File.ReadAllText(ResultsFile)) ?? new List<Result>();

                return results.Where(r => r.User == login).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении результатов: {ex.Message}");
                return new List<Result>();
            }
        }

        public static List<Result> GetTop20(string subject)
        {
            try
            {
                var results = JsonSerializer.Deserialize<List<Result>>(File.ReadAllText(ResultsFile)) ?? new List<Result>();

                var top20 = results.OrderByDescending(r => r.Score).ThenBy(r => r.User).Take(20).ToList();

                return top20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении результатов: {ex.Message}");
                return new List<Result>();
            }
        }

        public static void UpdateUser(UserData userData)
        {
            try
            {
                var users = JsonSerializer.Deserialize<List<UserData>>(File.ReadAllText(UsersFile)) ?? new List<UserData>();
                var user = users.FirstOrDefault(u => u.Login == userData.Login);
                if (user != null)
                {
                    user.Password = userData.Password;
                    user.BirthDate = userData.BirthDate;
                    File.WriteAllText(UsersFile, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении данных пользователя: {ex.Message}");
            }
        }
    }

    public class SubjectData
    {
        public string Subject { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public List<int> CorrectAnswers { get; set; }
        public string Subject { get; set; }

        public bool CheckAnswer(string[] userAnswers)
        {
            var correct = CorrectAnswers.Select(c => c.ToString()).ToList();
            return userAnswers.All(answer => correct.Contains(answer));
        }
    }

    public class Result
    {
        public string User { get; set; }
        public string Subject { get; set; }
        public int Score { get; set; }
    }
}
