using Godot;
using System;

public partial class TilemapTest : Node2D
{
    public TileMap Tilemap => GetNode<TileMap>("TileMap");

    public override void _Ready()
    {
        Tilemap.Clear();

        var maxSize = 4;

        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                Tilemap.SetCell(0, new Vector2I(i, j), 0, new Vector2I(0, 0), 0);
            }
        }

        var point = new Vector2I(0, maxSize - 1);

        for (int i = 0; i < maxSize; i++)
        {
            Tilemap.SetCell(0, new Vector2I(Math.Abs(point.X - i), point.Y), 3, new Vector2I(0, 0), 0);
            Tilemap.SetCell(0, new Vector2I(point.X, Math.Abs(point.Y - i)), 3, new Vector2I(0, 0), 0);
        }


        var peerpoint = new Vector2I(maxSize - 1 - point.X, maxSize - 1 - point.Y);
        Tilemap.SetCell(0, peerpoint, 1, new Vector2I(0, 0), 0);

        for (int i = 0; i < maxSize - 3; i++)
        {
            var next = new Vector2I(Math.Abs(peerpoint.X - i), peerpoint.Y);
            if (Tilemap.GetCellSourceId(0, next) == 0)
            {
                Tilemap.SetCell(0, next, 1, new Vector2I(0, 0), 0);
            }
        }

        for (int i = 0; i < maxSize - 2; i++)
        {
            var next = new Vector2I(peerpoint.X, Math.Abs(peerpoint.Y - i));
            if (Tilemap.GetCellSourceId(0, next) == 0)
            {
                Tilemap.SetCell(0, next, 1, new Vector2I(0, 0), 0);
            }
        }

        for (int i = 0; i < maxSize; i++)
        {
            var next = new Vector2I(i, maxSize - 1);
            if (Tilemap.GetCellSourceId(0, next) == 0)
            {
                Tilemap.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }

            next = new Vector2I(maxSize - 1, i);
            if (Tilemap.GetCellSourceId(0, next) == 0)
            {
                Tilemap.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }

            next = new Vector2I(i, 0);
            if (Tilemap.GetCellSourceId(0, next) == 0)
            {
                Tilemap.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }

            next = new Vector2I(0, i);
            if (Tilemap.GetCellSourceId(0, next) == 0)
            {
                Tilemap.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }
        }
    }
}
