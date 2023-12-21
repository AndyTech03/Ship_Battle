using System.Linq;

namespace Ship_Battle
{
    class Ship
	{
		/// <summary>
		/// Положение порабля
		/// </summary>
		public Point Location;
		/// <summary>
		/// Положение палуб
		/// </summary>
		public Point[] Decks;
		/// <summary>
		/// Целостность палуб
		/// </summary>
		public bool[] Decks_Health; 

		public Ship(int x, int y, Point[] decks)
		{
			Location = new Point(x, y);
			Decks = decks;
			Decks_Health = Enumerable.Repeat(true, decks.Length).ToArray();
		}

		public override string ToString()
		{
			return "Ship [" + string.Join(",", Decks.Select(ship => "(" + ship.ToString() + ")")) + "]";
		}
	}
}
