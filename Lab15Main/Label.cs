using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Lab15Main
{
    public class Label
    {
        public float X;
        public float Y;
        public string Txt;
        public Font Font = new Font(UserInterface.FONT);
        public Color Color = Color.Black;

        public Label(float x, float y, string txt)
        {
            X = x;
            Y = y;
            Txt = txt;
        }

        public void Draw(RenderWindow window)
        {
            var text = new SFML.Graphics.Text(Txt, Font);
            text.FillColor = Color;
            text.Scale = new Vector2f(0.6f, 0.6f);
            text.Origin = new Vector2f(text.GetLocalBounds().Width / 2, text.GetLocalBounds().Height);
            text.Position = new Vector2f(X, Y);

            window.Draw(text);
        }
    }
}