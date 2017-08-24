using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJwtToken
{
    class Program
    {
        static void Main(string[] args)
        {
            TokenClass tc = new TokenClass();
            tc.Test();
            Console.ReadLine();
        }
    }
}
