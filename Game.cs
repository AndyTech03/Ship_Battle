using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship_Battle
{
    class Game
	{
		/// <summary>
		/// Генератор случайных чисел
		/// </summary>
		private static readonly Random _random = new Random();
		/// <summary>
		/// Размер игрового поля
		/// </summary>
		public const int MAP_SIZE = 10;

		/// <summary>
		/// Список кораблей Компьютера
		/// </summary>
		private List<Ship> _computer_ships = new List<Ship>();
		/// <summary>
		/// Список кораблей Пользователя
		/// </summary>
		private List<Ship> _player_ships = new List<Ship>();
		/// <summary>
		/// Список клеток аттакованных Компьютером
		/// </summary>
		private List<AttackedCell> _computer_attacks = new List<AttackedCell>();
		/// <summary>
		/// Список клеток аттакованных Пользователем
		/// </summary>
		private List<AttackedCell> _player_attacks = new List<AttackedCell>();
		private List<Point> _computer_valid_attacks = new List<Point>();

		public enum FireResult
        {
			Miss,
			Hit,
			Wrong,
			Win,
        }

		public void Start()
		{
			const string eracer = "                                                                       ";
			Point cell;
			Restart_Game();
            while (true)
            {
				// Очистка предыдущего ввода
				Console.SetCursorPosition(0, MAP_SIZE * 2 + 3);
				for (int i = 0; i < 5; i++)
				{
					Console.WriteLine(eracer);
				}
				Console.SetCursorPosition(0, MAP_SIZE * 2 + 3);

				cell = new Point(Player_Input("X = ") - 1, Player_Input("Y = ") - 1);
				var result = Fire(cell, ref _player_attacks, ref _computer_ships);
				switch (result)
                {
					case FireResult.Wrong:
						Console.WriteLine("Клетка уже была аттакована! Нажмите любую клавишу...");
						Console.ReadKey();
						continue;
					case FireResult.Win:
						Console.WriteLine("Вы победили! Нажмите любую клавишу...");
						PaintCell(cell, "░░");
						Console.ReadKey();
						Restart_Game();
						continue;

					case FireResult.Hit:
						PaintCell(cell, "░░");
						break;
					case FireResult.Miss:
						PaintCell(cell, ")(");
						break;
				}
				cell = _computer_valid_attacks[_random.Next(_computer_valid_attacks.Count)];
				_computer_valid_attacks.Remove(cell);
				result = Fire(cell, ref _computer_attacks, ref _player_ships);
				cell.Y += MAP_SIZE + 2;
				switch (result)
				{
					case FireResult.Win:
						Console.WriteLine("Компьютер победил! Нажмите любую клавишу...");
						PaintCell(cell, "░░");
						Console.ReadKey();
						Restart_Game();
						continue;

					case FireResult.Hit:
						PaintCell(cell, "░░");
						break;
					case FireResult.Miss:
						PaintCell(cell, ")(");
						break;
				}
			}
		}

		private FireResult Fire(Point cell, ref List<AttackedCell> attacks, ref List<Ship> ships)
        {
			if (attacks.Any(attack => attack.Location == cell))
            {
				return FireResult.Wrong;
            }
			Ship hited_ship = ships.FirstOrDefault(ship => ship.Decks.ContainsKey(cell));
			if (hited_ship is null)
			{
				attacks.Add(new AttackedCell(cell, true));
				return FireResult.Miss;
			}
			hited_ship.Decks[cell] = false;
			attacks.Add(new AttackedCell(cell, false));
			if (ships.All(ship => ship.Decks.Values.All(deck => deck == false)))
            {
				return FireResult.Win;
            }
			return FireResult.Hit;
        }

		private void PaintCell(Point cell, string pen)
        {
			Console.SetCursorPosition((cell.X + 1) * 2, cell.Y + 1);
			Console.Write(pen);
        }

		private int Player_Input(string title)
        {
			Console.Write(title);
			if (
				int.TryParse(Console.ReadLine(), out int value) && 
				value >= 1 && value <= Game.MAP_SIZE
			)
            {
				return value;
            }
			return Player_Input(title);
        }

		/// <summary>
		/// Начинает игровой раунд
		/// </summary>
		public void Restart_Game()
        {
			// Очищаем список оставшихся кораблей Пользователя
			_computer_ships.Clear();
			// Заполняем его новыми корабляеми
			Randomize_Ships(ref _computer_ships);
			// Очищаем список оставшихся кораблей Компьютера
			_player_ships.Clear();
			// Заполняем его новыми корабляеми
			Randomize_Ships(ref _player_ships);
			Draw_Map();
			Draw_Ships();

			_computer_valid_attacks.Clear();
			for (int x = 0; x < MAP_SIZE; x++)
            {
				for (int y = 0; y < MAP_SIZE; y++)
                {
					_computer_valid_attacks.Add(new Point(x, y));
				}
            }
		}

		/// <summary>
		/// Отрисовывает игровые поля в консоль
		/// </summary>
		private static void Draw_Map()
		{
			Console.SetCursorPosition(0, 0);
			// Игровое поле будет высотой (MAP_SIZE * 2) строк + 3 границы в одну строку
			for (int y = 0; y < MAP_SIZE * 2 + 3; y++)
			{
				// И шириной в MAP_SIZE + 2 грацицы в 1 клетку
				for (int x = 0; x < MAP_SIZE + 2; x++)
				{   // Начнём с поля Компьютера
					if (
						y == 0 || y == MAP_SIZE + 1 ||	// Верхняя и Средняя границы
						y == MAP_SIZE * 2 + 2 ||		// Нижняя граница
						x == 0 || x == MAP_SIZE + 1		// Левая и Правая границы	
					)
					{
						Console.Write("██"); // Два символа Alt + 219
					}
					else
					{
						Console.Write("  "); // Два пробела
					}
				}
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Отрисовывает корабли игроков
		/// </summary>
		private void Draw_Ships()
		{
			Console.ForegroundColor = ConsoleColor.DarkRed; // Установим цвет
			// Переберём все корабли Компьютера
			foreach (Ship ship in _computer_ships)
			{
				foreach (Point deck in ship.Decks.Keys)
				{
					Console.SetCursorPosition((deck.X + 1) * 2, deck.Y + 1);
					Console.Write("██"); // Alt + 219
				}
			}

			Console.ForegroundColor = ConsoleColor.DarkCyan; // Установим цвет
			// Переберём все корабли Пользователя
			foreach (Ship ship in _player_ships)
			{
				foreach (Point deck in ship.Decks.Keys)
				{
					Console.SetCursorPosition((deck.X + 1) * 2, deck.Y + 2 + MAP_SIZE);
					Console.Write("██"); // Alt + 219
				}
			}
			Console.ResetColor(); // Восстановим цвет в значение по умолчанию
			Console.SetCursorPosition(0, MAP_SIZE * 2 + 3);
		}

		/// <summary>
		/// Заполняет список <paramref name="ships_list"/> кораблями, размещая их в случайные позиции
		/// </summary>
		/// <param name="ships_list">Ссылка на переменную списка, которую будет заполнять метод</param>
		private static void Randomize_Ships(ref List<Ship> ships_list)
		{
			// Создадим массив свободных клеток игрового поля
			List<Point> empty_cells = new List<Point>();
			for (int y = 0; y < MAP_SIZE; y++)
			{
				for (int x = 0; x < MAP_SIZE; x++)
				{
					empty_cells.Add(new Point(x, y));
				}
			}

			/// <summary>
			/// Начнём наполнение списка
			/// Заполнять будем в порядке от четырёх-палубного корабля до одно-палубного
			/// </summary>
			for (int size = 4; size >= 1; size--)
			{
				/// <summary>
				/// Число кораблей класса вычисляется по формуле: count = 5 - size:
				///		5 - 4 = 1, 
				///		5 - 3 = 2, 
				///		5 - 2 = 3,
				///		5 - 1 = 4.
				/// </summary>
				for (int i = 0; i < 5 - size; i++)
				{
					// Будущие палубы
					Point[] decks = new Point[size];
					/// <summary>
					/// Габаритные точки области будущего корабля, например:
					/// []#### - Слева Сверху будет min_point
					/// ##██##
					/// ##██##
					/// ####[] - Справа Снизу будет max_point
					/// </summary>
					Point min_point, max_point;
					// Выбираем на угад свободную точку на поле
					Point location = empty_cells[_random.Next(empty_cells.Count)];
					// Сперва рассмотрим корабли с размером больше 1 палубы
					if (size > 1)
					{
						// Выбераем на угад направление корабля
						int dir = _random.Next(4);
						int index = 0;
						// Создаём палубы
						switch (dir)
						{
							case 0:		// Направление Вправо
								for (int x = location.X; x < location.X + size; x++)
									decks[index++] = new Point(x, location.Y);
								break;
							case 1:		// Направление Влево
								for (int x = location.X; x > location.X - size; x--)
									decks[index++] = new Point(x, location.Y);
								break;
							case 2:		// Направление Вверх
								for (int y = location.Y; y > location.Y - size; y--)
									decks[index++] = new Point(location.X, y);
								break;
							default:	// Направление Вниз
								for (int y = location.Y; y < location.Y + size; y++)
									decks[index++] = new Point(location.X, y);
								break;
						}
						// Проверяем что корабль не вышел за границы поля или не зашёл в зону другого корабля
						bool collision = false;
						// Для этого переберём все палубы
						foreach (Point point in decks)
						{
							// Если хоть одна не соответствует условию
							if (empty_cells.Any(location => location == point) == false ||
								point.X < 0 || point.X >= MAP_SIZE ||
								point.Y < 0 || point.Y >= MAP_SIZE
							)
							{
								// Значит произошла коллизия
								collision = true;
								break;
							}
						}
						// А если коллизия произошла
						if (collision)
						{
							// Надо начать создание корабля с самого начала
							i--;
							continue;
						}

						// Находим границы корабля
						int min_x = decks.Min(location => location.X);
						int min_y = decks.Min(location => location.Y);
						int max_x = decks.Max(location => location.X);
						int max_y = decks.Max(location => location.Y);
						// Вычисляем их габаритные точки
						min_point = new Point(min_x - 1, min_y - 1);
						max_point = new Point(max_x + 1, max_y + 1);
					}
                    else
                    {
						// Однопалубный корабль это частный случай...
						// Для него всё гараздо проще
						decks[0] = location;
						min_point = new Point(location.X - 1, location.Y - 1);
						max_point = new Point(location.X + 1, location.Y + 1);
					}

					// Наконец создаём корабль обладая всей необходимой информацией
					Ship ship = new Ship(location.X, location.Y, decks);
					// Добавляем его в список
					ships_list.Add(ship);
					// Удаляем из списка свободных клеток игрового поля те клетки,
					// которые находятся вплотную к новому кораблю
					empty_cells.RemoveAll(
						point => (point.X <= max_point.X && point.X >= min_point.X) &&
						(point.Y <= max_point.Y && point.Y >= min_point.Y)
					);
				}
			}
		}
	}
}
