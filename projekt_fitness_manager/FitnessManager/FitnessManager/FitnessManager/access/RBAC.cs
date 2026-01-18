using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static FitnessManager.Program;
using FitnessManager.classes;

namespace FitnessManager.classes
{
    internal class RBAC
    {
        // Lista maszyn dostępnych w systemie
        public List<string> Machines { get; private set; }

        // Przechowuje uprawnienia przypisane do ról
        private readonly Dictionary<Roles, List<string>> _Permissions;

        // Lista użytkowników w systemie
        public List<User> Users { get; private set; }

        // Delegat do wypisywania użytkowników na podstawie ich ról
        public delegate bool ViewUserByRoles(User user, RBAC rbac);

        // Delegat do usuwania użytkownika
        public delegate bool DeleteUser(User user);

        // Delegat do logowania działań w systemie
        public delegate bool LogAdder(string message);

        // Konstruktor klasy RBAC
        // Inicjalizuje listy użytkowników, maszyn oraz słownik uprawnień
        public RBAC()
        {
            Users = new List<User>();
            Machines = new List<string>();
            _Permissions = new Dictionary<Roles, List<string>>()
                {
                    {Roles.Admin, new List<string>(){"write","read","delete","manageUsers","manageMachines"}},
                    {Roles.Worker, new List<string>(){"read","manageMachines"}},
                    {Roles.PersonalTrainer, new List<string>(){"read","train"}},
                    {Roles.Member, new List<string>(){"read","buy"}}
                };
        }

        // Funkcja przypisująca trenera personalnego do członka (Member) i umawiająca sesję treningową
        public PersonalTrainer BuyAndTrain(Member user, int i)
        {
            try
            {
                PersonalTrainer trainer = null;
                // Sprawdza, czy użytkownik ma uprawnienia do zakupu trenera
                if (HasPermission(user, "buy"))
                {
                    List<User> pTrainers = new List<User>();
                    // Pobiera wszystkich trenerów personalnych
                    foreach (var u in this.Users)
                    {
                        if (u.Role == Roles.PersonalTrainer)
                        {
                            pTrainers.Add(u);
                        }
                    }

                    // Sprawdza, czy w systemie są dostępni trenerzy
                    if (pTrainers.Count > 0)
                    {
                        foreach (var p in pTrainers)
                        {
                            CreateFile(p); // Tworzy plik dla każdego trenera
                        }

                        // Sprawdza, czy użytkownik wybrał odpowiedniego trenera
                        if (i <= pTrainers.Count && i > 0)
                        {
                            // Pobiera datę sesji treningowej
                            Console.WriteLine("\nNa kiedy chciałbyś/chciałabyś się umówić? [Format rok-miesiąc-dzień godzina:minuta (yyyy-MM-dd HH:mm)]");
                            string date = Console.ReadLine();

                            // Walidacja formatu daty
                            if (DateTime.TryParse(date, out DateTime temptime) && temptime > DateTime.Now)
                            {
                                // Przypisuje trenera do użytkownika i zapisuje sesję
                                user.PrTrainers.Add(pTrainers[i - 1]);
                                trainer = (PersonalTrainer)pTrainers[i - 1];
                                trainer.Members.Add(user);
                                string time = temptime.ToString("yyyy-MM-dd HH:mm");
                                ChangeColor(ConsoleColor.Green);
                                Console.WriteLine($"Dodano sesję treningów dnia {time}");
                                ChangeColor(ConsoleColor.White);
                                // Zapisuje sesję w pliku trenera
                                File.AppendAllText($"{trainer.UserName}.txt", $"{time},{user.UserName}\n");
                                return trainer;
                            }
                            else
                            {
                                ChangeColor(ConsoleColor.Red);
                                Console.WriteLine("Źle podana data treningu");
                                ChangeColor(ConsoleColor.White);
                            }
                        }
                        return null;
                    }
                    else
                    {
                        ChangeColor(ConsoleColor.Red);
                        Console.WriteLine("Nie ma żadnych trenerów personalnych dodanych do systemu");
                        ChangeColor(ConsoleColor.White);
                    }
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Użytkownik {user.UserName} nie ma dostępu do tej funkcji");
                    ChangeColor(ConsoleColor.White);
                }
                return null;
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas przypisywania trenera: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return null;
            }
        }

        // Funkcja anulująca sesję treningową użytkownika
        public void CancelTraining(Member user)
        {
            try
            {
                // Sprawdza, czy użytkownik ma uprawnienia do anulowania treningu
                if (!HasPermission(user, "buy"))
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Użytkownik {user.UserName} nie ma dostępu do tej funkcji.");
                    ChangeColor(ConsoleColor.White);
                    return;
                }

                List<User> trainersWithSessions = new List<User>();
                int y = 1;

                // Grupuje trenerów, którzy mają przypisane sesje dla użytkownika
                var uniqueTrainers = user.PrTrainers
                    .GroupBy(t => t.UserName)
                    .Select(g => g.First())
                    .ToList();

                // Wyszukuje sesje treningowe dla każdego trenera
                foreach (var trainer in uniqueTrainers)
                {
                    string trainerFile = $"{trainer.UserName}.txt";
                    if (File.Exists(trainerFile))
                    {
                        foreach (var line in File.ReadLines(trainerFile))
                        {
                            var parts = line.Split(',');
                            // Sprawdza, czy w sesji bierze udział użytkownik
                            if (parts.Length >= 2 && parts[1] == user.UserName)
                            {
                                Console.WriteLine($"{y}. {trainer.UserName}");
                                trainersWithSessions.Add(trainer);
                                y++;
                                break;
                            }
                        }
                    }
                }

                // Jeśli nie znaleziono sesji, informuje użytkownika
                if (trainersWithSessions.Count == 0)
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine("Nie dodano żadnych sesji treningów.");
                    ChangeColor(ConsoleColor.White);
                    return;
                }

                // Wybór trenera, którego sesję użytkownik chce anulować
                Console.Write("\nU którego trenera chciałbyś/chciałabyś usunąć sesję treningu?: ");
                if (int.TryParse(Console.ReadLine(), out int input) && input >= 1 && input <= trainersWithSessions.Count)
                {
                    string trainerFile = $"{trainersWithSessions[input - 1].UserName}.txt";
                    var fileLines = File.ReadAllLines(trainerFile).ToList();

                    // Filtruje sesje związane z użytkownikiem
                    var matchingSessions = fileLines
                        .Where(line => line.Split(',').Length >= 2 && line.Split(',')[1] == user.UserName)
                        .ToList();

                    // Wyświetla dostępne sesje do anulowania
                    for (int i = 0; i < matchingSessions.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {matchingSessions[i].Split(',')[0]}");
                    }

                    // Wybór sesji do anulowania
                    Console.Write("Którą sesję chciałbyś/chciałabyś anulować?: ");
                    if (int.TryParse(Console.ReadLine(), out int input1) && input1 >= 1 && input1 <= matchingSessions.Count)
                    {
                        string sessionToRemove = matchingSessions[input1 - 1];
                        fileLines.Remove(sessionToRemove);
                        File.WriteAllLines(trainerFile, fileLines);

                        ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"Usunięto sesję treningów w dniu {sessionToRemove.Split(',')[0]}");
                        ChangeColor(ConsoleColor.White);

                        // Usuwa użytkownika z listy trenerów
                        foreach (PersonalTrainer u in user.PrTrainers)
                        {
                            if (u.UserName == trainersWithSessions[input - 1].UserName)
                            {
                                u.Members.Remove(user);
                            }
                        }
                    }
                    else
                    {
                        ChangeColor(ConsoleColor.Red);
                        Console.WriteLine("Źle podany zakres wyboru.");
                        ChangeColor(ConsoleColor.White);
                    }
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine("Źle podany zakres wyboru.");
                    ChangeColor(ConsoleColor.White);
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas usuwania treningu: {ex.Message}");
                ChangeColor(ConsoleColor.White);
            }
        }

        // Funkcja dodająca nową maszynę do systemu
        public bool AddMachine(User user, string machine, LogAdder logAdder)
        {
            try
            {
                // Sprawdza, czy użytkownik ma uprawnienia do zarządzania maszynami
                if (HasPermission(user, "manageMachines"))
                {
                    if (Machines.Contains(machine))
                    {
                        ChangeColor(ConsoleColor.Red);
                        Console.WriteLine("Istnieje już taka maszyna do treningów");
                        ChangeColor(ConsoleColor.White);
                        return false;
                    }
                    else
                    {
                        // Dodaje maszynę do listy maszyn
                        ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"Pomyślnie dodano maszynę {machine}");
                        ChangeColor(ConsoleColor.White);
                        Machines.Add(machine);

                        // Loguje zdarzenie
                        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string logMessage = $"[{timestamp}] Użytkownik {user.UserName} dodał maszynę: {machine}";
                        logAdder(logMessage);
                    }
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Użytkownik {user.UserName} nie ma dostępu do tej funkcji");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas dodawania maszyny: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }

        // Funkcja wyświetlająca listę maszyn w systemie
        public bool ViewMachines(User user)
        {
            try
            {
                // Sprawdza, czy maszyny są dostępne w systemie
                if (Machines.Count > 0)
                {
                    // Sprawdza, czy użytkownik ma odpowiednie uprawnienia
                    if (HasPermission(user, "manageMachines"))
                    {
                        int c = 1;
                        foreach (string m in Machines)
                        {
                            Console.WriteLine($"{c}. {m}");
                            c++;
                        }
                    }
                    else
                    {
                        ChangeColor(ConsoleColor.Red);
                        Console.WriteLine($"Użytkownik {user.UserName} nie ma dostępu do tej funkcji");
                        ChangeColor(ConsoleColor.White);
                        return false;
                    }
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine("Najpierw dodaj maszyny do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania maszyn: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }

        // Funkcja usuwająca maszynę z listy maszyn
        public bool RemoveMachine(User user, string machine, LogAdder logAdder)
        {
            try
            {
                // Sprawdza, czy maszyny są dostępne
                if (Machines.Count > 0)
                {
                    // Sprawdza, czy użytkownik ma uprawnienia do usuwania maszyn
                    if (HasPermission(user, "manageMachines"))
                    {
                        if (Machines.Contains(machine))
                        {
                            Machines.Remove(machine);
                            ChangeColor(ConsoleColor.Green);
                            Console.WriteLine($"Pomyslnie usunięto maszynę {machine}");
                            ChangeColor(ConsoleColor.White);

                            // Logowanie usunięcia maszyny
                            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string logMessage = $"[{timestamp}] Użytkownik {user.UserName} usunął maszynę: {machine}";
                            logAdder(logMessage);
                        }
                        else
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine($"Nie ma takiej maszyny {machine} w liście maszyn");
                            ChangeColor(ConsoleColor.White);
                            return false;
                        }
                    }
                    else
                    {
                        ChangeColor(ConsoleColor.Red);
                        Console.WriteLine($"Użytkownik {user.UserName} nie ma dostępu do tej funkcji");
                        ChangeColor(ConsoleColor.White);
                        return false;
                    }
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine("Najpierw dodaj maszyny do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas usuwania maszyny: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }

        // Funkcja dodająca użytkownika do listy Users
        public bool AddUser(User user, LogAdder logAdder)
        {
            try
            {
                if (!Users.Contains(user))
                {
                    Users.Add(user);
                    ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"Dodano użytkownika {user.UserName} do systemu");
                    ChangeColor(ConsoleColor.White);

                    // Logowanie dodania użytkownika
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string logMessage = $"[{timestamp}] Dodano użytkownika: {user.UserName} (Rola: {user.Role})";
                    logAdder(logMessage);
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Istnieje już użytkownik {user.UserName}");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas dodawania użytkownika: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }

        // Funkcja wyświetlająca wszystkich użytkowników
        public bool ViewAllUsers(User user)
        {
            try
            {
                // Sprawdza, czy użytkownik ma uprawnienia do zarządzania użytkownikami
                if (HasPermission(user, "manageUsers"))
                {
                    int c = 1;
                    foreach (var u in Users)
                    {
                        ChangeColor(ConsoleColor.Cyan);
                        Console.WriteLine($"{c}. {u.UserName}, hasło: {u.Password}, rola:  {u.Role}");
                        ChangeColor(ConsoleColor.Green);
                        c++;
                    }
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Użytkownik {user.UserName} nie ma dostępu do tej funkcji");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania użytkowników: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }

        // Funkcja usuwająca użytkownika z systemu
        public bool RemoveUser(User admin, User user, DeleteUser dUser, LogAdder logAdder)
        {
            try
            {
                // Sprawdza, czy użytkownik ma uprawnienia do usuwania innych użytkowników
                if (HasPermission(admin, "manageUsers"))
                {
                    dUser(user);
                    Users.Remove(user);
                    ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"Pomyślnie usunięto użytkownika {user.UserName} z systemu");
                    ChangeColor(ConsoleColor.White);

                    // Logowanie usunięcia użytkownika
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string logMessage = $"[{timestamp}] Użytkownik {admin.UserName} usunął użytkownika: {user.UserName}";
                    logAdder(logMessage);
                }
                else
                {
                    ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Użytkownik {admin.UserName} nie ma dostępu do tej funkcji");
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
            return true;
        }

        // Funkcja sprawdzająca, czy użytkownik ma odpowiednie uprawnienia
        public bool HasPermission(User user, string permission)
        {
            try
            {
                // Sprawdza, czy użytkownik ma przypisane dane uprawnienie do swojej roli
                foreach (var u in _Permissions)
                {
                    if (u.Key == user.Role)
                    {
                        foreach (var v in u.Value)
                        {
                            if (permission == v)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas sprawdzania uprawnień: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
        }
    }
}
