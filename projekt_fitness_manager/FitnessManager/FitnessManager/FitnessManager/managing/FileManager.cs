using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static FitnessManager.Program;

namespace FitnessManager.classes
{
    internal class FileManager : ILogger
    {
        // Zdarzenia wywoływane podczas różnych akcji użytkownika
        public event Action<string> OnAccountSignedUp;
        public event Action<string> OnChangedPassword;
        public event Action<string> OnAccountLoggedIn;

        // Ścieżki do plików przechowujących dane użytkowników, maszyn i logów
        private readonly string _Users = "users.txt";
        private readonly string _Machines = "machines.txt";
        private readonly string _Logs = "logs.txt";

        // Konstruktor inicjalizujący FileManager – ładuje dane z plików, jeśli istnieją
        public FileManager(RBAC rbac)
        {
            // Tworzy pliki, jeśli nie istnieją
            if (!File.Exists(_Users))
            {
                File.WriteAllText(_Users, "");
            }
            else if (!File.Exists(_Logs))
            {
                File.WriteAllText(_Logs, "");
            }
            else if (!File.Exists(_Machines))
            {
                File.WriteAllText(_Machines, "");
            }

            // Wczytuje użytkowników z pliku 'users.txt' do listy RBAC
            if (File.Exists(_Users))
            {
                rbac.Users.Clear();
                if (File.ReadAllLines(_Users).Length > 0)
                {
                    foreach (string line in File.ReadAllLines(_Users))
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length < 3) continue;

                        User user = null;
                        switch (parts[2])
                        {
                            case "Admin":
                                user = new Admin(parts[0], parts[1]);
                                break;
                            case "Worker":
                                user = new Worker(parts[0], parts[1]);
                                break;
                            case "PersonalTrainer":
                                user = new PersonalTrainer(parts[0], parts[1], rbac);
                                break;
                            case "Member":
                                user = new Member(parts[0], parts[1], rbac);
                                break;
                        }

                        if (user != null)
                            rbac.Users.Add(user);
                    }
                }
            }

            // Wczytuje maszyny z pliku 'machines.txt'
            if (File.Exists(_Machines))
            {
                rbac.Machines.Clear();
                if (File.ReadAllLines(_Machines).Length > 0)
                {
                    foreach (string line in File.ReadAllLines(_Machines))
                    {
                        rbac.Machines.Add(line);
                    }
                }
            }
        }

        // Rejestracja nowego użytkownika i zapisanie jego danych do pliku
        public bool SignUp(User user)
        {
            try
            {
                foreach (string line in File.ReadLines(_Users))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 3) continue;

                    // Sprawdza, czy użytkownik już istnieje
                    if (parts[0] == user.UserName)
                    {
                        ChangeColor(ConsoleColor.Red);
                        Console.WriteLine($"Istnieje już użytkownik {user.UserName}");
                        ChangeColor(ConsoleColor.White);
                        return false;
                    }
                }

                // Dodaje użytkownika do pliku
                File.AppendAllText(_Users, $"{user.UserName},{HashPassword(user.Password)},{user.Role}\n");

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logMessage = $"[{timestamp}] Zarejestrowano nowego użytkownika: {user.UserName} (Rola: {user.Role})";
                ChangeColor(ConsoleColor.Green);
                OnAccountSignedUp?.Invoke($"Pomyślnie zarejestrowano użytkownika {user.UserName}");
                ChangeColor(ConsoleColor.White);
                AddLog(logMessage);
                return true;
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas rejestracji: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
        }

        // Logowanie użytkownika, weryfikacja hasła i loginu
        public bool LogIn(User user)
        {
            try
            {
                FileInfo info = new FileInfo(_Users);
                if (info.Length > 0)
                {
                    foreach (string line in File.ReadLines(_Users))
                    {
                        var parts = line.Split(',');
                        if (parts.Length < 3) continue;

                        if (parts[0] == user.UserName && user.Password == parts[1])
                        {
                            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string logMessage = $"[{timestamp}] Użytkownik {user.UserName} zalogował się";
                            AddLog(logMessage);
                            ChangeColor(ConsoleColor.Green);
                            OnAccountLoggedIn?.Invoke($"Pomyślnie zalogowano użytkownika {user.UserName}");
                            ChangeColor(ConsoleColor.White);
                            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                            return true;
                        }
                    }
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Nazwa użytkownika bądź hasło jest niepoprawne. Spróbuj jeszcze raz");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine("Nie ma żadnych użytkowników dodanych do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas logowania: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
        }

        // Dodaje maszynę do pliku 'machines.txt'
        public void AddMachine(string machineName)
        {
            if (File.Exists(_Machines))
            {
                File.AppendAllText(_Machines, $"{machineName}\n");
            }
        }

        // Usuwa maszynę z pliku
        public void RemoveMachine(string machineName)
        {
            if (File.Exists(_Machines))
            {
                var machines = File.ReadLines(_Machines).ToList();
                if (machines.Count > 0 && machines.Contains(machineName))
                {
                    machines.Remove(machineName);
                    File.WriteAllLines(_Machines, machines);
                }
            }
        }

        // Usuwa użytkownika z systemu
        public bool DeleteUser(User user)
        {
            try
            {
                FileInfo info = new FileInfo(_Users);
                if (info.Length > 0)
                {
                    var lines = File.ReadAllLines(_Users).ToList();
                    foreach (string line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts.Length < 3) continue;
                        if (parts[0] == user.UserName)
                        {
                            lines.Remove(line);
                            File.WriteAllLines("users.txt", lines);

                            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string logMessage = $"[{timestamp}] Usunięto użytkownika: {user.UserName}";
                            AddLog(logMessage);
                            return true;
                        }
                    }
                    File.WriteAllLines(_Users, lines);
                    return false;
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine("Nie ma żadnych użytkowników dodanych do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas usuwania użytkownika: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
        }

        // Dodaje nowy wpis do logów
        public bool AddLog(string message)
        {
            try
            {
                File.AppendAllText(_Logs, $"{message}\n");
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas zapisywania loga: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }

        // Zmienia hasło użytkownika i aktualizuje plik
        public bool ChangePassword(User user, string newPassword)
        {
            try
            {
                var lines = File.ReadAllLines(_Users).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length < 3) continue;

                    if (parts[0] == user.UserName)
                    {
                        if (HashPassword(newPassword) == parts[1])
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine("Hasło nie może być takie samo jak poprzednie.");
                            ChangeColor(ConsoleColor.White);
                            return false;
                        }

                        parts[1] = HashPassword(newPassword);
                        lines[i] = string.Join(",", parts);
                        File.WriteAllLines(_Users, lines);
                        ChangeColor(ConsoleColor.Green);
                        OnChangedPassword?.Invoke($"Hasło dla {user.UserName} zostało zmienione.");
                        ChangeColor(ConsoleColor.White);
                        return true;
                    }
                }

                ChangeColor(ConsoleColor.Red);
                Console.WriteLine("Niepoprawne dane użytkownika. Spróbuj jeszcze raz.");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas zmiany hasła: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
        }
    }
}
