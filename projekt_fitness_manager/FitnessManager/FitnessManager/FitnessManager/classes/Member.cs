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
    internal class Member : User
    {
        //klasa Member dziedzicząca po User
        public List<User> PrTrainers { get; set; }
        public Member(string userName, string password, RBAC rbac)
        {
            PrTrainers = new List<User>();
            UserName = userName;
            Password = PreparePassword(password);
            Role = Roles.Member;
            foreach (var u in rbac.Users)
            {
                if (u.Role == Roles.PersonalTrainer)
                {
                    if (File.Exists($"{u.UserName}.txt"))
                    {
                        foreach (var line in File.ReadLines($"{u.UserName}.txt").ToList())
                        {
                            if (UserName == line.Split(',')[1])
                            {
                                PrTrainers.Add(u);
                            }
                        }
                    }
                }
            }
        }
    }
}
