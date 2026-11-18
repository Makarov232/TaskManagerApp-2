using System;
using System.Collections.Generic;
using Serilog;

namespace TaskManagerApp
{
    internal class Program
    {
        private static readonly List<string> Tasks = new List<string>();

        static void Main(string[] args)
        {
            // Шаг 3: Настройка и инициализация Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Минимальный уровень логирования для захвата всех логов
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/taskmanager-.log",
                    rollingInterval: RollingInterval.Day, // Лог-файлы с датами (например, taskmanager-20261118.log)
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                // [INFO] Лог старта приложения
                Log.Information("Программа TaskManager успешно запущена.");

                bool isRunning = true;
                while (isRunning)
                {
                    Console.WriteLine("\n=== TaskManager ===");
                    Console.WriteLine("Доступные команды: add, remove, list, exit");
                    Console.Write("Введите команду: ");
                    string? command = Console.ReadLine()?.Trim().ToLower();

                    // [DEBUG] Трассировка ввода пользователя
                    Log.Debug("Пользователь ввел команду: {Command}", command);

                    switch (command)
                    {
                        case "add":
                            AddTask();
                            break;

                        case "remove":
                            RemoveTask();
                            break;

                        case "list":
                            ListTasks();
                            break;

                        case "exit":
                            // [INFO] Лог выхода
                            Log.Information("Завершение работы программы TaskManager по запросу пользователя.");
                            isRunning = false;
                            break;

                        default:
                            // [WARNING] Неизвестная команда
                            Log.Warning("Введена неизвестная команда: '{Command}'", command);
                            Console.WriteLine("Ошибка: неизвестная команда. Попробуйте снова.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // [ERROR] Логирование непредвиденных критических ошибок
                Log.Error(ex, "Произошла непредвиденная фатальная ошибка в работе программы.");
            }
            finally
            {
                // Корректный сброс буфера логов перед выходом
                Log.CloseAndFlush();
            }
        }

        private static void AddTask()
        {
            Log.Debug("Начало выполнения операции AddTask.");
            Console.Write("Введите название задачи: ");
            string? taskName = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(taskName))
            {
                // [WARNING] Попытка добавить пустую задачу
                Log.Warning("Пользователь попытался добавить пустую задачу.");
                Console.WriteLine("Название задачи не может быть пустым.");
                return;
            }

            Tasks.Add(taskName);
            // [INFO] Успешное добавление
            Log.Information("Задача \"{TaskName}\" успешно добавлена.", taskName);
            Console.WriteLine($"[+] Задача \"{taskName}\" добавлена.");
            Log.Debug("Конец выполнения операции AddTask.");
        }

        private static void RemoveTask()
        {
            Log.Debug("Начало выполнения операции RemoveTask.");
            Console.Write("Введите точное название задачи для удаления: ");
            string? taskName = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(taskName))
            {
                Log.Warning("Попытка удаления задачи с пустым именем.");
                return;
            }

            if (Tasks.Remove(taskName))
            {
                // [INFO] Успешное удаление
                Log.Information("Задача \"{TaskName}\" успешно удалена.", taskName);
                Console.WriteLine($"[-] Задача \"{taskName}\" удалена.");
            }
            else
            {
                // [ERROR] Задача не найдена
                Log.Error("Задача \"{TaskName}\" не найдена в списке для удаления.", taskName);
                Console.WriteLine($"[!] Ошибка: задача \"{taskName}\" не найдена.");
            }

            Log.Debug("Конец выполнения операции RemoveTask.");
        }

        private static void ListTasks()
        {
            Log.Debug("Начало выполнения операции ListTasks.");

            if (Tasks.Count == 0)
            {
                // [WARNING] Список пуст
                Log.Warning("Запрошен список задач, но список пуст.");
                Console.WriteLine("Список задач пуст.");
            }
            else
            {
                // [INFO] Вывод списка
                Log.Information("Выведен список задач. Всего задач: {Count}.", Tasks.Count);
                Console.WriteLine("\nСписок задач:");
                for (int i = 0; i < Tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Tasks[i]}");
                }
            }

            Log.Debug("Конец выполнения операции ListTasks.");
        }
    }
}