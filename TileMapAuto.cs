using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TileMapAuto : TileMap
{
    private Dictionary<Vector2I, int> dict = new Dictionary<Vector2I, int>();
    private Random random = new Random();

    public void Generate()
    {
        dict.Clear();

        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                dict.Add(new Vector2I(i, j), random.Next(0, 1000));
                SetCell(0, new Vector2I(i, j), 3, new Vector2I(0, 0), 0);
            }
        }
    }

    internal void Show(double percent)
    {
        var array = dict.OrderBy(x => x.Value).ToArray();

        for (int i = 0; i < array.Length * percent; i++)
        {
            SetCell(0, array[i].Key, 3, new Vector2I(0, 0), 0);
        }

        for (int i = (int)(array.Length * percent); i < array.Length; i++)
        {
            EraseCell(0, array[i].Key);
        }
    }

    public void Generate2()
    {
        dict.Clear();

        var start = new Vector2I(0, 0);
        dict.Add(start, 0);

        for (int i = 1; i < 64; i++)
        {
            var tempDict = new Dictionary<Vector2I, int>();

            for (int j = 0; j < i + 1; j++)
            {
                var index1 = new Vector2I(i, j);

                var maxValue = this.GetNeighborCells_8(index1).Values
                    .Where(n => dict.ContainsKey(n))
                    .Select(n => dict[n])
                    .Max();

                tempDict.Add(index1, maxValue + random.Next(1, 1000));

                if (i == j)
                {
                    continue;
                }

                var index2 = new Vector2I(j, i);
                maxValue = this.GetNeighborCells_8(index2).Values
                    .Where(n => dict.ContainsKey(n))
                    .Select(n => dict[n])
                    .Max();

                tempDict.Add(index2, maxValue + random.Next(1, 1000));
            }

            foreach (var pair in tempDict)
            {
                dict.Add(pair.Key, pair.Value);
            }
        }
    }
}
