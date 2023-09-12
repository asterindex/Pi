using System;
using System.Collections.Generic;
using System.Threading;

namespace Pi
{
    internal class Program
    {
        private const int Size = 10000000;
        private const int ThreadNumber = 48;

        static bool IsInside(double x, double y)
        {
            double l = Math.Sqrt(x * x + y * y);
            return l <= 1;
        }

        static double CalcPi(List<Point> points)
        {
            double pi = 0;
            int inside = 0;
            foreach (Point p in points)
            {
                if (IsInside(p.X, p.Y))
                    inside++;
            }
            pi = inside / (points.Count * 1.0) * 4.0;
            return pi;
        }

        static Point GeneratePoint()
        {
            Random random = new Random();
            Point p = new Point(random.NextDouble(), random.NextDouble());
            return p;
        }

        static public void Run(object obj)
        {
            Container container = (Container)obj;
            List<Point> points = new List<Point>();
            for (int i = 0; i < Size / ThreadNumber; i++)
            {
                Point p = GeneratePoint();
                points.Add(p);
            }

            double pi = CalcPi(points);
            container.Pi = pi;
            container.Event.Signal();

        }
        
        public static void Main(string[] args)
        {
            // one thread
            DateTime t1 = DateTime.Now;
            List<Point> points = new List<Point>();
            for (int i = 0; i < Size; i++)
            {
                Point p = GeneratePoint();
                points.Add(p);
            }

            double pi = CalcPi(points);
            DateTime t2 = DateTime.Now;
            Console.WriteLine(pi + " for " + (t2-t1).TotalSeconds + " sec");
            
            // multi threading

            DateTime t3 = DateTime.Now;
            List<Container> containers = new List<Container>();
            CountdownEvent ev = new CountdownEvent(ThreadNumber);
            for (int i = 0; i < ThreadNumber; i++)
            {
                Container container = new Container();
                container.Event = ev;
                containers.Add(container);
                Thread thread = new Thread(new ParameterizedThreadStart(Run));
                thread.Start(container);
            }

            ev.Wait();

            pi = 0;
            foreach (Container c in containers)
            {
                pi = pi + c.Pi;
            }

            pi = pi / ThreadNumber;
            DateTime t4 = DateTime.Now;
            Console.WriteLine(pi + " for " + (t4-t3).TotalSeconds + " sec");
        }
    }
}