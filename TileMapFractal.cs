using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class TileMapFractal : TileMap
{
    int turn = 1;
    private Random random = new Random();
    //private Dictionary<int, Dictionary<Vector2I, bool>> dict = new Dictionary<int, Dictionary<Vector2I, bool>>();

    public void Generate(Vector2I edgePoint, double percent, TileMap baseMap = null)
    {

        if (baseMap != null)
        {
            Clear();

            var factor = baseMap.TileSet.TileSize / TileSet.TileSize;
            foreach (var index in baseMap.GetUsedCells(0))
            {
                int id = baseMap.GetCellSourceId(0, index);
                for (int x = index.X * factor.X; x < (index.X + 1) * factor.X; x++)
                {
                    for (int y = index.Y * factor.Y; y < (index.Y + 1) * factor.Y; y++)
                    {
                        SetCell(0, new Vector2I(x, y), id, new Vector2I(0, 0), 0);
                    }
                }
                continue;
            }
        }

        edgePoint = edgePoint * GetUsedCells(0).Select(index => index.X).Max();
        var all = GetUsedCells(0).Count();

        int count = 0;
        while (count < all * percent)
        {
            var indexs = GetUsedCells(0)
                .Where(index => index.X != edgePoint.X && index.Y != edgePoint.Y)
                .Where(x => this.GetNeighbor4CellsById(x, -1).Any())
                .Where(x => !IsConnectNode(x)).ToArray();

            var index = indexs[random.Next(0, indexs.Length)];
            EraseCell(0, index);
            count++;
        }


    }

    private bool IsConnectNode(Vector2I index)
    {
        var neighbors = this.GetNeighborCells_8(index);

        if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.LeftSide]) != -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.RightSide]) != -1)
        {
            if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopSide]) == -1)
            {
                if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomRightCorner]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomSide]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomLeftCorner]) == -1)
                {
                    return true;
                }
            }

            if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomSide]) == -1)
            {
                if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopRightCorner]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopSide]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopLeftCorner]) == -1)
                {
                    return true;
                }
            }
        }

        if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomSide]) != -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopSide]) != -1)
        {
            if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.LeftSide]) == -1)
            {
                if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomRightCorner]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.RightSide]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopRightCorner]) == -1)
                {
                    return true;
                }
            }
            if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.RightSide]) == -1)
            {
                if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomLeftCorner]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.LeftSide]) == -1
                    || GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopLeftCorner]) == -1)
                {
                    return true;
                }
            }
        }

        //if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopSide]) != -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomSide]) != -1
        // && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.LeftSide]) == -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.RightSide]) == -1)
        //{
        //    return true;
        //}
        //if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopSide]) == -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomSide]) == -1
        // && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.LeftSide]) != -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.RightSide]) != -1)
        //{
        //    return true;
        //}
        if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopLeftCorner]) == -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomRightCorner]) == -1)
        {
            return true;
        }
        if (GetCellSourceId(0, neighbors[TileSet.CellNeighbor.TopRightCorner]) == -1 && GetCellSourceId(0, neighbors[TileSet.CellNeighbor.BottomLeftCorner]) == -1)
        {
            return true;
        }
        return false;
    }
}
