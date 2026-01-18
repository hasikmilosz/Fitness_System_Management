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
using static FitnessManager.classes.Menu;
using static FitnessManager.classes.GetUsers;
namespace FitnessManager
{
    internal class Program
    {
        //delegat, który będzie zajmował się kolorowaniem tekstu w konsoli
        public delegate void SetForegroundColor(ConsoleColor color);

        //enumerator z rolami użytkownika
        public enum Roles
        {
            Admin,
            Worker,
            PersonalTrainer,
            Member
        }
        //Interfejs do FileManager
        public interface ILogger
        {
            bool SignUp(User user);
            bool LogIn(User user);
            bool AddLog(string message);
            bool ChangePassword(User user, string password);
        }
        //funkcja hash'ująca hasło i zwaracająca go
        public static string HashPassword(string password)
        {
            try
            {
                byte[] output;
                using (SHA256 sha256 = SHA256.Create())
                {
                    output = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
                return Convert.ToBase64String(output);
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas hashowania hasła: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return string.Empty;
            }
        }
        //metoda zmieniająca foregorundColor
        public static void ChangeColor(ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas zmiany koloru: {ex.Message}");
                ChangeColor(ConsoleColor.White);
            }
        }
        public static User CreateUserByRole(string username, string password, Roles role, RBAC rbac)
        {
            try
            {
                switch (role)
                {
                    case Roles.Admin: return new Admin(username, password);
                    case Roles.Worker: return new Worker(username, password);
                    case Roles.PersonalTrainer: return new PersonalTrainer(username, password, rbac);
                    case Roles.Member: return new Member(username, password, rbac);
                    default: return null;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas tworzenia użytkownika: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return null;
            }
        }
        public static void CreateFile(User user)
        {
            if (!File.Exists($"{user.UserName}.txt"))
            {
                File.WriteAllText($"{user.UserName}.txt", "");
            }
        }

        static void Main(string[] args)
        {
            //tworzenie delegatów
            RBAC.ViewUserByRoles viewUserByRoles = null;
            SetForegroundColor setForegroundColor = ChangeColor;
            RBAC.LogAdder logAdder = null;

            try
            {
                RBAC rbac = new RBAC();
                FileManager fileManager = new FileManager(rbac);
                logAdder = fileManager.AddLog;
                User currentUser = null;

                // Podpięcie eventów
                fileManager.OnAccountSignedUp += message =>
                {
                    Console.WriteLine(message);
                    fileManager.AddLog(message);
                };

                fileManager.OnAccountLoggedIn += message =>
                {
                    Console.WriteLine(message);
                    fileManager.AddLog(message);
                };

                fileManager.OnChangedPassword += message =>
                {
                    Console.WriteLine(message);
                    fileManager.AddLog(message);
                };

                bool exit = false;
                while (!exit)
                {
                    PrintMainMenu();
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": // Logowanie
                            try
                            {
                                Console.Write("Podaj nazwę użytkownika: ");
                                string loginUsername = Console.ReadLine();
                                Console.Write("Podaj hasło: ");
                                string loginPassword = Console.ReadLine();
                                loginPassword = HashPassword(loginPassword);
                                User tempUser = rbac.Users.FirstOrDefault(u => u.UserName == loginUsername);
                                if (tempUser == null)
                                {
                                    tempUser = new Member(loginUsername, loginPassword, rbac);
                                }
                                else
                                {
                                    tempUser.Password = loginPassword;
                                }

                                if (fileManager.LogIn(tempUser))
                                {
                                    currentUser = rbac.Users.FirstOrDefault(u => u.UserName == loginUsername) ?? tempUser;
                                    bool loggedIn = true;

                                    while (loggedIn)
                                    {
                                        switch (currentUser.Role)
                                        {
                                            case Roles.Admin:
                                                PrintAdminMenu(currentUser);
                                                var adminChoice = Console.ReadLine();
                                                switch (adminChoice)
                                                {
                                                    case "1":
                                                        rbac.ViewAllUsers(currentUser);
                                                        break;
                                                    case "2":
                                                        viewUserByRoles = GetAdmins;
                                                        viewUserByRoles(currentUser, rbac);
                                                        break;
                                                    case "3":
                                                        viewUserByRoles = GetMembers;
                                                        viewUserByRoles(currentUser, rbac);
                                                        break;
                                                    case "4":
                                                        viewUserByRoles = GetPrTrainers;
                                                        viewUserByRoles(currentUser, rbac);
                                                        break;
                                                    case "5":
                                                        viewUserByRoles = GetWorkers;
                                                        viewUserByRoles(currentUser, rbac);
                                                        break;
                                                    case "6":
                                                        Console.Write("Podaj nazwę użytkownika: ");
                                                        string username = Console.ReadLine();
                                                        Console.Write("Podaj hasło: ");
                                                        string password = Console.ReadLine();
                                                        Console.WriteLine("Wybierz rolę:");
                                                        Console.WriteLine("1. Admin");
                                                        Console.WriteLine("2. Worker");
                                                        Console.WriteLine("3. PersonalTrainer");
                                                        Console.WriteLine("4. Member");
                                                        string newRoleChoice = Console.ReadLine();

                                                        Roles role = Roles.Member;
                                                        switch (newRoleChoice)
                                                        {
                                                            case "1": role = Roles.Admin; break;
                                                            case "2": role = Roles.Worker; break;
                                                            case "3": role = Roles.PersonalTrainer; break;
                                                            case "4": role = Roles.Member; break;
                                                            default:
                                                                setForegroundColor(ConsoleColor.Red);
                                                                Console.WriteLine("Nieprawidłowy wybór roli. Ustawiono Member.");
                                                                role = Roles.Member;
                                                                setForegroundColor(ConsoleColor.White);
                                                                break;
                                                        }

                                                        User userToAdd = CreateUserByRole(username, password, role, rbac);
                                                        if (userToAdd != null)
                                                        {
                                                            if (rbac.HasPermission(currentUser, "manageUsers"))
                                                            {
                                                                if (fileManager.SignUp(userToAdd))
                                                                {
                                                                    rbac.AddUser(userToAdd, logAdder);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    case "7":
                                                        rbac.ViewAllUsers(currentUser);
                                                        Console.Write("Podaj numer użytkownika do usunięcia: ");
                                                        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= rbac.Users.Count)
                                                        {
                                                            User userToRemove = rbac.Users[index - 1];
                                                            //stworzenie delegata
                                                            RBAC.DeleteUser deleteUser = fileManager.DeleteUser;
                                                            rbac.RemoveUser(currentUser, userToRemove, deleteUser, logAdder);
                                                        }
                                                        else
                                                        {
                                                            setForegroundColor(ConsoleColor.Red);
                                                            Console.WriteLine("Nieprawidłowy numer użytkownika");
                                                            setForegroundColor(ConsoleColor.White);
                                                        }
                                                        break;
                                                    case "8":
                                                        Console.Write("Podaj nazwę maszyny: ");
                                                        string machine = Console.ReadLine();
                                                        rbac.AddMachine(currentUser, machine, logAdder);
                                                        fileManager.AddMachine(machine);
                                                        break;
                                                    case "9":
                                                        rbac.ViewMachines(currentUser);
                                                        break;
                                                    case "10":
                                                        rbac.ViewMachines(currentUser);
                                                        Console.Write("Podaj numer maszyny do usunięcia: ");
                                                        if (int.TryParse(Console.ReadLine(), out int machineIndex) && machineIndex > 0 && machineIndex <= rbac.Machines.Count)
                                                        {
                                                            string machineToRemove = rbac.Machines[machineIndex - 1];
                                                            rbac.RemoveMachine(currentUser, machineToRemove, logAdder);
                                                            fileManager.RemoveMachine(machineToRemove);
                                                        }
                                                        else
                                                        {
                                                            setForegroundColor(ConsoleColor.Red);
                                                            Console.WriteLine("Nieprawidłowy numer maszyny");
                                                            setForegroundColor(ConsoleColor.White);
                                                        }
                                                        break;
                                                    case "11":
                                                        Console.WriteLine($"Podaj nowe hasło dla użytkownika {currentUser.UserName}");
                                                        string newPass = Console.ReadLine();
                                                        fileManager.ChangePassword(currentUser, newPass);
                                                        break;
                                                    case "12":
                                                        loggedIn = false;
                                                        currentUser = null;
                                                        break;
                                                    default:
                                                        setForegroundColor(ConsoleColor.Red);
                                                        Console.WriteLine("Nieprawidłowy wybór");
                                                        setForegroundColor(ConsoleColor.White);
                                                        break;
                                                }
                                                break;

                                            case Roles.Worker:
                                                PrintWorkerMenu(currentUser);
                                                var workerChoice = Console.ReadLine();
                                                switch (workerChoice)
                                                {
                                                    case "1":
                                                        Console.Write("Podaj nazwę maszyny: ");
                                                        string machine = Console.ReadLine();
                                                        rbac.AddMachine(currentUser, machine, logAdder);
                                                        fileManager.AddMachine(machine);
                                                        break;
                                                    case "2":
                                                        rbac.ViewMachines(currentUser);
                                                        break;
                                                    case "3":
                                                        rbac.ViewMachines(currentUser);
                                                        Console.Write("Podaj numer maszyny do usunięcia: ");
                                                        if (int.TryParse(Console.ReadLine(), out int machineIndex) && machineIndex > 0 && machineIndex <= rbac.Machines.Count)
                                                        {
                                                            string machineToRemove = rbac.Machines[machineIndex - 1];
                                                            rbac.RemoveMachine(currentUser, machineToRemove, logAdder);
                                                            fileManager.RemoveMachine(machineToRemove);
                                                        }
                                                        else
                                                        {
                                                            setForegroundColor(ConsoleColor.Red);
                                                            Console.WriteLine("Nieprawidłowy numer maszyny");
                                                            setForegroundColor(ConsoleColor.White);
                                                        }
                                                        break;
                                                    case "4":
                                                        Console.WriteLine($"Podaj nowe hasło dla użytkownika {currentUser.UserName}");
                                                        string newPass = Console.ReadLine();
                                                        fileManager.ChangePassword(currentUser, newPass);
                                                        break;
                                                    case "5":
                                                        loggedIn = false;
                                                        currentUser = null;
                                                        break;
                                                    default:
                                                        setForegroundColor(ConsoleColor.Red);
                                                        Console.WriteLine("Nieprawidłowy wybór");
                                                        setForegroundColor(ConsoleColor.White);
                                                        break;
                                                }
                                                break;

                                            case Roles.PersonalTrainer:
                                                PrintTrainerMenu(currentUser);
                                                var trainerChoice = Console.ReadLine();
                                                switch (trainerChoice)
                                                {
                                                    case "1":
                                                        var pt = (PersonalTrainer)currentUser;
                                                        if (pt.Members.Count > 0)
                                                        {
                                                            Console.WriteLine("Twoi członkowie:");
                                                            foreach (var member in pt.Members)
                                                            {
                                                                Console.WriteLine($"- {member.UserName}");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            setForegroundColor(ConsoleColor.Red);
                                                            Console.WriteLine("Nie masz jeszcze żadnych członków");
                                                            setForegroundColor(ConsoleColor.White);
                                                        }
                                                        break;
                                                    case "2":
                                                        Console.WriteLine($"Podaj nowe hasło dla użytkownika {currentUser.UserName}");
                                                        string newPass = Console.ReadLine();
                                                        fileManager.ChangePassword(currentUser, newPass);
                                                        break;
                                                    case "3":
                                                        loggedIn = false;
                                                        currentUser = null;
                                                        break;
                                                    default:
                                                        setForegroundColor(ConsoleColor.Red);
                                                        Console.WriteLine("Nieprawidłowy wybór");
                                                        setForegroundColor(ConsoleColor.White);
                                                        break;
                                                }
                                                break;

                                            case Roles.Member:
                                                PrintMemberMenu(currentUser);
                                                var memberChoice = Console.ReadLine();
                                                switch (memberChoice)
                                                {
                                                    case "1":
                                                        viewUserByRoles = GetPrTrainers;
                                                        viewUserByRoles(currentUser, rbac);
                                                        break;
                                                    case "2":
                                                        viewUserByRoles = GetPrTrainers;

                                                        if (viewUserByRoles(currentUser, rbac))
                                                        {
                                                            Console.Write("Wybierz numer trenera: ");
                                                            if (int.TryParse(Console.ReadLine(), out int trainerIndex))
                                                            {
                                                                var m = (Member)currentUser;
                                                                var trainer = rbac.BuyAndTrain(m, trainerIndex);
                                                                if (trainer != null)
                                                                {
                                                                    setForegroundColor(ConsoleColor.Green);
                                                                    Console.WriteLine($"Zakupiono usługę trenera {trainer.UserName}");
                                                                    setForegroundColor(ConsoleColor.White);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                setForegroundColor(ConsoleColor.Red);
                                                                Console.WriteLine("Nieprawidłowy numer trenera");
                                                                setForegroundColor(ConsoleColor.White);
                                                            }
                                                        }
                                                        break;
                                                    case "3":
                                                        var m1 = (Member)currentUser;
                                                        rbac.CancelTraining(m1);
                                                        break;
                                                    case "4":
                                                        Console.WriteLine($"Podaj nowe hasło dla użytkownika {currentUser.UserName}");
                                                        string newPass = Console.ReadLine();
                                                        fileManager.ChangePassword(currentUser, newPass);
                                                        break;
                                                    case "5":
                                                        loggedIn = false;
                                                        currentUser = null;
                                                        break;
                                                    default:
                                                        setForegroundColor(ConsoleColor.Red);
                                                        Console.WriteLine("Nieprawidłowy wybór");
                                                        setForegroundColor(ConsoleColor.White);
                                                        break;
                                                }
                                                break;
                                        }

                                        if (loggedIn)
                                        {
                                            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
                                            Console.ReadKey();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                setForegroundColor(ConsoleColor.Red);
                                Console.WriteLine($"Błąd podczas logowania: {ex.Message}");
                                setForegroundColor(ConsoleColor.White);
                            }
                            break;

                        case "2": // Wyjście
                            ChangeColor(ConsoleColor.Green);
                            Console.WriteLine("\nDziękujemy za skorzystanie z programu. Naciśnij dowolny klawisz, aby kontynuować...");
                            ChangeColor(ConsoleColor.White);
                            Console.ReadKey();
                            exit = true;
                            break;

                        default:
                            setForegroundColor(ConsoleColor.Red);
                            Console.WriteLine("Nieprawidłowy wybór");
                            setForegroundColor(ConsoleColor.White);
                            break;
                    }

                    if (!exit)
                    {
                        ChangeColor(ConsoleColor.Green);
                        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
                        ChangeColor(ConsoleColor.Green);
                        Console.ReadKey();
                    }
                }

            }
            catch (Exception ex)
            {
                setForegroundColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas działania programu: {ex.Message}");
                setForegroundColor(ConsoleColor.White);
            }
        }
    }

}