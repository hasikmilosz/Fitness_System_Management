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
    internal class Menu
    {
        //metoda wypisująca Menu główne
        public static void PrintMainMenu()
        {
            try
            {
                Console.Clear();
                ChangeColor(ConsoleColor.Blue);
                Console.WriteLine("=== Fitness Manager ===");
                ChangeColor(ConsoleColor.White);
                Console.WriteLine("| 1. Zaloguj się      |");
                Console.WriteLine("| 2. Wyjdź z programu |");
                Console.WriteLine("|_____________________|");
                Console.WriteLine();
                Console.Write("Wybierz opcję: ");
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania menu głównego: {ex.Message}");
                ChangeColor(ConsoleColor.White);
            }
        }

        //metoda wypisująca Menu dla admin'a
        public static void PrintAdminMenu(User user)
        {
            try
            {
                Console.Clear();
                ChangeColor(ConsoleColor.Blue);
                Console.WriteLine($"=== Fitness Manager (Admin: {user.UserName}) ===");
                ChangeColor(ConsoleColor.White);
                Console.WriteLine("| 1. Wyświetl wszystkich użytkowników |");
                Console.WriteLine("| 2. Wyświetl adminów                 |");
                Console.WriteLine("| 3. Wyświetl członków                |");
                Console.WriteLine("| 4. Wyświetl trenerów personalnych   |");
                Console.WriteLine("| 5. Wyświetl pracowników             |");
                Console.WriteLine("| 6. Dodaj użytkownika                |");
                Console.WriteLine("| 7. Usuń użytkownika                 |");
                Console.WriteLine("| 8. Dodaj maszynę                    |");
                Console.WriteLine("| 9. Wyświetl maszyny                 |");
                Console.WriteLine("| 10. Usuń maszynę                    |");
                Console.WriteLine("| 11. Zmień hasło                     |");
                Console.WriteLine("| 12. Wyloguj się                     |");
                Console.WriteLine("|_____________________________________|");
                Console.WriteLine();
                Console.Write("Wybierz opcję: ");
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania menu admina: {ex.Message}");
                ChangeColor(ConsoleColor.White);
            }
        }

        //metoda wypisująca Menu dla worker'a
        public static void PrintWorkerMenu(User user)
        {
            try
            {
                Console.Clear();
                ChangeColor(ConsoleColor.Blue);
                Console.WriteLine($"=== Fitness Manager (Pracownik: {user.UserName}) ===");
                ChangeColor(ConsoleColor.White);
                Console.WriteLine("| 1. Dodaj maszynę    |");
                Console.WriteLine("| 2. Wyświetl maszyny |");
                Console.WriteLine("| 3. Usuń maszynę     |");
                Console.WriteLine("| 4. Zmień hasło      |");
                Console.WriteLine("| 5. Wyloguj się      |");
                Console.WriteLine("|_____________________|");
                Console.WriteLine();
                Console.Write("Wybierz opcję: ");
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania menu pracownika: {ex.Message}");
                ChangeColor(ConsoleColor.White);
            }
        }

        //metoda wypisująca Menu dla PersonalTrainer'a
        public static void PrintTrainerMenu(User user)
        {
            try
            {
                Console.Clear();
                ChangeColor(ConsoleColor.Blue);
                Console.WriteLine($"=== Fitness Manager (Trener: {user.UserName}) ===");
                ChangeColor(ConsoleColor.White);
                Console.WriteLine("| 1. Wyświetl swoich członków |");
                Console.WriteLine("| 2. Zmień hasło              |");
                Console.WriteLine("| 3. Wyloguj się              |");
                Console.WriteLine("|_____________________________|");
                Console.WriteLine();
                Console.Write("Wybierz opcję: ");
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania menu trenera: {ex.Message}");
                ChangeColor(ConsoleColor.White);
            }
        }

        //metoda wypisująca Menu dla Memeber'a
        public static void PrintMemberMenu(User user)
        {
            try
            {
                Console.Clear();
                ChangeColor(ConsoleColor.Blue);
                Console.WriteLine($"=== Fitness Manager (Członek: {user.UserName}) ===");
                ChangeColor(ConsoleColor.White);
                Console.WriteLine("| 1. Wyświetl trenerów personalnych |");
                Console.WriteLine("| 2. Kup usługę trenera             |");
                Console.WriteLine("| 3. Anuluj usługę trenera          |");
                Console.WriteLine("| 4. Zmień hasło                    |");
                Console.WriteLine("| 5. Wyloguj się                    |");
                Console.WriteLine("|___________________________________|");
                Console.WriteLine();
                Console.Write("Wybierz opcję: ");
            }
            catch (Exception ex)
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"Błąd podczas wyświetlania menu członka: {ex.Message}");
                ChangeColor(ConsoleColor.White);
            }
        }
    }
}
