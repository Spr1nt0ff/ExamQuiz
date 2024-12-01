using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ExamQuiz
{
    public class User
    {
        public static UserData Current { get; set; }

        public static void UpdateSettings()
        {
            Console.WriteLine("Для изменения настроек введите логин:");
            string login = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            string password = Console.ReadLine();

            if (Current.Login == login && Current.Password == password) 
            { 
                Console.WriteLine("1. Изменить пароль");
                Console.WriteLine("2. Изменить дату рождения");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Введите новый пароль:");
                        string newPassword = Console.ReadLine();
                        Current.Password = newPassword;
                        Database.UpdateUser(Current);
                        Console.WriteLine("Пароль успешно изменён.");
                        break;

                    case "2":
                        Console.WriteLine("Введите новую дату рождения (дд.мм.гггг):");
                        string newDate = Console.ReadLine();
                        Current.BirthDate = newDate;
                        Database.UpdateUser(Current);
                        Console.WriteLine("Дата рождения успешно изменена.");
                        break;

                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Логин или пароль неверны. Попробуйте снова.");
            }
        }
        public static UserData AuthenticateUser(string login, string password)
        {
            try
            {
                var users = JsonSerializer.Deserialize<List<UserData>>(File.ReadAllText(Database.UsersFile));
                return users?.FirstOrDefault(u => u.Login == login && u.Password == password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при аутентификации: {ex.Message}");
                return null;
            }
        }



        public static UserData RegisterUser(string login, string password, string birthDate)
        {
            try
            {
                var users = JsonSerializer.Deserialize<List<UserData>>(File.ReadAllText(Database.UsersFile)) ?? new List<UserData>();

                if (users.Any(u => u.Login == login))
                {
                    Console.WriteLine("Пользователь с таким логином уже существует.");
                    return null;
                }

                var newUser = new UserData
                {
                    Login = login,
                    Password = password,
                    BirthDate = birthDate
                };

                users.Add(newUser);
                File.WriteAllText(Database.UsersFile, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));

                Console.WriteLine("Регистрация успешна.");
                return newUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при регистрации: {ex.Message}");
                return null;
            }
        }
    }

    public class UserData
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; }
    }
}
