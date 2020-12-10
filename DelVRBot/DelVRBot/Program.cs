using System;

namespace DelVRBot
{

    class Program
    {
        public static bool DebugMode { get; private set; }

        static void Main(string[] args)
        {
#if DEBUG
            DebugMode = true;
#else
            DebugMode = false;
#endif

            if (DebugMode)
                Console.WriteLine("Debug");
            else
                Console.WriteLine("Release");

            var bot = new Bot();
            bot.RunAsyn().GetAwaiter().GetResult();
        }
    }
}