using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class TilemapTest : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapBase BaseMap => GetNode<TileMapBase>("CanvasLayer2/TileMapBase");
    public TileMapTerrain TerrainMap => GetNode<TileMapTerrain>("CanvasLayer2/TileMapTerrain");

    private Random random = new Random();

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            //BaseMap.GenerateMap();

            var cellIndexs = BaseMap.GetUsedCells(0);

            int n = 64;
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

            var edgeIndexs = TerrainMap.GetUsedCells(0).Where(index =>
            {
                int id = TerrainMap.GetCellSourceId(0, index);
                if (id == 3)
                {
                    return false;
                }

                var neighborDict = GetNeighborCells(index);
                return neighborDict.Values.Any(x => TerrainMap.GetCellSourceId(0, x) == 3);
            });

            foreach (var index in edgeIndexs)
            {
                if (random.Next(0, 100) >= 50)
                {
                    TerrainMap.EraseCell(0, index);
                }
            }
        };
    }

    private Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells(Vector2I index)
    {
        var directs = new[] { TileSet.CellNeighbor.TopSide, TileSet.CellNeighbor.LeftSide, TileSet.CellNeighbor.BottomSide, TileSet.CellNeighbor.RightSide };
        return directs.ToDictionary(x => x, x => TerrainMap.GetNeighborCell(index, x));
    }
}
