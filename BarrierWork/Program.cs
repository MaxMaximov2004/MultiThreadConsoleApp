using System;
using System.Collections.Generic;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;


Calc();

void Test()
{
    const int threadCount = 5;
    var barrier = new SimpleBarrier(threadCount);
    var threads = new List<Thread>();

    for (int i = 0; i < threadCount; i++)
    {

        var thread = new Thread(() =>
        {
            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: работаю до барьера...");
            Thread.Sleep(new Random().Next(100, 700)); // Имитация важной работы

            Console.WriteLine($"\tПоток {Thread.CurrentThread.ManagedThreadId}: достиг барьера, жду остальных...");

            //barrier.SignalAndWait();

            Console.WriteLine($"\t\tПоток {Thread.CurrentThread.ManagedThreadId}: все собрались, продолжаю работу!");
            Thread.Sleep(new Random().Next(100, 500)); // Ещё работа
            Console.WriteLine($"\t\t\tПоток {Thread.CurrentThread.ManagedThreadId}: завершил работу.");
        });
        threads.Add(thread);
        thread.Start();
    }


    foreach (var thread in threads)
    {
        thread.Join();
    }

    Console.WriteLine("\n=== Все потоки завершились ===");
}


void Calc()
{
    var data_1 = new List<double>() { 23.4, 21.4, 3.344, 6.499, 2.455, 15, 20.4, 3.4, 203.4 };
    var data_2 = new List<double>() { 23.4, 21.4, 3.344, 6.499, 2.455, 15, 20.4, 3.4, 203.4 };
    var operators = new List<oper>() { oper.sum, oper.sum, oper.mul, oper.dif, oper.dif, oper.div, oper.div, oper.mul, oper.mul };

    if ((data_1.Count != data_2.Count) && (data_2.Count != operators.Count)) { throw new ArgumentException("List must be ONE size!"); }

    var threads = new List<Thread>();
    const int threadCount = 5;
    var barrier = new SimpleBarrier(threadCount);

    for (int i = 0;i< threadCount; i++)
    {
        var thread = new Thread(() => 
        {
            var resault = new List<double>() {0,0,0,0,0,0,0,0,0};
            
            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} начал вычисления");

            for (int j = 0; j < data_1.Count; j++) 
            {

                switch (operators[j])
                {
                    case (oper.sum): { resault[j]=data_1[j]+data_2[j]; break; }
                    case (oper.mul): { resault[j] = data_1[j] * data_2[j]; break; }
                    case (oper.dif): { resault[j] = data_1[j] - data_2[j]; break; }
                    case (oper.div): { resault[j] = data_1[j] / data_2[j]; break; }
                }
            }
            Console.WriteLine($" Поток {Thread.CurrentThread.ManagedThreadId} окончил вычисления и занимается ИБД");
            Thread.Sleep(new Random().Next(100, 700));

            barrier.SignalAndWait();
            string str = "результат ";
            foreach(var res in resault) { str += res.ToString() + " "; }
            Console.WriteLine($" Поток {Thread.CurrentThread.ManagedThreadId} выдал {str}");
        });

        threads.Add(thread);
        thread.Start();
    }

    foreach (var thread in threads)
    {
        thread.Join();
    }

}
enum oper : byte { sum, mul,dif,div };

public class SimpleBarrier
{
    private readonly int _totalThreads;
    private int _waitingThreads = 0;
    private readonly ManualResetEvent _barrierEvent = new ManualResetEvent(false);
    private readonly object _lock = new object();

    public SimpleBarrier(int totalThreads)
    {
        if (totalThreads <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalThreads));

        _totalThreads = totalThreads;
    }

    /// <summary>
    /// Поток достигает барьера и ждёт остальных
    /// </summary>
    public void SignalAndWait()
    {
        lock (_lock)
        {
            _waitingThreads++;

            // Последний поток открывает барьер
            if (_waitingThreads >= _totalThreads)
            {
                _barrierEvent.Set(); // ← Отпускаем всех
            }
        }

        // Ждём, пока все потоки не соберутся
        _barrierEvent.WaitOne();
    }

    /// <summary>
    /// Сброс барьера для повторного использования
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            _waitingThreads = 0;
            _barrierEvent.Reset();
        }
    }
}