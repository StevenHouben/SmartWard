using System;
using System.Linq.Expressions;
using ABC.Infrastructure.ActivityBase;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var ac = new ActivitySystem();
            ac.Run("http://127.0.0.1:8080/");

            Console.ReadLine();
        }
    }
}
