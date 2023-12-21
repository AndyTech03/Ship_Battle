using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ship_Battle
{
    class Point
    {
        /// <summary>
        /// X координата
        /// </summary>
		public int X;
        /// <summary>
        /// Y координата
        /// </summary>
		public int Y;

        /// <summary>
        /// Создаёт новую точку с координатами X = <paramref name="x"/>, Y = <paramref name="y"/>
        /// </summary>
        /// <param name="x">X координата</param>
        /// <param name="y">Y координата</param>
		public Point(int x, int y)
		{
			X = x;
			Y = y;
        }

        /// <summary>
        /// Преобразует объект <see cref="Point"/> в строку
        /// </summary>
        /// <returns>Строка вида: "X,Y"</returns>
        public override string ToString()
        {
            return X + "," + Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point point &&
                   X == point.X &&
                   Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point left, Point right)
        {
            return EqualityComparer<Point>.Default.Equals(left, right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }
    }
}
