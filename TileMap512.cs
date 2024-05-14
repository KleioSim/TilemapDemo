using Godot;
using System;
using System.Linq;

public partial class TileMap512 : TileMap
{
    private Random random;

    internal void GenerateMap(TileMap baseMap)
    {
        random = new Random();
        FullFillByBase(baseMap);
    }

    private void FullFillByBase(TileMap baseMap)
    {
        int n = 1024 / 512;
        foreach (var index in baseMap.GetUsedCells(0))
        {
            int id = baseMap.GetCellSourceId(0, index);
            if (id != 0)
            {
                for (int x = index.X * n; x < (index.X + 1) * n; x++)
                {
                    for (int y = index.Y * n; y < (index.Y + 1) * n; y++)
                    {
                        SetCell(0, new Vector2I(x, y), SwitchTerrain(id), new Vector2I(0, 0), 0);
                    }
                }
                continue;
            }

            var isNearbyPurple = baseMap.GetNeighborCells_8(index).Values.Any(x => baseMap.GetCellSourceId(0, x) == 2);
            for (int x = index.X * n; x < (index.X + 1) * n; x++)
            {
                for (int y = index.Y * n; y < (index.Y + 1) * n; y++)
                {
                    var percent = isNearbyPurple ? 60 : 30;
                    SetCell(0, new Vector2I(x, y), random.Next(0, 100) < percent ? 2 : 0, new Vector2I(0, 0), 0);
                }
            }

        }

        var newLandCells = GetUsedCellsById(0, 3).Where(cell => this.GetNeighborCells_8(cell).Values.Any(neighbor => GetCellSourceId(0, neighbor) is not 3 and not -1)).ToArray();
        foreach (var index in newLandCells)
        {
            SetCell(0, index, 0, new Vector2I(0, 0), 0);
        }
    }

    private int SwitchTerrain(int id)
    {
        switch (id)
        {
            //case 2:
            //    {
            //        var ret = id;
            //        var randomValuw = random.Next(0, 100);
            //        if (randomValuw * 100 / 100 <= 10)
            //        {
            //            ret = 0;
            //        }
            //        return ret;
            //    }
            case 0:
                {
                    var ret = id;
                    var randomValuw = random.Next(0, 100);
                    if (randomValuw * 100 / 100 <= 25)
                    {
                        ret = 2;
                    }
                    return ret;
                }
            default:
                return id;
        }
    }
}
