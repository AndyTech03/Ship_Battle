using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ship_Battle
{
    class Ship
    {
		// Положение порабля
		public Point Location;
		// Положение палуб
		public Point[] Decks;
		// Целостность палуб
		public bool[] Decks_Health; 

		public Ship(int x, int y, Point[] decks)
		{
			Location = new Point(x, y);
			Decks = decks;
			Decks_Health = Enumerable.Repeat(true, decks.Length).ToArray();
		}

		public override string ToString()
		{
			return "Ship" + Location.ToString();
		}
	}
}
