using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static System.Math;

namespace Lab15Main
{
    public delegate void ClickHandler();

    public static class UserInterface
    {
        public static List<Button> buttons = new List<Button>();
        public static List<ChartPoint> points = new List<ChartPoint>();
        public static List<Label> labels = new List<Label>();
        public static Label errorMsg = new Label(WINDOW_WIDTH / 2, WINDOW_HEIGHT - 200, "");

        public const uint WINDOW_WIDTH = 1080;
        public const uint WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "Графики";
        public const string FONT = "calibri.ttf";

        public const int SLEEP_INTERVAL = 5;

        public const float EPSI = 0.075f;
        public const float COEF = 10f;
        public const float SCALE = 10f;

        public static RenderWindow MAIN_WINDOW = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), WINDOW_TITLE, Styles.Close);

        public static readonly object locker = new();
        static CancellationTokenSource source = new CancellationTokenSource();        

        static PrioritizedThread thread1 = new PrioritizedThread();
        static PrioritizedThread thread2 = new PrioritizedThread();
        static PrioritizedThread thread3 = new PrioritizedThread();

        public static bool IsDrawing()
        {
            return thread1.Thread.IsAlive || thread2.Thread.IsAlive || thread3.Thread.IsAlive;
        }

        public static void Draw(RenderWindow window)
        {
            window.SetActive(true);
            window.Clear(SFML.Graphics.Color.White);

            lock(locker)
            {
                foreach (Button button in buttons)
                {
                    button.Draw(window);
                }
            }            

            lock(locker)
            {
                foreach (ChartPoint point in points)
                {
                    point.Draw(window);
                }
            }
            
            foreach (Label label in labels)
            {
                label.Draw(window);
            }

            errorMsg.Draw(window);

            window.Display();
            window.SetActive(false);
        }

        public static void Close(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var window = (RenderWindow)sender;
                window.Close();
            }
        }

        public static void MousePressed(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var window = (RenderWindow)sender;
                var position = Mouse.GetPosition(window);
                if (((MouseButtonEventArgs)e).Button == Mouse.Button.Left)
                {
                    foreach (Button element in buttons)
                    {
                        if (element.CheckClick(position))
                        {
                            element.Click();
                            break;
                        }
                    }
                }
            }
        }

        public static void Stop()
        {
            source.Cancel();
            errorMsg.Txt = "";
            Draw(MAIN_WINDOW);
        }

        public static void ClearChart()
        {
            if (IsDrawing())
            {
                errorMsg.Txt = "Stop drawing first";
            }
            else
            {
                errorMsg.Txt = "";
                points = new List<ChartPoint>();
            }
            Draw(MAIN_WINDOW);
        }

        public static void Begin()
        {
            Stop();
            ClearChart();
            source = new CancellationTokenSource();

            thread1.Thread = new Thread(Function1);
            thread2.Thread = new Thread(Function2);
            thread3.Thread = new Thread(Function3);

            thread1.Thread.Priority = ThreadPriority.Highest;
            thread2.Thread.Priority = ThreadPriority.Highest;
            thread3.Thread.Priority = ThreadPriority.Highest;

            thread1.Thread.Start(source.Token);
            thread2.Thread.Start(source.Token);
            thread3.Thread.Start(source.Token);
        }

        public static void SubscribeEvents(RenderWindow window)
        {
            window.Closed += Close;
            window.MouseButtonPressed += MousePressed;
        }

        public static void AddUI()
        {
            buttons.Add(new Button(1000, 50, 120, 50, "Start drawing", Begin));
            buttons.Add(new Button(1000, 110, 120, 50, "Stop", Stop));
            buttons.Add(new Button(1000, 170, 120, 50, "Clear", ClearChart));

            buttons.Add(new ThreadPriorityButton(740, 50, 50, 50, thread1, 1, SFML.Graphics.Color.Red));
            buttons.Add(new ThreadPriorityButton(740, 110, 50, 50, thread1, 2, SFML.Graphics.Color.Red));
            buttons.Add(new ThreadPriorityButton(740, 170, 50, 50, thread1, 3, SFML.Graphics.Color.Red));

            buttons.Add(new ThreadPriorityButton(810, 50, 50, 50, thread2, 1, SFML.Graphics.Color.Green));
            buttons.Add(new ThreadPriorityButton(810, 110, 50, 50, thread2, 2, SFML.Graphics.Color.Green));
            buttons.Add(new ThreadPriorityButton(810, 170, 50, 50, thread2, 3, SFML.Graphics.Color.Green));

            buttons.Add(new ThreadPriorityButton(880, 50, 50, 50, thread3, 1, SFML.Graphics.Color.Blue));
            buttons.Add(new ThreadPriorityButton(880, 110, 50, 50, thread3, 2, SFML.Graphics.Color.Blue));
            buttons.Add(new ThreadPriorityButton(880, 170, 50, 50, thread3, 3, SFML.Graphics.Color.Blue));

            labels.Add(new Label(810, 220, "Thread sequence"));
            errorMsg.Color = SFML.Graphics.Color.Red;
        }

        public static void Function1(object? token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            if (thread2.Priority < thread1.Priority && thread2.Thread.ThreadState != ThreadState.Unstarted)
            {
                thread2.Thread.Join();
            }
            if (thread3.Priority < thread1.Priority && thread3.Thread.ThreadState != ThreadState.Unstarted)
            {
                thread3.Thread.Join();
            }
            for (float x = 0.0f; x < WINDOW_WIDTH / SCALE; x += EPSI)
            {
                if (((CancellationToken)token).IsCancellationRequested)
                {
                    return;
                }                
                float y = (float)Math.Sin(x);
                var coordX = x * COEF;
                var coordY = 0 - y * COEF + WINDOW_HEIGHT / 2; 
                lock(locker)
                {
                    points.Add(new ChartPoint(coordX, coordY, SFML.Graphics.Color.Red));
                    Draw(MAIN_WINDOW);
                }
                if (coordY < 0)
                {
                    return;
                }
                Thread.Sleep(SLEEP_INTERVAL);
            }
        }

        public static void Function2(object? token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            if (thread1.Priority < thread2.Priority && thread1.Thread.ThreadState != ThreadState.Unstarted)
            {
                thread1.Thread.Join();
            }
            if (thread3.Priority < thread2.Priority && thread3.Thread.ThreadState != ThreadState.Unstarted)
            {
                thread3.Thread.Join();
            }
            for (float x = 0.0f; x < WINDOW_WIDTH / SCALE; x += EPSI)
            {
                if (((CancellationToken)token).IsCancellationRequested)
                {
                    return;
                }                
                float y = 4 * x * x - 2 * x - 22;
                var coordX = x * COEF;
                var coordY = 0 - y * COEF + WINDOW_HEIGHT / 2;
                lock (locker)
                {
                    points.Add(new ChartPoint(coordX, coordY, SFML.Graphics.Color.Green));
                    Draw(MAIN_WINDOW);
                }
                if (coordY < 0)
                {
                    return;
                }
                Thread.Sleep(SLEEP_INTERVAL);
            }
        }

        public static void Function3(object? token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            if (thread1.Priority < thread3.Priority && thread1.Thread.ThreadState != ThreadState.Unstarted)
            {
                thread1.Thread.Join();
            }
            if (thread2.Priority < thread3.Priority && thread2.Thread.ThreadState != ThreadState.Unstarted)
            {
                thread2.Thread.Join();
            }
            for (float x = 0.0f; x < WINDOW_WIDTH / SCALE; x += EPSI)
            {
                if (((CancellationToken)token).IsCancellationRequested)
                {
                    return;
                }              
                float y = (float)Math.Log10(x * x) / (float)Math.Pow(x, 3);
                var coordX = x * COEF;
                var coordY = 0 - y * COEF + WINDOW_HEIGHT / 2;
                lock (locker)
                {
                    points.Add(new ChartPoint(coordX, coordY, SFML.Graphics.Color.Blue));
                    Draw(MAIN_WINDOW);
                }
                if (coordY < 0)
                {
                    return;
                }
                Thread.Sleep(SLEEP_INTERVAL);
            }
        }

        public static void Start()
        {
            thread1.Priority = 1;
            thread2.Priority = 1;
            thread3.Priority = 1;

            thread1.Thread = new Thread(Function1);
            thread2.Thread = new Thread(Function2);
            thread3.Thread = new Thread(Function3);

            AddUI();
            SubscribeEvents(MAIN_WINDOW);
            Draw(MAIN_WINDOW);

            while (MAIN_WINDOW.IsOpen)
            { 
                MAIN_WINDOW.DispatchEvents();
            }
        }
    }
}