using System;

namespace DelVRBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var bot = new Bot();
            bot.RunAsyn().GetAwaiter().GetResult();
            
        }
    }
}
