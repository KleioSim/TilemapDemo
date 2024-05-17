using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TileMapBase : TileMap
{
    private Random random;


    public Vector2I GenerateMap()
    {
        random = new Random();

        this.Clear();

        var randomValue = random.Next();

        var maxSize = 4;
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                this.SetCell(0, new Vector2I(i, j), 0, new Vector2I(0, 0), 0);
            }
        }

        var bpoints = new[] { new Vector2I(0, 0), new Vector2I(0, maxSize - 1), new Vector2I(maxSize - 1, 0), new Vector2I(maxSize - 1, maxSize - 1) };
        var index = randomValue % 4;

        var bpoint = bpoints[index];
        for (int i = 0; i < maxSize; i++)
        {
            this.SetCell(0, new Vector2I(Math.Abs(bpoint.X - i), bpoint.Y), 3, new Vector2I(0, 0), 0);
            this.SetCell(0, new Vector2I(bpoint.X, Math.Abs(bpoint.Y - i)), 3, new Vector2I(0, 0), 0);
        }


        var peerpoint = new Vector2I(maxSize - 1 - bpoint.X, maxSize - 1 - bpoint.Y);
        this.SetCell(0, peerpoint, 1, new Vector2I(0, 0), 0);

        var yCount = maxSize - 2;
        var param1 = randomValue % 2;

        for (int i = 1; i < param1 + 1; i++)
        {
            var next = new Vector2I(Math.Abs(peerpoint.X - i), peerpoint.Y);
            if (this.GetCellSourceId(0, next) == 0)
            {
                this.SetCell(0, next, 1, new Vector2I(0, 0), 0);
            }
        }
        for (int i = 1; i < yCount - param1; i++)
        {
            var next = new Vector2I(peerpoint.X, Math.Abs(peerpoint.Y - i));
            if (this.GetCellSourceId(0, next) == 0)
            {
                this.SetCell(0, next, 1, new Vector2I(0, 0), 0);
            }
        }

        for (int i = 0; i < maxSize; i++)
        {
            var next = new Vector2I(i, maxSize - 1);
            if (this.GetCellSourceId(0, next) == 0)
            {
                this.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }

            next = new Vector2I(maxSize - 1, i);
            if (this.GetCellSourceId(0, next) == 0)
            {
                this.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }

            next = new Vector2I(i, 0);
            if (this.GetCellSourceId(0, next) == 0)
            {
                this.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }

            next = new Vector2I(0, i);
            if (this.GetCellSourceId(0, next) == 0)
            {
                this.SetCell(0, next, 2, new Vector2I(0, 0), 0);
            }
        }

        return bpoint / (maxSize - 1);
    }
}