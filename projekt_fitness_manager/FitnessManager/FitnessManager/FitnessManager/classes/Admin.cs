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
    internal class Admin: User
    {
        //klasa Admin dziedzicząca po User
        public Admin(string userName, string password)
        {
            UserName = userName;
            Password = PreparePassword(password);
            Role = Roles.Admin;
        }
    }

}
