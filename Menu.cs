using System;

namespace ExamQuiz
{
    static class Menu
    {
        public static void ShowMainMenu()
        {
            Console.WriteLine("Добро пожаловать в Викторину!");
            if (User.Current == null)
            {
                Console.WriteLine("1. Войти");
                Console.WriteLine("2. Зарегистрироваться");
            }
            else
            {
                Console.WriteLine("1. Начать викторину");
                Console.WriteLine("2. Посмотреть результаты");
                Console.WriteLine("3. Топ-20 игроков");
                Console.WriteLine("4. Изменить настройки");
                Console.WriteLine("5. Выйти");
            }
            string choice = Console.ReadLine();

            if (User.Current == null)
            {
                switch (choice)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Register();
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "1":
                        StartQuiz();
                        break;
                    case "2":
                        ShowResults();
                        break;
                    case "3":
                        ShowTop20();
                        break;
                    case "4":
                        User.UpdateSettings();
                        break;
                    case "5":
                        Console.WriteLine("До свидания!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
        }

        private static void Login()
        {
            Console.WriteLine("Введите логин:");
            string login = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            string password = Console.ReadLine();

            UserData user = User.AuthenticateUser(login, password);
            if (user != null)
            {
                User.Current = user;
                Console.WriteLine("Успешный вход!");
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль.");
            }
        }

        private static void Register()
        {
            Console.WriteLine("Введите логин:");
            string login = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            string password = Console.ReadLine();
            Console.WriteLine("Введите дату рождения (дд.мм.гггг):");
            string birthDate = Console.ReadLine();

            UserData newUser = User.RegisterUser(login, password, birthDate);
            if (newUser != null)
            {
                User.Current = newUser;
                Console.WriteLine("Регистрация успешна, теперь можете войти!");
            }
        }

        private static void StartQuiz()
        {
            Console.WriteLine("Выберите тему:");
            string[] subjects = { "Биология", "Математика", "История", "Украинская литература", "Химия", "Смешанная викторина" };
            for (int i = 0; i < subjects.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {subjects[i]}");
            }

            string subject = Console.ReadLine();
            if (int.TryParse(subject, out int index) && index >= 1 && index <= 6)
            {
                if (index == 6)
                {
                    Quiz.StartMixed();
                }
                else
                {
                    Quiz.Start(subjects[index - 1]);
                }
            }
            else
            {
                Console.WriteLine("Некорректный выбор.");
            }
        }

        private static void ShowResults()
        {
            var results = Database.GetUserResults(User.Current.Login);
            foreach (var result in results)
            {
                Console.WriteLine($"Тема: {result.Subject}, Баллы: {result.Score}");
            }
        }

        private static void ShowTop20()
        {
            Console.WriteLine("Введите тему викторины:");
            string subject = Console.ReadLine();
            var topPlayers = Database.GetTop20(subject);
            foreach (var player in topPlayers)
            {
                Console.WriteLine($"Игрок: {player.User}, Баллы: {player.Score}");
            }
        }
    }
}
