using System.Diagnostics.Contracts;
using System.Threading;

namespace MyThreadPool
{
    internal class Program
    {


        public class MyThreadPool
        {
            private readonly Queue<Action> _queue;
            private readonly Thread[] _thread;

            private readonly object _sysRoot = new object();

            public MyThreadPool(int ThreadCount = 4)
            {
                _thread = new Thread[ThreadCount];
                _queue = new Queue<Action>();
                for (int i = 0; i < ThreadCount; i++)
                {
                    _thread[i]= new Thread(ThreadProc)
                    {
                        IsBackground = true,
                        Name= "MyThreadPool"
                    };
                    _thread[i].Start();
                }

            }
            public void Queue(Action action)
            {


                Monitor.Enter(_sysRoot);
                try
                {
                    _queue.Enqueue(action);
                }
                finally
                {
                    Monitor.Exit(_sysRoot);
                }


            }


            private void ThreadProc(object? obj)
            {
                Action action;
                while (true)
                {
                    try
                    {
                        Monitor.Enter(_sysRoot);
                        if (_queue.Count > 0)
                        {
                            action = _queue.Dequeue();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    finally { Monitor.Exit(_sysRoot); }
                    action();
                }

            }



        }



        static void Main(string[] args)
        {


            MyThreadPool myThreadPool = new MyThreadPool(1);

            myThreadPool.Queue(() => { Console.WriteLine("MyThreadPool"); });

            Console.ReadKey();
        }
    }
}
