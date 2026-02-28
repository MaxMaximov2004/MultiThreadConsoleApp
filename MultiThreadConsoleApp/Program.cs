using System.Threading;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MultiThreadConsoleApp;


Manager.Run();

static class Manager
{
    private static ThreadSafeQueueData data = new ThreadSafeQueueData();
    private static ThreadSafeQueueCommands commands = new ThreadSafeQueueCommands();
    private static SimpleView screen = new SimpleView("Simple editor", ["enter - new line","del - del line" ]);
    private static void KeyBoardReader()
    {
        while (true)
        {
            ConsoleKeyInfo consoleKey = Console.ReadKey(true);
            Debug.WriteLine($"Key:{consoleKey.Key.ToString()}\tKeyChar:{consoleKey.KeyChar}\tModifires:{consoleKey.Modifiers}");


            switch (consoleKey.Key,consoleKey.Modifiers) {
                case (ConsoleKey.Enter, ConsoleModifiers.None):{ data.Push('\n'); screen.AddChar('\n'); ;break; }
                case (ConsoleKey.Delete, ConsoleModifiers.None): { Console.Clear(); break; }
                /*case (ConsoleKey.Backspace, ConsoleModifiers.None):
                {
                        Console.Clear();
                        
                        data.PopDel();
                        break;
                }*/
                default: { if (Char.IsLetter(consoleKey.KeyChar)) { data.Push(consoleKey.KeyChar); screen.AddChar(consoleKey.KeyChar); } ;break; }
            }
            
        }
    }

    private static void ScreenBuilder()
    {

        while (true)
        {
            if (screen.ScreenChanged())
            {
                Console.Clear();
                Console.WriteLine(screen.GetScreen());
            }
            /*if (data.CheckLastAction())
            {
                Console.Write(data.Pop());
            }*/
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
