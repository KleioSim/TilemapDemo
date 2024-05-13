using Godot;
using System;
using System.Linq;

public partial class TileMap512 : TileMap
{
    internal void GenerateMap(TileMap baseMap)
    {
        FullFillByBase(baseMap);
    }

    private void FullFillByBase(TileMap baseMap)
    {
        int n = 1024 / 512;
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

        var newLandCells = GetUsedCellsById(0,3).Where(cell => this.GetNeighborCells(cell).Values.Any(neighbor=> GetCellSourceId(0,neighbor) is not 3 and not -1)).ToArray();
        foreach (var index in newLandCells)
        {
            SetCell(0, index, 0, new Vector2I(0, 0), 0);
        }
    }
}
