using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ship_Battle
{
    class AttackedCell
    {
		// Место выстрела
		public Point Location;
		// Выстрел промазал?
		public bool Miss;

		public AttackedCell(int x, int y, bool is_miss)
		{
			Location = new Point(x, y);
			Miss = is_miss;
		}

		public override String ToString()
		{
			if (Miss)
			{
				return "Miss" + Location.ToString();
			}
			return "Hit" + Location.ToString();
		}
	}
}
