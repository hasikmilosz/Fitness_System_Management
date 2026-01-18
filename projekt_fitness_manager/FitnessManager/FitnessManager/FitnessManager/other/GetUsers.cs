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
    internal class GetUsers
    {
        //metoda wypisująca listę członków
        public static bool GetMembers(User user, RBAC rbac)
        {
            try
            {
                if (rbac.Users.Count != 0)
                {
                    if (rbac.HasPermission(user, "manageUsers"))
                    {
                        List<User> members = new List<User>();
                        foreach (var u in rbac.Users)
                        {
                            if (u.Role == Roles.Member)
                            {
                                members.Add(u);
                            }
                        }
                        if (members.Count != 0)
                        {
                            int c = 1;
                            foreach (var i in members)
                            {
                                ChangeColor(ConsoleColor.Cyan);
                                Console.WriteLine($"{c}. {i.UserName}, hasło: {i.Password} \n");
                                ChangeColor(ConsoleColor.White);
                                c++;
                            }
                        }
                        else
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine("Nie dodano żadnych członków do systemu");
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
                    Console.WriteLine("Najpierw dodaj użytkowników do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania członków: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }
        //metoda wypisująca listę adminów
        public static bool GetAdmins(User user, RBAC rbac)
        {
            try
            {
                if (rbac.Users.Count != 0)
                {
                    if (rbac.HasPermission(user, "manageUsers"))
                    {
                        List<User> admins = new List<User>();
                        foreach (var u in rbac.Users)
                        {
                            if (u.Role == Roles.Admin)
                            {
                                admins.Add(u);
                            }
                        }
                        if (admins.Count != 0)
                        {
                            int c = 1;
                            foreach (var i in admins)
                            {
                                ChangeColor(ConsoleColor.Cyan);
                                Console.WriteLine($"{c}. {i.UserName}, hasło:  {i.Password}\n");
                                ChangeColor(ConsoleColor.White);
                                c++;
                            }
                        }
                        else
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine("Nie dodano żadnych administratorów do systemu");
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
                    Console.WriteLine("Najpierw dodaj użytkowników do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania administratorów: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }
        //metoda wypisująca listę pracowników
        public static bool GetWorkers(User user, RBAC rbac)
        {
            try
            {
                if (rbac.Users.Count != 0)
                {
                    if (rbac.HasPermission(user, "manageUsers"))
                    {
                        List<User> workers = new List<User>();
                        foreach (var u in rbac.Users)
                        {
                            if (u.Role == Roles.Worker)
                            {
                                workers.Add(u);
                            }
                        }
                        if (workers.Count != 0)
                        {
                            int c = 1;
                            foreach (var i in workers)
                            {
                                ChangeColor(ConsoleColor.Cyan);
                                Console.WriteLine($"{c}. {i.UserName}, hasło:  {i.Password}\n");
                                ChangeColor(ConsoleColor.White);
                                c++;
                            }
                        }
                        else
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine("Nie dodano żadnych pracowników do systemu");
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
                    Console.WriteLine("Najpierw dodaj użytkowników do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania pracowników: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }
        //metoda wypisująca listę trenerów personalnych
        public static bool GetPrTrainers(User user, RBAC rbac)
        {
            try
            {
                if (rbac.Users.Count != 0)
                {
                    if (rbac.HasPermission(user, "manageUsers"))
                    {
                        List<User> pTrainers = new List<User>();
                        foreach (var u in rbac.Users)
                        {
                            if (u.Role == Roles.PersonalTrainer)
                            {
                                pTrainers.Add(u);
                            }
                        }
                        if (pTrainers.Count != 0)
                        {
                            int c = 1;
                            foreach (var i in pTrainers)
                            {
                                ChangeColor(ConsoleColor.Cyan);
                                Console.WriteLine($"{c}. {i.UserName}, hasło: {i.Password}\n");
                                ChangeColor(ConsoleColor.White);
                                c++;
                            }
                        }
                        else
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine("Nie dodano żadnych trenerów personalnych do systemu");
                            ChangeColor(ConsoleColor.White);
                            return false;
                        }
                    }
                    else if (rbac.HasPermission(user, "buy"))
                    {
                        List<User> pTrainers = new List<User>();
                        foreach (var u in rbac.Users)
                        {
                            if (u.Role == Roles.PersonalTrainer)
                            {
                                pTrainers.Add(u);
                            }
                        }
                        if (pTrainers.Count != 0)
                        {
                            int c = 1;
                            foreach (var i in pTrainers)
                            {
                                ChangeColor(ConsoleColor.Cyan);
                                Console.WriteLine($"{c}. {i.UserName}\n");
                                ChangeColor(ConsoleColor.White);
                                c++;
                            }
                        }
                        else
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine("Nie dodano żadnych trenerów personalnych do systemu");
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
                    Console.WriteLine("Najpierw dodaj użytkowników do systemu");
                    ChangeColor(ConsoleColor.White);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania trenerów: {ex.Message}");
                ChangeColor(ConsoleColor.White);
                return false;
            }
            return true;
        }
    }
}
