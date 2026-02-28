using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadConsoleApp
{
    public class SimpleView
    {
        protected string title;
        protected List<String> functions;
        protected List<String> text;
        protected object AtomicFlagChange = false;
        protected object Locker = new object();

        public SimpleView(string title, List<String> functions)
        {
            this.title = title;
            this.functions = functions;
            text = new List<String>() {""};
        }

        public string GetScreen()
        {
            Interlocked.Exchange(ref this.AtomicFlagChange, false);

            lock (Locker)
            {
                string str = $"--=={title}==--\n";
                foreach (String function in functions)
                {
                    str += $"\t{function}";
                }
                str += "\n\n";

                foreach (String line in text)
                {
                    str += line;
                }
                return str;

            }
        }

        public bool ScreenChanged() { return (AtomicFlagChange as bool?)??false; }

        public void AddChar(char ch) {


            lock (Locker)
            {
                if (ch == '\n')
                {
                    text.Last().Append('\n');
                    text.Add("");
                }
                else
                {
                    text.Last().Append(ch);
                }
            }

            Interlocked.Exchange(ref this.AtomicFlagChange, true);

        }

        public void RemoveChar()
        {

            lock (Locker)
            {
                if (text.Count > 0)
                {

                    if (text.Last().Length > 0)
                    {
                        text[text.Count - 1] = text.Last().Remove(text.Last().Length - 1);
                    }
                    else
                    {
                        text.RemoveAt(text.Count - 1);
                    }
                }
            }

            Interlocked.Exchange(ref this.AtomicFlagChange, true);

        }

        public void RemoveLine()
        {

            lock (Locker) {
                if (text.Count > 0)
                {
                    text.RemoveAt(text.Count - 1);

                }
            }

            Interlocked.Exchange(ref this.AtomicFlagChange, true);

        }

    }
}
