using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
          
            Searchfight.SearchEngineAsync(args).Wait();
            Console.Read();

        }
    }
}
