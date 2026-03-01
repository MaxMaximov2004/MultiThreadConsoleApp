using System.Threading;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MultiThreadConsoleApp;


SimpleEditorManager.Run();

static class SimpleEditorManager
{
    private static ThreadSafeQueueData data = new ThreadSafeQueueData();
    private static ThreadSafeQueueCommands commands = new ThreadSafeQueueCommands();
    private static SimpleView screen = new SimpleView("Simple editor", ["enter - new line","del - del all", "ctrl+backspace - del last char", "shift+backspace - del last line", "ctrl+q - left", "ctrl+s - save"]);
    private static void KeyBoardReader()
    {
        while (true)
        {
            
            if (commands.CheckLastAction())
            {
                switch (commands.Pop())
                {
                    case (Commands.Exit): {  return; }
                }
            }

            ConsoleKeyInfo consoleKey = Console.ReadKey(true);


            switch (consoleKey.Key,consoleKey.Modifiers) {
                case (ConsoleKey.Enter, ConsoleModifiers.None):{ data.Push('\n');screen.AddChar('\n');break; }
                case (ConsoleKey.Delete, ConsoleModifiers.None): { screen.RemoveAll(); break; }
                case (ConsoleKey.Backspace, ConsoleModifiers.Control): {screen.RemoveChar(); break; }
                case (ConsoleKey.Backspace, ConsoleModifiers.Shift): { screen.RemoveLine(); break; }
                case (ConsoleKey.Q, ConsoleModifiers.Control): 
                { 
                        commands.Push(Commands.Exit);
                        return;
                }
                case (ConsoleKey.S, ConsoleModifiers.Control):
                {
                        Thread FileSave = new Thread(screen.SaveData);
                        FileSave.Start();
                        break;
                }
                default: { if (Char.IsLetter(consoleKey.KeyChar)) { data.Push(consoleKey.KeyChar);  screen.AddChar(consoleKey.KeyChar); } ;break; }
            }
            
        }
    }

    private static void ScreenBuilder()
    {

        while (true)
        {
            if (commands.CheckLastAction())
            {
                switch (commands.Pop())
                {
                    case (Commands.Exit): { return; }
                }
            }

            if (screen.ScreenChanged())
            {
                Console.Clear();
                Console.WriteLine(screen.GetScreen());
            }
        }
    }

    public static void Run()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;


        Thread KeySpectator = new Thread(KeyBoardReader);
        Thread ScreenPainter = new Thread(ScreenBuilder);
        ScreenPainter.Priority = ThreadPriority.Highest;

        KeySpectator.Start();
        ScreenPainter.Start();

        
    }

}
/*
static class SimpleCompareManager
{
    //private static ThreadSafeQueueLineData file_A = new ThreadSafeQueueLineData();
    //private static ThreadSafeQueueLineData file_B = new ThreadSafeQueueLineData();
    private static ThreadSafeQueueLineData file_resault = new ThreadSafeQueueLineData();
    private static ThreadSafeQueueCommands commands = new ThreadSafeQueueCommands();
    private static SimpleCompareView? screen = null;
    private static SimpleFileWorker? file = null;

    public static void RunHandled()
    {
        screen = new SimpleCompareView("Simple Comparer", ["ctrl+a - choose first","ctrl+b - choose second"]);
        string path = Directory.GetCurrentDirectory() + $"{DateTime.Now.ToString("g").Replace(':','_')}.txt"; ;
        file = new SimpleFileWorker(path);


    }

    private static void KeyBoardReader()
    {
        while (true)
        {

            if (commands.CheckLastAction())
            {
                switch (commands.Pop())
                {
                    case (Commands.Exit): { return; }
                }
            }

            ConsoleKeyInfo consoleKey = Console.ReadKey(true);


            switch (consoleKey.Key, consoleKey.Modifiers)
            {
                
            }

        }
    }

    private static void ScreenBuilder()
    {

        while (true)
        {
            if (commands.CheckLastAction())
            {
                switch (commands.Pop())
                {
                    case (Commands.Exit): { return; }
                }
            }

            if (screen.ScreenChanged())
            {
                Console.Clear();
                Console.WriteLine(screen.GetScreen());
            }
        }
    }

}
*/