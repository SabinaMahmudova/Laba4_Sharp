using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{ 
    public struct Token
    {
        public String data;
        public int recipient;

        public Token (string data, int recipient)
        {
            this.data = data;
            this.recipient = recipient;
        }
    }
    class Program
    {
        static object locker = new object();  //объект - заглушка
        static AutoResetEvent autoEvent;    //событие синхронизации,
        //автоматически изменяется с состояния с сигналом на состояние без сигнала всегда при активации потока
        static void Main(string[] args)
        {
            autoEvent = new AutoResetEvent(false); 
            int n = 10;
            Thread[] threads = new Thread[10];
            Token token = new Token("Token", 8);
            for (int i = 0; i <= n - 1; i++)
            {
                threads[i] = new Thread(() => run(i, token, n));
                threads[i].Start();
                Thread.Sleep(100);  // без sleep некоторые потоки не успевают выполниться
                autoEvent.Set(); // сигнализирует поток о том что можно продолжать. 
            }    
        }
        static void run(int index, Token tok, int n) {
            lock(locker) //чтобы программа приоставливалась если найден reciever
                         //для предотвращения одновременного выполнения блоков кода
            {
                Console.WriteLine("Thread " + index + " received token for"+ tok.recipient);
                if (tok.recipient == index)
                {
                    Console.WriteLine("I am the reciever " + index + ". Data: " + tok.data);
                    Console.ReadLine();
                }
                else if (index != n - 1)
                {
                    Console.WriteLine("Thread " + index + " sending to" + (index + 1));
                    autoEvent.WaitOne();  //WaitOne() заставляет поток ждать сигнала одиночного события, т.е. новый поток ждет события с помощью WaitOne
                    //блокирует текущий поток пока главный поток не получит сигнал
                }
                else {
                    Console.WriteLine("You reaced the end of the chain. There's no reciever");
                    Console.ReadLine();
                }
            }   
        }
    }
}
