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

namespace FitnessManager.classes
{
    internal class PersonalTrainer : User
    {
        //klasa PersonalTrainer dziedzicząca po User

        public List<User> Members { get; set; }
        public PersonalTrainer(string userName, string password, RBAC rbac)
        {
            Members = new List<User>();
            UserName = userName;
            Password = PreparePassword(password);
            Role = Roles.PersonalTrainer;
            if (File.Exists($"{UserName}.txt"))
            {
                foreach (var line in File.ReadLines($"{UserName}.txt").ToList())
                {
                    foreach (var line2 in File.ReadLines($"users.txt").ToList())
                        if (line.Split(',')[1] == line2.Split(',')[0])
                        {
                            Members.Add(new Member(line2.Split(',')[0], line2.Split(',')[1], rbac));
                        }
                }
            }
        }

    }
}
