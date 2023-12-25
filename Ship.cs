using System.Linq;
using System;
using System.Collections.Generic;

namespace Ship_Battle
{
    class Ship
	{
		/// <summary>
		/// Положение порабля
		/// </summary>
		public Point Location;
		/// <summary>
		/// Палубы корабля
		/// </summary>
		public Dictionary<Point, bool> Decks;

		public Ship(int x, int y, Point[] decks)
		{
			Location = new Point(x, y);
			Decks = new Dictionary<Point, bool>();
			foreach(Point cell in decks)
            {
				Decks.Add(cell, true);
			}
		}

		public override string ToString()
		{
			return "Ship [" + string.Join(",", Decks.Select(ship => "(" + ship.ToString() + ")")) + "]";
		}
	}
}
