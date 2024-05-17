using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class TilemapTest2 : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapBase BaseMap => GetNode<TileMapBase>("CanvasLayer2/TileMap2048");
    public TileMapFractal Terrain1024 => GetNode<TileMapFractal>("CanvasLayer2/TileMap1024");
    public TileMapFractal Terrain512 => GetNode<TileMapFractal>("CanvasLayer2/TileMap512");
    public TileMapFractal Terrain128 => GetNode<TileMapFractal>("CanvasLayer2/TileMap128");
    public TileMapTerrain TerrainMap => GetNode<TileMapTerrain>("CanvasLayer2/TileMapTerrain");
    public TileMapAuto AutoMap => GetNode<TileMapAuto>("CanvasLayer2/TilemapAuto");
    public TileMapFractal TileMap2048 => GetNode<TileMapFractal>("CanvasLayer2/TileMap2048");
    public Slider slider => GetNode<Slider>("CanvasLayer/Slider");

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            var startPoint = new Vector2I(0, 0);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    TileMap2048.SetCell(0, new Vector2I(i, j), 3, new Vector2I(0, 0), 0);
                }
            }

            TileMap2048.Generate(startPoint, 0.1);
            Terrain1024.Generate(startPoint, 0.1, TileMap2048);
            Terrain512.Generate(startPoint, 0.1, Terrain1024);
            Terrain128.Generate(startPoint, 0.1, Terrain512);

            //AutoMap.Generate2();
            //AutoMap.Show(0.5);

            //var landcount = TerrainMap.GetUsedCells(0).Where(x => TerrainMap.GetCellSourceId(0, x) != 3).Count();

            //GD.Print($"{landcount}, {landcount / 20}, {landcount / 100}");


        };

        //slider.ValueChanged += (percent) =>
        //{
        //    AutoMap.Show(percent);
        //};
    }
}
