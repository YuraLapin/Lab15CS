using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab15Main
{
    public class ChartPoint
    {
        public float X;
        public float Y;
        public Color Color;
        public float Radius = 2;

        public ChartPoint(float _x, float _y, Color _color)
        {
            X = _x;
            Y = _y;
            Color = _color;
        }

        public void Draw(RenderWindow window)
        {
            var point = new CircleShape();
            point.Position = new SFML.System.Vector2f(X, Y);
            point.FillColor = Color;
            point.OutlineThickness = 0;
            point.Radius = Radius;
            point.Origin = new SFML.System.Vector2f(Radius / 2, Radius / 2);

            window.Draw(point);
        }
    }
}
