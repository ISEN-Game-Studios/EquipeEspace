using System.Collections.Generic;
using System;

namespace SpaceTeam
{
	public class Board
	{
		private int[] board;
		private List<Item> items;

		private bool binary;
		private int width;

		public Board()
		{
			binary = new Random().NextDouble() < 0.5;
			width = binary ? 4 : 3;

			board = new int[width * width];

			items = new List<Item>();

			bool large, high, big;
			for (int y = 0, x; y < width; ++y)
			{
				for (x = 0; x < width; ++x)
				{
					if (IsEmpty(x, y))
					{
						large = IsEmpty(x + 1, y);
						high = IsEmpty(x, y + 1);

						big = binary && large && high && IsEmpty(x + 1, y + 1);

						big = big && new Random().Next(4) == 0;
						large = large && new Random().Next(4) == 0;
						high = high && new Random().Next(4) == 0;

						/*if (big)
							AddItem(new Item(x, y, 2, 2));
						else if (large)
							AddItem(new Item(x, y, 2, 1));
						else if (high)
							AddItem(new Item(x, y, 1, 2));
						else
							AddItem(new Item(x, y, 1, 1));*/
					}
				}
			}

			foreach (Item item in items)
			{
				Console.WriteLine(item.x + " " + item.y + " / " + item.width + " " + item.height);
			}
		}

		private bool IsEmpty(int x, int y)
		{
			return board[y * width + x] == 0;
		}

		private void AddItem(Item item)
		{
			items.Add(item);
		}

		private void GenerateBoard()
		{

		}
	}
}
