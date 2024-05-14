using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TileMapTerrain : TileMap
{
    private Random random;

    internal void GenerateMap(TileMap baseMap, Vector2I bpoint)
    {
        random = new Random();

        FullFillByBase(baseMap);

        BuildLandEdge(bpoint);
        BuildMountions();
        BuildHills();

        var indexs = GetUsedCells(0);
        GD.Print($"total cell count{indexs.Count()}, water cell count {indexs.Count(x => GetCellSourceId(0, x) == 3)}");
    }

    private void BuildHills()
    {
        ChangeMountionEdge2Hill();
    }

    private void ChangeMountionEdge2Hill()
    {
        var maxValue = GetUsedCells(0).Select(index => index.X).Max();

        var edgeIndex2Factor = GetUsedCellsById(0, 2)
             .Where(neighbor => neighbor.X != 0 && neighbor.Y != 0 && neighbor.X != maxValue && neighbor.Y != maxValue)
            .Where(index =>
        {
            var neighborDict = this.GetNeighborCells_4(index);
            return neighborDict.Values.Any(x => GetCellSourceId(0, x) != 2);
        }).ToDictionary(x => x, _ => 1);

        var eraserCount = 0;
        var gCount = GetUsedCellsById(0, 2).Count();
        int turn = 1;
        while (eraserCount * 100 / gCount <50)
        {
            var eraseIndexs = new HashSet<Vector2I>();
            foreach (var index in edgeIndex2Factor.Keys)
            {
                var factor = edgeIndex2Factor[index];

                if (random.Next(0, 1000) <= 800 / factor)
                {
                    eraseIndexs.Add(index);
                }
            }

            foreach (var index in eraseIndexs)
            {
                edgeIndex2Factor.Remove(index);
                SetCell(0, index, 4, Vector2I.Zero, 0);
                eraserCount++;
            }

            foreach (var index in eraseIndexs)
            {
                var neighbors = this.GetNeighborCells_4(index).Values.Where(x => GetCellSourceId(0, x) == 2);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.X == 0 || neighbor.Y == 0)
                    {
                        continue;
                    }
                    if (neighbor.X == maxValue || neighbor.Y == maxValue)
                    {
                        continue;
                    }

                    edgeIndex2Factor.TryAdd(neighbor, 1);
                }
            }

            turn++;
        }
    }

    private void BuildMountions()
    {
        GenerateGPointSeed();

        FlushMountionEdge();
    }

    private void FlushMountionEdge()
    {
        var maxValue = GetUsedCells(0).Select(index => index.X).Max();

        var edgeIndex2Factor = GetUsedCellsById(0, 2)
            .Where(neighbor => neighbor.X != 0 && neighbor.Y != 0 && neighbor.X != maxValue && neighbor.Y != maxValue)
            .Where(index =>
        {
            var neighborDict = this.GetNeighborCells_4(index);
            return neighborDict.Values.Any(neighbor =>  GetCellSourceId(0, neighbor) != 2);
        }).ToDictionary(x => x, _ => 1);

        var eraserCount = 0;
        var gCount = GetUsedCellsById(0, 2).Count();
        int turn = 1;
        while (eraserCount * 100 / gCount < 20)
        {
            var eraseIndexs = new HashSet<Vector2I>();
            foreach (var index in edgeIndex2Factor.Keys)
            {
                var factor = edgeIndex2Factor[index];

                if (random.Next(0, 1000) <= 300 / factor)
                {
                    eraseIndexs.Add(index);
                }
            }

            foreach (var index in eraseIndexs)
            {
                edgeIndex2Factor.Remove(index);
                SetCell(0, index, 0, Vector2I.Zero, 0);
                eraserCount++;
            }

            foreach (var key in edgeIndex2Factor.Keys)
            {
                edgeIndex2Factor[key] *= 3;
            }

            foreach (var index in eraseIndexs)
            {
                var neighbors = this.GetNeighborCells_4(index).Values.Where(x => GetCellSourceId(0, x) == 2);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.X == 0 || neighbor.Y == 0)
                    {
                        continue;
                    }
                    if (neighbor.X == maxValue || neighbor.Y == maxValue)
                    {
                        continue;
                    }

                    edgeIndex2Factor.TryAdd(neighbor, 1);
                }
            }

            turn++;
        }
    }

    private void GenerateGPointSeed()
    {
        var bCellIndexs = GetUsedCellsById(0, 2);

        var maxValue = bCellIndexs.Select(index => index.X).Max();

        var selected = new HashSet<Vector2I>();
        while (selected.Count < bCellIndexs.Count * 0.15)
        {
            var index = bCellIndexs[random.Next(0, bCellIndexs.Count)];
            if(index.X == 0 || index.X == maxValue)
            {
                continue;
            }
            if (index.Y == 0 || index.Y == maxValue)
            {
                continue;
            }

            selected.Add(index);
        }

        foreach (var index in selected)
        {
            SetCell(0, index, 0, Vector2I.Zero, 0);
        }
    }

    private void BuildLandEdge(Vector2I bpoint)
    {
        var egdeIndexs = FlushLandEdge();

        var removedIndexs = RemoveSmallLandBlock();

        FixLandEdge(egdeIndexs.Except(removedIndexs), removedIndexs, bpoint * GetUsedCells(0).Select(x => x.X).Max());
    }

    private void FixLandEdge(IEnumerable<Vector2I> edgeIndexs, IEnumerable<Vector2I> removedIndexs, Vector2I bpoint)
    {
        var orderList = edgeIndexs.OrderBy(x => (bpoint - x).LengthSquared()).ToList();
        foreach (var item in orderList)
        {
            var perfers = this.GetNeighborCells_4(item).Values.Where(x => GetCellSourceId(0, x) == 3).ToArray();
            if (perfers.Length == 0)
            {
                throw new Exception();
            }
        }

        for (int i = 0; i < removedIndexs.Count()/2; i++)
        {
            var index = random.Next(0, orderList.Count() - 1) / random.Next(1, 10);

            var currIndex = orderList.ElementAt(index);

            var perfers = this.GetNeighborCells_4(currIndex).Values.Where(x => GetCellSourceId(0, x) == 3).ToArray();
            if (perfers.Length == 0)
            {
                throw new Exception();
            }

            var perfer = perfers[index % perfers.Length];
            SetCell(0, perfer, GetCellSourceId(0, currIndex), new Vector2I(0, 0), 0);

            orderList.Add(perfer);

            var ulives = this.GetNeighborCells_4(perfer).Values.Append(perfer)
              .Where(x => this.GetNeighborCells_4(x).Values.All(x => GetCellSourceId(0, x) != 3))
              .ToArray();

            foreach (var ulive in ulives)
            {
                orderList.Remove(ulive);
            }

            orderList = orderList.OrderBy(x => (bpoint - x).LengthSquared()).ToList();

        }
    }

    private IEnumerable<Vector2I> FlushLandEdge()
    {
        var edgeIndex2Factor = GetUsedCells(0).Where(index =>
        {
            int id = GetCellSourceId(0, index);
            if (id == 3)
            {
                return false;
            }

            var neighborDict = this.GetNeighborCells_4(index);
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

                if (random.Next(0, 1000) <= 300 / factor)
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
                edgeIndex2Factor[key] *= 3;
            }

            foreach (var index in eraseIndexs)
            {
                var neighbors = this.GetNeighborCells_4(index).Values.Where(x => GetCellSourceId(0, x) != -1 && GetCellSourceId(0, x) != 3);
                foreach (var neighbor in neighbors)
                {
                    edgeIndex2Factor.TryAdd(neighbor, 1);
                }
            }

            turn++;
        }

        return edgeIndex2Factor.Keys;
    }

    private void FullFillByBase(TileMap baseMap)
    {
        int n = 512 / 128;
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
        }
    }

    private IEnumerable<Vector2I> RemoveSmallLandBlock()
    {
        var groups = new List<(HashSet<Vector2I> live, HashSet<Vector2I> ulive)>();

        var landCells = new Stack<Vector2I>(GetUsedCells(0).Where(x => GetCellSourceId(0, x) != 3));
        while (landCells.Count > 0)
        {
            var currIndex = landCells.Pop();

            var inGroups = groups.Where(group => group.live.Any(item => this.GetNeighborCells_4(item).Values.Contains(currIndex)))
                .OrderByDescending(x => x.live.Count + x.ulive.Count)
                .ToArray();

            if (inGroups.Length == 0)
            {
                groups.Add((new HashSet<Vector2I>() { currIndex }, new HashSet<Vector2I>()));
            }
            else
            {
                inGroups[0].live.Add(currIndex);

                var ulivesNew = this.GetNeighborCells_4(currIndex).Values.Where(x => inGroups[0].live.Contains(x))
                    .Append(currIndex)
                    .Where(index => !this.GetNeighborCells_4(index).Values.Except(inGroups[0].live).Any())
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

        var needRemoveIndexs = groups.SelectMany(x => x.ulive.Concat(x.live));
        foreach (var index in needRemoveIndexs)
        {
            SetCell(0, index, 3, Vector2I.Zero, 0);
        }

        return needRemoveIndexs;
    }

    //private Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells(Vector2I index)
    //{
    //    var directs = new[] { TileSet.CellNeighbor.TopSide, TileSet.CellNeighbor.LeftSide, TileSet.CellNeighbor.BottomSide, TileSet.CellNeighbor.RightSide };
    //    return directs.ToDictionary(x => x, x => GetNeighborCell(index, x));
    //}
}

public static class TileMapExtension
{
    public static Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells_8(this TileMap tilemap, Vector2I index)
    {
        //var dict = Enum.GetValues<TileSet.CellNeighbor>().ToDictionary(x => x, x => tilemap.GetNeighborCell(Vector2I.Zero, x));
        var directs = new[] {
            TileSet.CellNeighbor.TopSide,
            TileSet.CellNeighbor.TopLeftCorner,
            TileSet.CellNeighbor.LeftSide,
            TileSet.CellNeighbor.BottomLeftCorner,
            TileSet.CellNeighbor.BottomSide,
            TileSet.CellNeighbor.BottomRightCorner,
            TileSet.CellNeighbor.RightSide,
            TileSet.CellNeighbor.TopRightCorner,};
        return directs.ToDictionary(x => x, x => tilemap.GetNeighborCell(index, x));
    }

    public static Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells_4(this TileMap tilemap, Vector2I index)
    {
        //var dict = Enum.GetValues<TileSet.CellNeighbor>().ToDictionary(x => x, x => tilemap.GetNeighborCell(Vector2I.Zero, x));
        var directs = new[] {
            TileSet.CellNeighbor.TopSide,
            TileSet.CellNeighbor.LeftSide,
            TileSet.CellNeighbor.BottomSide,
            TileSet.CellNeighbor.RightSide,
        };
        return directs.ToDictionary(x => x, x => tilemap.GetNeighborCell(index, x));
    }

}