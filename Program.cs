using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonersDilemmaArena
{
    class Program
    {
        static void Main(string[] args)
        {
            Arena a = new Arena();
            a.RoundRobin(100);
            a.PrintResults();
            Console.ReadKey();
        }
    }
}
