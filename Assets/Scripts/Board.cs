using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpaceTeam
{
    public class Board : IEnumerable<(Shape shape, Vector2Int position)>
    {
        private bool[] board;
        private List<(Shape shape, Vector2Int position)> items;

        public bool Binary { get; private set; }
        public int Width { get; private set; }

        public Board(double difficulty = 0.5f)
        {
            Binary = UnityEngine.Random.value < difficulty;
            Width = Binary ? 4 : 3;

            //binary = true; width = 4;
            //binary = false; width = 3;

            board = new bool[Width * Width];

            items = new List<(Shape shape, Vector2Int position)>();

            bool large, high, big;
            for (int y = 0, x; y < Width; ++y)
            {
                for (x = 0; x < Width; ++x)
                {
                    if (IsEmpty(x, y))
                    {
                        large = IsEmpty(x + 1, y);
                        high = IsEmpty(x, y + 1);

                        big = Binary && large && high && IsEmpty(x + 1, y + 1);

                        // Difficulty = 0 => 1
                        // Difficulty = 0.5 => 0.25
                        // Difficulty = 1 => 0
                        double proba = difficulty * difficulty - 2 * difficulty + 1;

                        big = big && UnityEngine.Random.value < proba;
                        large = large && UnityEngine.Random.value < proba;
                        high = high && UnityEngine.Random.value < proba;

                        if (big)
                            AddItem(Shape.Big, new Vector2Int(x, y));
                        else if (large)
                            AddItem(Shape.Horizontal, new Vector2Int(x, y));
                        else if (high)
                            AddItem(Shape.Vertical, new Vector2Int(x, y));
                        else
                            AddItem(Shape.Small, new Vector2Int(x, y));
                    }
                }
            }
        }



        private bool IsEmpty(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Width)
                return false;
            else
                return !board[y * Width + x];
        }

        private void AddItem(Shape shape, Vector2Int position)
        {
            for (int y = 0, x; y < position.y; ++y)
                for (x = 0; x < position.x; ++x)
                    board[(position.y + y) * Width + position.x + x] = true;

            items.Add((shape: shape, position: position));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<(Shape shape, Vector2Int position)> GetEnumerator()
        {
            foreach (var item in items)
                yield return item;
        }
    }
}
