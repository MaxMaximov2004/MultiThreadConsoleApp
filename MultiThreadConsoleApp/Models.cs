using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadConsoleApp
{
    public interface SafeQueue;
    public class ThreadSafeQueueData:SafeQueue, IDisposable
    {
        private Mutex? mut;
        private object? locker;
        private Queue<char>? data;
        private bool Action = false;
        public ThreadSafeQueueData()
        {
            mut = new Mutex();
            data = new Queue<char>();
            locker = new object();
        }

        public void Push(char ch)
        {
            lock (locker)
            {
                Action = true;
                data.Enqueue(ch);
            }
        }

        public char Pop()
        {
            lock (locker)
            {
                char last = data.LastOrDefault('!');
                return last;
            }
        }
        
        public void PopDel()
        {
            lock (locker)
            {
                Action = true;
                if (data.Count > 0)
                {
                    data.Dequeue();
                }
            }
        }

        public bool CheckLastAction()
        {
            lock (locker)
            {
                bool Flag = false;
                if (Action)
                {
                    Flag = true;
                    Action = false;
                }
                return Flag;
            }
        }

        public void Dispose()
        {
            mut.Dispose();
        }
    }

    public class ThreadSafeQueueLineData : SafeQueue, IDisposable
    {
        private Mutex? mut;
        private object? locker;
        private Queue<string>? data;
        private bool Action = false;
        public ThreadSafeQueueLineData()
        {
            mut = new Mutex();
            data = new Queue<string>();
            locker = new object();
        }

        public void Push(string ch)
        {
            lock (locker)
            {
                Action = true;
                data.Enqueue(ch);
            }
        }

        public string Pop()
        {
            lock (locker)
            {
                string last = data.LastOrDefault("!!!");
                return last;
            }
        }

        public void PopDel()
        {
            lock (locker)
            {
                Action = true;
                if (data.Count > 0)
                {
                    data.Dequeue();
                }
            }
        }

        public bool CheckLastAction()
        {
            lock (locker)
            {
                bool Flag = false;
                if (Action)
                {
                    Flag = true;
                    Action = false;
                }
                return Flag;
            }
        }

        public void Dispose()
        {
            mut.Dispose();
        }
    }

    enum Commands:byte { Nothing,Del,NewLine,NewChar,RollBack,RollForward,Exit }

    class ThreadSafeQueueCommands:SafeQueue,IDisposable
    {
        private Mutex? mut;
        private object? locker;
        private Queue<Commands>? commands;
        private bool Action = false;
        public ThreadSafeQueueCommands()
        {
            mut = new Mutex();
            commands = new Queue<Commands>();
            locker = new object();
        }


        public void Push(Commands ch)
        {
            lock (locker)
            {
                Action = true;
                commands.Enqueue(ch);
            }
        }

        public Commands Pop()
        {
            lock (locker)
            {
                Commands last = commands.LastOrDefault(Commands.Nothing);
                return last;
            }
        }

        public void PopDel()
        {
            lock (locker)
            {
                Action = true;
                if (commands.Count > 0)
                {
                    commands.Dequeue();
                }
            }
        }

        public bool CheckLastAction()
        {
            lock (locker)
            {
                bool Flag = false;
                if (Action)
                {
                    Flag = true;
                    Action = false;
                }
                return Flag;
            }
        }

        public void Dispose()
        {
            mut.Dispose();
        }
    }

}
