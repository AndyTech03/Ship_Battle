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
		// Две карты, четыре границы, две подписи оси X-ов
		public const int FIELD_ROWS = MAP_SIZE * 2 + 4 + 2;

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

		/// <summary>
		/// Исходы выстрела
		/// </summary>
		public enum FireResult
		{
			/// <summary>
			/// Невозможный
			/// </summary>
			Wrong,
			/// <summary>
			/// Промазал
			/// </summary>
			Miss,
			/// <summary>
			/// Попал
			/// </summary>
			Hit,
			/// <summary>
			/// Потопил
			/// </summary>
			Destroy,
			/// <summary>
			/// Победил
			/// </summary>
			Win,
        }

		/// <summary>
		/// Функция начинающая игру
		/// </summary>
		public void Start()
		{
			// Начинаем первый раунд вручную
			Restart_Game();
			// Начинаем главный цикл игры
            while (true)
            {
				bool player_win = Player_Turn();
				if (player_win)
                {
					Restart_Game();
                }
			}
		}

		/// <summary>
		/// Метод реализующий ход Пользователя.
		/// Ход считается завершённым как только игрок промазал, или как только он победит.
		/// </summary>
		/// <returns>Возвращает true - если он победил, false - если он промазал</returns>
		private bool Player_Turn()
		{
			// Ластик, в нашем случае пустая строка длинной во всю консоль...
			string eraser = new string(' ', Console.BufferWidth);
			while (true)
			{
				// Запоминаем на какой строке закончился ввод
				int last_line = Console.CursorTop + 1;
				// Поднимаемся нас строку под игровым полем
				Console.SetCursorPosition(0, FIELD_ROWS);
				// Очищаем строки
				for (int i = 0; i < last_line - FIELD_ROWS; i++)
				{
					Console.WriteLine(eraser);
				}
				// Вновь поднимаемся
				Console.SetCursorPosition(0, FIELD_ROWS);
				// Пользователь вводит координаты выстрела
				Point cell = new Point(Player_Input("X = "), Player_Input("Y = "));
				// Производим выстрел
				(FireResult result, Ship hited_ship) = Fire(cell, ref _player_attacks, ref _computer_ships);
				switch (result)
				{
					case FireResult.Wrong:
						{
							Console.WriteLine("Сюда нельзя выстрелить!");
							Console.WriteLine("Нажмите любую клавишу чтобы продолжить...");
							Console.ReadKey();
						}
						// Если выстрел неверный вводим по новой
						break;
					case FireResult.Miss:
						{
							Paint_Computer_Cell(cell, false);
							Console.WriteLine("Мимо, но так как противник не умеет стрелять...");
							Console.WriteLine("Нажмите любую клавишу чтобы продолжить...");
							Console.ReadKey();
						}
						// Ход окончен, очередь переходит к Компьютеру
						return false;
					case FireResult.Hit:
						{
							Paint_Computer_Cell(cell, true);
						}
						break;
					case FireResult.Destroy:
						{
							Paint_Computer_Cell(cell, true);
							int min_x = hited_ship.Decks.Keys.Min(location => location.X) - 1;
							int min_y = hited_ship.Decks.Keys.Min(location => location.Y) - 1;
							int max_x = hited_ship.Decks.Keys.Max(location => location.X) + 1;
							int max_y = hited_ship.Decks.Keys.Max(location => location.Y) + 1;

							for (int y = min_y; y <= max_y; y++)
							{
								if (y == -1 || y == 10)
								{
									continue;
								}
								for (int x = min_x; x <= max_x; x++)
								{
									if (x == -1 || x == 10)
									{
										continue;
									}
									Point p = new Point(x, y);
									if (_player_attacks.All(attack => attack.Location != p))
									{
										_player_attacks.Add(new AttackedCell(p, true));
										Paint_Computer_Cell(p, false);
									}
								}
							}
						}
						// Продолжаем ход
						break;
					case FireResult.Win:
						{
							Paint_Computer_Cell(cell, true);
							int min_x = hited_ship.Decks.Keys.Min(location => location.X) - 1;
							int min_y = hited_ship.Decks.Keys.Min(location => location.Y) - 1;
							int max_x = hited_ship.Decks.Keys.Max(location => location.X) + 1;
							int max_y = hited_ship.Decks.Keys.Max(location => location.Y) + 1;

							for (int y = min_y; y <= max_y; y++)
							{
								if (y == -1 || y == 10)
								{
									continue;
								}
								for (int x = min_x; x <= max_x; x++)
								{
									if (x == -1 || x == 10)
									{
										continue;
									}
									Point p = new Point(x, y);
									if (_player_attacks.All(attack => attack.Location != p))
									{
										_player_attacks.Add(new AttackedCell(p, true));
										Paint_Computer_Cell(p, false);
									}
								}
							}
							Console.WriteLine("Вы победили! Наверное это круто.");
							Console.WriteLine("Нажмите любую клавишу чтобы продолжить...");
							Console.ReadKey();
						}
						// Игра окончена
						return true;
				}
			}
		}

		/// <summary>
		/// Метод реализующий выстрел по кораблям противника
		/// </summary>
		/// <param name="cell">Клетка по которой ведётся огонь</param>
		/// <param name="attacks">Список предыдущих выстрелов</param>
		/// <param name="ships">Список ВРАЖЕСКИХ кораблей</param>
		/// <returns>result - результат выстрела, hited_ship - корабль в который он попал, иначе null</returns>
		private (FireResult result, Ship hited_ship) Fire(
			Point cell, ref List<AttackedCell> attacks, ref List<Ship> ships
		)
        {
			// Если в эту клетку уже совершался выстрел
			if (attacks.Any(attack => attack.Location == cell))
            {
				// То выстрел нельзя совершить!
				return (FireResult.Wrong, null);
            }
			// Иначе ищем корабль у которого хотябы одна палуба в точке выстрела
			Ship hited_ship = ships.FirstOrDefault(ship => ship.Decks.ContainsKey(cell));
			// Если корабль найден
			if (hited_ship is not null)
			{
				// То разрушаем палубу в которую попали
				hited_ship.Decks[cell] = false;
				// Фиксируем выстрел как успешный
				attacks.Add(new AttackedCell(cell, false));
				// И если это была последняя целая палуба корабля
				if (hited_ship.Decks.Values.All(deck => deck == false))
				{
					// И если это последний целый корабль
					if (ships.All(ship => ship.Decks.Values.All(deck => deck == false)))
					{
						// То победа
						return (FireResult.Win, hited_ship);
					}
					// Иначе корабль уничтожен
					return (FireResult.Destroy, hited_ship);
				}
				// Иначе просто попадание
				return (FireResult.Hit, hited_ship);
			}
			// Иначе фиксируем выстрел как промах
			attacks.Add(new AttackedCell(cell, true));
			return (FireResult.Miss, null);
		}

		private void Paint_Computer_Cell(Point cell, bool hit)
        {
			// Запоминаем где был курсор до операции
			(int x, int y) = Console.GetCursorPosition();

			// Делаем операцию по закрашиванию
			Console.SetCursorPosition((cell.X + 2) * 2, cell.Y + 2);
			if (hit)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.Write("██"); // Впишите сюда символ попадания
				Console.ResetColor();
			}
            else
			{
				Console.Write("<>"); // Впишите сюда символ промаха
			}

			// Возвращаем курсор в изначальное положение
			Console.SetCursorPosition(x, y);
		}

		/// <summary>
		/// Выполняет ввод координаты в диапозоне от 1 до 10 (MAP_SIZE)
		/// </summary>
		/// <param name="title">Сообщение выводимое пользователю</param>
		/// <returns>Число введёное пользователем, уменьшеное на 1 (от 0 до 9)</returns>
		private int Player_Input(string title)
        {
			// Запоминаем номер строки консоли
			int y = Console.CursorTop;
            while (true)
			{
				// Выводим сообщение пользователю
				Console.Write(title);
				if (
					// Считываем и проверяем что введено число
					int.TryParse(Console.ReadLine(), out int value) &&
					// И это число от 1 до 10
					value >= 1 && value <= MAP_SIZE
				)
				{
					return value - 1; // Возвращаем результат, но уже вычитая единицу
				}
				// Иначе стираем предыдущий ввод и повторяем попытку
				Console.SetCursorPosition(0, y);
				Console.Write(new string(' ', Console.BufferWidth) + "\r");
			}
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
			// Отрисовываем карту
			Draw_Map();
			// Отрисовываем корабли
			Draw_Ships();
		}

		/// <summary>
		/// Отрисовывает игровые поля в консоль
		/// </summary>
		private static void Draw_Map()
		{
			// Распологаем курсор в начало
			Console.SetCursorPosition(0, 0);

			// Игровое поле будет высотой (MAP_SIZE * 2) строк + 3 границы в одну строку
			for (int y = 0; y < FIELD_ROWS; y++)
			{
				// Если номер строки равен 0 или MAP_SIZE + 3
				if (y == 0 || y == MAP_SIZE + 3)
				{
					// Пишем пишем разметку X
					Console.Write("    ");
					for (int i = 1; i <= 10; i++)
					{
						Console.Write(i + " ");
					}
					Console.WriteLine();
					continue;
				}
				// Иначе если это начало строки на поле Компьютера
				else if (y >= 1 + 1 && y <= MAP_SIZE + 1)
				{
					Console.Write($"{y - 1,2}");
				}
				// Иначе если это начало строки на поле компьютера
				else if (y >= MAP_SIZE + 3 + 2 && y <= MAP_SIZE * 2 + 2 + 2)
				{
					Console.Write($"{y - MAP_SIZE - 3 - 1, 2}");
				}
                else
                {
					Console.Write("  ");
                }
				// И шириной в MAP_SIZE + 2 грацицы в 1 клетку
				for (int x = 0; x < MAP_SIZE + 2; x++)
				{   // Начнём с поля Компьютера
					if (
						y == 1 || y == MAP_SIZE + 2 ||						// Верхняя и Нижняя границы Компьютера
						y == MAP_SIZE + 4 || y == MAP_SIZE * 2 + 3 + 2 ||	// Верхняя и Нижняя границы Пользователя
						x == 0 || x == MAP_SIZE + 1							// Левая и Правая границы	
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
			Console.ForegroundColor = ConsoleColor.DarkCyan; // Установим цвет
			// Переберём все корабли Пользователя
			foreach (Ship ship in _player_ships)
			{
				foreach (Point deck in ship.Decks.Keys)
				{
					Console.SetCursorPosition((deck.X + 2) * 2, deck.Y + 3 + 2 + MAP_SIZE);
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
