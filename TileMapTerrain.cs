using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TileMapTerrain : TileMap
{
    internal void GenerateMap(TileMap baseMap)
    {
        var random = new Random();

        var cellIndexs = baseMap.GetUsedCells(0);

        FullFillByBase(baseMap, cellIndexs);

        FlushLandEdge(random);

        RemoveSmallLandBlock();
    }

    private void FlushLandEdge(Random random)
    {
        var edgeIndex2Factor = GetUsedCells(0).Where(index =>
        {
            int id = GetCellSourceId(0, index);
            if (id == 3)
            {
                return false;
            }

            var neighborDict = GetNeighborCells(index);
            return neighborDict.Values.Any(x => GetCellSourceId(0, x) == 3);
        }).ToDictionary(x => x, _ => 1);

        var eraserCount = 0;
        var gCount = GetUsedCells(0).Where(x => GetCellSourceId(0, x) != 3).Count();
        int turn = 1;
        while (eraserCount * 100 / gCount < 30)
        {
            var eraseIndexs = new HashSet<Vector2I>();
            foreach (var index in edgeIndex2Factor.Keys)
            {
                var factor = edgeIndex2Factor[index];

                if (random.Next(0, 100) <= 50 / factor)
                {
                    eraseIndexs.Add(index);
                }
            }

            foreach (var index in eraseIndexs)
            {
                edgeIndex2Factor.Remove(index);
                SetCell(0, index, 3, Vector2I.Zero, 0);
                eraserCount++;
            }

            foreach (var key in edgeIndex2Factor.Keys)
            {
                edgeIndex2Factor[key] *= 2;
            }

            foreach (var index in eraseIndexs)
            {
                var neighbors = GetNeighborCells(index).Values.Where(x => GetCellSourceId(0, x) != -1 && GetCellSourceId(0, x) != 3);
                foreach (var neighbor in neighbors)
                {
                    edgeIndex2Factor.TryAdd(neighbor, 1);
                }
            }

            turn++;
        }
    }

    private void FullFillByBase(TileMap baseMap, Godot.Collections.Array<Vector2I> cellIndexs)
    {
        int n = 2048 / 128;
        foreach (var index in cellIndexs)
        {
            int id = baseMap.GetCellSourceId(0, index);
            for (int x = index.X * n; x < (index.X + 1) * n; x++)
            {
                for (int y = index.Y * n; y < (index.Y + 1) * n; y++)
                {
                    SetCell(0, new Vector2I(x, y), id, new Vector2I(0, 0), 0);
                }
            }
        }
    }

    private void RemoveSmallLandBlock()
    {
        var groups = new List<(HashSet<Vector2I> live, HashSet<Vector2I> ulive)>();

        var landCells = new Stack<Vector2I>(GetUsedCells(0).Where(x => GetCellSourceId(0, x) != 3));
        while (landCells.Count > 0)
        {
            var currIndex = landCells.Pop();

            var inGroups = groups.Where(group => group.live.Any(item => GetNeighborCells(item).Values.Contains(currIndex)))
                .OrderByDescending(x => x.live.Count + x.ulive.Count)
                .ToArray();

            if (inGroups.Length == 0)
            {
                groups.Add((new HashSet<Vector2I>() { currIndex }, new HashSet<Vector2I>()));
            }
            else
            {
                inGroups[0].live.Add(currIndex);

                var ulivesNew = GetNeighborCells(currIndex).Values.Where(x => inGroups[0].live.Contains(x))
                    .Append(currIndex)
                    .Where(index => !GetNeighborCells(index).Values.Except(inGroups[0].live).Any())
                    .ToArray();

                inGroups[0].live.ExceptWith(ulivesNew);
                inGroups[0].ulive.UnionWith(ulivesNew);

                for (int i = 1; i < inGroups.Length; i++)
                {
                    inGroups[0].live.UnionWith(inGroups[i].live);
                    inGroups[0].ulive.UnionWith(inGroups[i].ulive);
                    groups.Remove(inGroups[i]);
                }

            }
        }

        var maxItemCountGroup = groups.MaxBy(x => x.ulive.Count + x.live.Count);
        groups.Remove(maxItemCountGroup);

        foreach (var index in groups.SelectMany(x => x.ulive.Concat(x.live)))
        {
            SetCell(0, index, 3, Vector2I.Zero, 0);
        }
    }

    private Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells(Vector2I index)
    {
        var directs = new[] { TileSet.CellNeighbor.TopSide, TileSet.CellNeighbor.LeftSide, TileSet.CellNeighbor.BottomSide, TileSet.CellNeighbor.RightSide };
        return directs.ToDictionary(x => x, x => GetNeighborCell(index, x));
    }
}