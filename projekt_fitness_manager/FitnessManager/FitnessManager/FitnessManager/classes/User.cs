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
    internal class User
    {
        //abstrackyjna klasa User

        public string UserName { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
        protected string PreparePassword(string password)
        {
            return password.Length == 44 ? password : HashPassword(password);
        }
    }
}
