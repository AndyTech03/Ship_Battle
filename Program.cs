using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship_Battle
{
    class Program
    {
		public static void Main(string[] args)
        {
			Game game = new Game();
            while (true)
            {
                Console.ReadKey();
                game.Restart_Game();
            }
		}
	}
}
