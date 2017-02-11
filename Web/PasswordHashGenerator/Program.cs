using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greenhouse.Core;

namespace PasswordHashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter password");
            var psw = Console.ReadLine();

            var hash = HashHelper.GetMd5Hash(psw);
            Console.WriteLine(hash);

            Console.ReadLine();
        }
    }
}
