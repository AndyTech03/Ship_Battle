using System;

namespace Ship_Battle
{
    class AttackedCell
    {
		/// <summary>
		/// Место выстрела
		/// </summary>
		public Point Location;

		/// <summary>
		/// Выстрел промазал?
		/// </summary>
		public bool Miss;

		public AttackedCell(Point cell, bool is_miss)
		{
			Location = cell;
			Miss = is_miss;
		}

		public override string ToString()
		{
			if (Miss)
			{
				return "Miss (" + Location.ToString() + ")";
			}
			return "Hit (" + Location.ToString() + ")";
		}
	}
}
