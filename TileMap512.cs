using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public partial class TileMap512 : TileMap
{
    private Random random;

    internal void GenerateMap(TileMap baseMap, Vector2I bpoint)
    {


        random = new Random();

        FullFillByBase(baseMap);
        bpoint = bpoint * GetUsedCells(0).Select(x => x.X).Max();

        AddGPoint2LandEdge(bpoint);
        //RandomConvert2P();
        //RandomConvert2Y();
    }

    private void AddGPoint2LandEdge(Vector2I bpoint)
    {
        var pEdge = GetUsedCellsById(0, 3)
            .Where(index => index.X != bpoint.X && index.Y != bpoint.Y).ToArray();

        foreach (var index in pEdge)
        {
            SetCell(0, index, 0, new Vector2I(0, 0), 0);
        }

        var landEgde = GetUsedCellsById(0, 0).Where(x => this.GetNeighbor4CellsById(x, 3).Any()).ToArray();
        foreach (var index in landEgde.OrderBy(_ => random.Next()).Take(landEgde.Length / 2))
        {
            SetCell(0, index, 3, new Vector2I(0, 0), 0);
        }
    }

    private void RandomConvert2Y()
    {
        var dict = new Dictionary<Vector2I, int>();
        for (var i = 0; i < 3; i++)
        {
            var items = GetUsedCells(0).Where(x => GetCellSourceId(0, x) != 1 && this.GetNeighbor4CellsById(x, 1).Any()).ToArray();
            foreach (var item in items)
            {
                dict.Add(item, GetCellSourceId(0, item));
                SetCell(0, item, 1, new Vector2I(0, 0), 0);
            }
        }

        for (var i = 0; i < 3; i++)
        {
            var yEgde = GetUsedCellsById(0, 1).Where(index => dict.ContainsKey(index) && this.GetNeighborCells_4(index).Values.Any(neighbor => this.GetCellSourceId(0, neighbor) != 1)).ToArray();
            yEgde = yEgde.OrderBy(_ => random.Next()).Take(yEgde.Length * 7 / 10).ToArray();
            foreach (var index in yEgde)
            {
                SetCell(0, index, dict[index], new Vector2I(0, 0), 0);
            }
        }
    }

    private void RandomConvert2P()
    {
        var dict = GetUsedCellsById(0, 0).ToDictionary(k => k, v => this.GetNeighbor4CellsById(v, 2).Count());
        foreach (var pair in dict)
        {
            if (random.Next(0, 100) < (pair.Value + 1) * 25)
            {
                SetCell(0, pair.Key, 2, new Vector2I(0, 0), 0);
            }
        }
    }

    private void FullFillByBase(TileMap baseMap)
    {
        int n = 2048 / 512;

        foreach (var index in baseMap.GetUsedCells(0))
        {
            int id = baseMap.GetCellSourceId(0, index);
            for (int x = index.X * n; x < (index.X + 1) * n; x++)
            {
                for (int y = index.Y * n; y < (index.Y + 1) * n; y++)
                {
                    SetCell(0, new Vector2I(x, y), id, new Vector2I(0, 0), 0);
                }
            }
            continue;
        }

        //foreach (var index in baseMap.GetUsedCells(0))
        //{
        //    int id = baseMap.GetCellSourceId(0, index);
        //    if (id != 0)
        //    {
        //        for (int x = index.X * n; x < (index.X + 1) * n; x++)
        //        {
        //            for (int y = index.Y * n; y < (index.Y + 1) * n; y++)
        //            {
        //                SetCell(0, new Vector2I(x, y), SwitchTerrain(id), new Vector2I(0, 0), 0);
        //            }
        //        }
        //        continue;
        //    }

        //    var isNearbyPurple = baseMap.GetNeighborCells_8(index).Values.Any(x => baseMap.GetCellSourceId(0, x) == 2);
        //    for (int x = index.X * n; x < (index.X + 1) * n; x++)
        //    {
        //        for (int y = index.Y * n; y < (index.Y + 1) * n; y++)
        //        {
        //            var percent = isNearbyPurple ? 70 : 35;
        //            SetCell(0, new Vector2I(x, y), random.Next(0, 100) < percent ? 2 : 0, new Vector2I(0, 0), 0);
        //        }
        //    }

        //}

        //var newLandCells = GetUsedCellsById(0, 3).Where(cell => this.GetNeighborCells_8(cell).Values.Any(neighbor => GetCellSourceId(0, neighbor) is not 3 and not -1)).ToArray();
        //foreach (var index in newLandCells)
        //{
        //    SetCell(0, index, 0, new Vector2I(0, 0), 0);
        //}
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
