using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadConsoleApp
{
    public interface SafeQueue;
    public class ThreadSafeQueueData:SafeQueue
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
    }


    enum Commands:byte { Nothing,Del,NewLine,RollBack,RollForward,Exit }

    class ThreadSafeQueueCommands:SafeQueue
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
    }

}
