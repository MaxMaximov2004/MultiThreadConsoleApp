using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MultiThreadConsoleApp
{
    public class SimpleView:IDisposable
    {
        protected string title;
        protected List<String> functions;
        protected List<String> text;
        protected object AtomicFlagChange = true;
        protected object Locker = new object();
        protected Mutex? FileGuard = null;
        protected string? CurrentDir = null; 

        public SimpleView(string title, List<String> functions)
        {
            this.title = title;
            this.functions = functions;
            text = new List<String>() {""};

            try
            {
                CurrentDir = Directory.GetCurrentDirectory();
            } catch { }

            if (CurrentDir != null) { CurrentDir += $"\\{DateTime.Now.ToString("g")}.txt".Replace(':','_'); }

        }

        public string GetScreen()
        {
            Interlocked.Exchange(ref this.AtomicFlagChange, false);

            lock (Locker)
            {
                string str = $"----===={title}====----\n";
                foreach (String function in functions)
                {
                    str += $"{function}  ";
                }
                str += $"\nfile:{CurrentDir}\n";

                if (File.Exists(CurrentDir)) { str += "\tSaved!\n"; }

                foreach (String line in text)
                {
                    str += line+"\n";
                }
                return str;

            }
        }

        public void SaveData()
        {
            if (CurrentDir != null)
            {
                if (FileGuard == null) { FileGuard = new Mutex(); }

                FileGuard.WaitOne();
                //CurrentDir += $"{DateTime.Now.ToString("g")}.txt";

                if (File.Exists(CurrentDir)) {  File.Delete(CurrentDir); }
                File.WriteAllLines(CurrentDir, text);

                FileGuard.ReleaseMutex();
                Interlocked.Exchange(ref this.AtomicFlagChange, true);
            }
        }

        public bool ScreenChanged() { return (AtomicFlagChange as bool?)??false; }

        public void AddChar(char ch) {


            lock (Locker)
            {
                
                if (ch == '\n')
                {
                    //text[text.Count - 1] += ch;
                    text.Add("");
                }
                else
                {
                    text[text.Count - 1] += ch;
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
                        text[text.Count - 1] = "";
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
                    
                    if(text.Count > 1)
                    {
                        text.RemoveAt(text.Count - 1);
                    } else
                    {
                        text[0] = "";
                    }
                }
            }

            Interlocked.Exchange(ref this.AtomicFlagChange, true);

        }

        public void RemoveAll()
        {
            Interlocked.Exchange(ref text, new List<string>() {""});
            Interlocked.Exchange(ref this.AtomicFlagChange, true);
        }

        public void Dispose()
        {
            FileGuard.Dispose();
        }
    }

    public class SimpleCompareView
    {
        protected string title;
        protected List<String> functions;
        protected List<String>? lines_A = null;
        protected List<String>? lines_B = null;
        protected List<String>? lines_choosed = null;

        protected object AtomicFlagChange = true;
        protected object Locker = new object();

        public SimpleCompareView(string title, List<String> functions)
        {
            this.title = title;
            this.functions = functions;
        }

        public string GetScreen()
        {
            Interlocked.Exchange(ref this.AtomicFlagChange, false);

            lock (Locker)
            {
                string str = $"----===={title}====----\n";
                foreach (String function in functions)
                {
                    str += $"{function}  ";
                }
                
                return str;
            }
        }



    }

    public class SimpleFileWorker
    {
        protected List<String> file_lines = new List<string>();
        protected FileInfo? file = null;
        protected Mutex? file_guard = null;

        private SimpleFileWorker() { }

        public SimpleFileWorker(string path)
        {
            file_guard = new Mutex();
            file = new FileInfo(path);
            if (!file.Exists)
            {
                file.Create();
            }
        }

        public void WriteString(string str) 
        {
            file_guard.WaitOne();
            if (!file.Exists)
            {
                file.Create();
                file_lines.Add(str);

                foreach (string line in file_lines)
                {
                    using (StreamWriter sw = file.AppendText()) { sw.WriteLine(line); }
                }
            }
            else
            {
                using (StreamWriter sw = file.AppendText()) { sw.WriteLine(str); }
            }
            file_guard.ReleaseMutex();
        }

    }
}
