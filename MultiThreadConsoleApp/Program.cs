using System.Threading;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MultiThreadConsoleApp;
/*
ThreadSafeQueueData data = new ThreadSafeQueueData();
long last_length = 0;
object locker = new object();

void KeyBoardReader()
{
    while (true)
    {

        ConsoleKeyInfo consoleKey = Console.ReadKey(true);

         Debug.WriteLine($"Key:{consoleKey.Key.ToString()}\tKeyChar:{consoleKey.KeyChar}\tModifires:{consoleKey.Modifiers}");
         data.Push(consoleKey.KeyChar);
        
    }
}

void ScreenBuilder()
{
    
    while (true) {

        if (data.CheckLastAction())
        { 
            Console.Write(data.Pop());
        }
    }
}

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;


Thread KeySpectator = new Thread(KeyBoardReader);
Thread ScreenPainter = new Thread(ScreenBuilder);

KeySpectator.Start();
ScreenPainter.Start();
*/

Manager.Run();

static class Manager
{
    private static ThreadSafeQueueData data = new ThreadSafeQueueData();
    private static ThreadSafeQueueCommands commands = new ThreadSafeQueueCommands();

    private static void KeyBoardReader()
    {
        while (true)
        {

            ConsoleKeyInfo consoleKey = Console.ReadKey(true);

            Debug.WriteLine($"Key:{consoleKey.Key.ToString()}\tKeyChar:{consoleKey.KeyChar}\tModifires:{consoleKey.Modifiers}");

            //data.Push(consoleKey.KeyChar);
            switch (consoleKey.Key,consoleKey.Modifiers) {
                case (ConsoleKey.Enter, ConsoleModifiers.None):{data.Push('\n');break; }
                default: { if (Char.IsLetter(consoleKey.KeyChar)) { data.Push(consoleKey.KeyChar); } ;break; }
            }
            
        }
    }

    private static void ScreenBuilder()
    {

        while (true)
        {

            if (data.CheckLastAction())
            {
                Console.Write(data.Pop());
            }
        }
    }

    public static void Run()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;


        Thread KeySpectator = new Thread(KeyBoardReader);
        Thread ScreenPainter = new Thread(ScreenBuilder);

        KeySpectator.Start();
        ScreenPainter.Start();

    }

}
