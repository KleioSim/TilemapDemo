using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class TilemapTest : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapBase BaseMap => GetNode<TileMapBase>("CanvasLayer2/TileMap2048");
    public TileMapTerrain TerrainMap => GetNode<TileMapTerrain>("CanvasLayer2/TileMap128");

    private Random random = new Random();

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            BaseMap.GenerateMap();

            var cellIndexs = BaseMap.GetUsedCells(0);

            int n = 2048 / 128;
            foreach (var index in cellIndexs)
            {
                int id = BaseMap.GetCellSourceId(0, index);
                for (int x = index.X * n; x < (index.X + 1) * n; x++)
                {
                    for (int y = index.Y * n; y < (index.Y + 1) * n; y++)
                    {
                        TerrainMap.SetCell(0, new Vector2I(x, y), id, new Vector2I(0, 0), 0);
                    }
                }
            }

            var edgeIndex2Factor = TerrainMap.GetUsedCells(0).Where(index =>
            {
                int id = TerrainMap.GetCellSourceId(0, index);
                if (id == 3)
                {
                    return false;
                }

                var neighborDict = GetNeighborCells(index);
                return neighborDict.Values.Any(x => TerrainMap.GetCellSourceId(0, x) == 3);
            }).ToDictionary(x => x, _ => 1);

            var eraserCount = 0;
            var gCount = TerrainMap.GetUsedCellsById(0, 0).Count();
            int turn = 1;
            while (eraserCount * 100 / gCount < 50)
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
                    TerrainMap.EraseCell(0, index);
                    eraserCount++;
                }

                foreach (var key in edgeIndex2Factor.Keys)
                {
                    edgeIndex2Factor[key] *=2;
                }

                foreach (var index in eraseIndexs)
                {
                    var neighbors = GetNeighborCells(index).Values.Where(x => TerrainMap.GetCellSourceId(0, x) != -1 && TerrainMap.GetCellSourceId(0, x) != 3);
                    foreach (var neighbor in neighbors)
                    {
                        edgeIndex2Factor.TryAdd(neighbor, 1);
                    }
                }

                turn++;
            }

        };
    }

    private Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells(Vector2I index)
    {
        var directs = new[] { TileSet.CellNeighbor.TopSide, TileSet.CellNeighbor.LeftSide, TileSet.CellNeighbor.BottomSide, TileSet.CellNeighbor.RightSide };
        return directs.ToDictionary(x => x, x => TerrainMap.GetNeighborCell(index, x));
    }
}
