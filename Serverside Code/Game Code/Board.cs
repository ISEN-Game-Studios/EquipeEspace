using System.Collections.Generic;
using System;

namespace SpaceTeam
{
    public class Board
    {
        private bool[] board;
        private List<Item> items;

        private bool binary;
        private int width;

        public Board(double difficulty = 0.5f)
        {
            binary = new Random().NextDouble() < difficulty;
            width = binary ? 4 : 3;

            board = new bool[width * width];

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

                        // Difficulty = 0 => 1
                        // Difficulty = 0.5 => 0.25
                        // Difficulty = 1 => 0
                        double proba = difficulty * difficulty - 2 * difficulty + 1;

                        big = big && new Random().NextDouble() < proba;
                        large = large && new Random().NextDouble() < proba;
                        high = high && new Random().NextDouble() < proba;

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
        }

        private bool IsEmpty(int x, int y)
        {
            if (x >= width || y >= width)
                return false;
            else
                return !board[y * width + x];
        }

        private void AddItem(Item item)
        {
            for (int y = 0, x; y < item.height; ++y)
                for (x = 0; x < item.width; ++x)
                    board[(item.y + y) * width + (item.x + x)] = true;

            items.Add(item);
        }

        public Item[] GetItems()
        {
            return items.ToArray();
        }
    }
}
