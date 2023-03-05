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

        public static RenderWindow MAIN_WINDOW = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), WINDOW_TITLE, Styles.Close);

        public const float EPSI = 0.075f;
        public const float COEF = 10f;
        public const float SCALE = 10f;

        public static readonly object locker = new();

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

        public static void SubscribeEvents(RenderWindow window)
        {
            window.Closed += Close;
            window.MouseButtonPressed += MousePressed;
        }

        public static void AddButtons()
        {
            buttons.Insert(0, new Button(1000, 50, 100, 50, "New Vertex", () => { }));
        }

        public static void Function1()
        {
            for (float x = 0.0f; x < WINDOW_WIDTH / SCALE; x += EPSI)
            {
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
                    break;
                }
            }
        }

        public static void Function2()
        {
            for (float x = 0.0f; x < WINDOW_WIDTH / SCALE; x += EPSI)
            {
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
                    break;
                }
            }
        }

        public static void Function3()
        {
            for (float x = 0.0f; x < WINDOW_WIDTH / SCALE; x += EPSI)
            {
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
                    break;
                }
            }
        }

        public static void Start()
        {

            AddButtons();
            SubscribeEvents(MAIN_WINDOW);
            Draw(MAIN_WINDOW);

            Thread f1 = new Thread(Function1);
            Thread f2 = new Thread(Function2);
            Thread f3 = new Thread(Function3);

            f1.Start();
            f2.Start();
            f3.Start();

            while (MAIN_WINDOW.IsOpen)
            {
                MAIN_WINDOW.DispatchEvents();
            }
        }
    }
}