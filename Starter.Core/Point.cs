using System;

namespace Starter.Core
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int DistanceTo(Point p)
        {
            int dx = this.X - p.X;
            int dy = this.Y - p.Y;

            return Math.Abs(dx) + Math.Abs(dy);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public override bool Equals(object obj)
        {
            if(obj is Point)
            {
                return this.X == (obj as Point).X && this.Y == (obj as Point).Y;
            }
            return base.Equals(obj);
        }
    }
}