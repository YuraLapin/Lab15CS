using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace Lab15Main
{
    public delegate void ClickHandler();

    public static class UserInterface
    {
        public static List<Button> buttons = new List<Button>();
        public static List<ChartPoint> points = new List<ChartPoint>();

        public const uint WINDOW_WIDTH = 1080;
        public const uint WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "Графики";
        public const string FONT = "calibri.ttf";

        public const float EPSI = 0.075f;
        public const float COEF = 10f;
        public const float SCALE = 10f;

        public static RenderWindow MAIN_WINDOW = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), WINDOW_TITLE, Styles.Close);

        public static readonly object locker = new();
        static CancellationTokenSource source = new CancellationTokenSource();

        static Thread f1 = new Thread(Function1);
        static Thread f2 = new Thread(Function2);
        static Thread f3 = new Thread(Function3);

        public static void Draw(RenderWindow window)
        {
            window.SetActive(true);
            window.Clear(SFML.Graphics.Color.White);

            buttons.Reverse();
            foreach (Button element in buttons)
            {
                element.Draw(window);
            }
            buttons.Reverse();

            points.Reverse();
            foreach (ChartPoint element in points)
            {
                element.Draw(window);
            }
            points.Reverse();

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

        public static void ClearChart()
        {
            source.Cancel();
            points = new List<ChartPoint>();
        }

        public static void Begin()
        {
            ClearChart();
            source = new CancellationTokenSource();

            f1 = new Thread(Function1);
            f2 = new Thread(Function2);
            f3 = new Thread(Function3);

            f1.Start(source.Token);
            f2.Start(source.Token);
            f3.Start(source.Token);
        }

        public static void SubscribeEvents(RenderWindow window)
        {
            window.Closed += Close;
            window.MouseButtonPressed += MousePressed;
        }

        public static void AddButtons()
        {
            buttons.Insert(0, new Button(1000, 50, 120, 50, "Start drawing", Begin));
            buttons.Insert(0, new Button(1000, 110, 120, 50, "Clear", ClearChart));
        }

        public static void Function1(object? token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                for (float x = 0.0f; x < WINDOW_WIDTH / SCALE; x += EPSI)
                {
                    if (((CancellationToken)token).IsCancellationRequested)
                    {
                        return;
                    }
                    float y = (float)Math.Sin(x);
                    var coordX = x * COEF;
                    var coordY = 0 - y * COEF + WINDOW_HEIGHT / 2;
                    lock (locker)
                    {
                        points.Add(new ChartPoint(coordX, coordY, SFML.Graphics.Color.Red));
                        Draw(MAIN_WINDOW);
                    }
                    if (coordY < 0)
                    {
                        return;
                    }
                }
            }
            catch (ThreadInterruptedException) { }
        }

        public static void Function2(object? token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
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
                }
            }
            catch (ThreadInterruptedException) { }            
        }

        public static void Function3(object? token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
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
                }
            }
            catch (ThreadInterruptedException) { }            
        }

        public static void Start()
        {

            AddButtons();
            SubscribeEvents(MAIN_WINDOW);
            Draw(MAIN_WINDOW);

            while (MAIN_WINDOW.IsOpen)
            {
                MAIN_WINDOW.DispatchEvents();
            }
        }
    }
}