using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab15Main
{
    internal class ThreadPriorityButton: Button
    {
        public PrioritizedThread Thread;
        public int Priority;
        public Color ActiveColor;

        public ThreadPriorityButton(): base()
        {
            Priority = 1;
            Thread = new PrioritizedThread();
            ClickFunction = () => { };
            Txt = Priority.ToString();
        }

        public ThreadPriorityButton(float x, float y, float width, float height, PrioritizedThread thread, int priority, Color activeColor)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Thread = thread;
            Priority = priority;
            Txt = priority.ToString();
            ActiveColor = activeColor;
            ClickFunction = () => 
            {
                if (UserInterface.IsDrawing())
                {
                    UserInterface.errorMsg.Txt = "Wait for threads to finish or stop them manually";
                    return;
                }
                else
                {
                    Thread.Priority = Priority;
                    UserInterface.errorMsg.Txt = "";
                    UserInterface.Draw(UserInterface.MAIN_WINDOW);
                }
            };
        }

        public override void Draw(RenderWindow window)
        {
            var frame = new RectangleShape();
            frame.FillColor = Color;
            frame.Size = new Vector2f(Width, Height);
            if (Priority == Thread.Priority)
            {
                frame.OutlineColor = ActiveColor;
            }
            else
            {
                frame.OutlineColor = SFML.Graphics.Color.Black;
            }
            frame.OutlineThickness = 2;
            frame.Origin = new Vector2f(Width / 2, Height / 2);
            frame.Position = new Vector2f(X, Y);

            var text = new Text(Txt, Font);
            text.FillColor = SFML.Graphics.Color.Black;
            text.Scale = new Vector2f(0.6f, 0.6f);
            text.Origin = new Vector2f(text.GetLocalBounds().Width / 2, text.GetLocalBounds().Height);
            text.Position = new Vector2f(X, Y);

            window.Draw(frame);
            window.Draw(text);
        }
    }
}