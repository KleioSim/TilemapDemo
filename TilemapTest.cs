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
    public TileMap512 Terrain512 => GetNode<TileMap512>("CanvasLayer2/TileMap512");
    public TileMap1024 Terrain1024 => GetNode<TileMap1024>("CanvasLayer2/TileMap1024");

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            var bpoint = BaseMap.GenerateMap();
            Terrain1024.GenerateMap(BaseMap);
            Terrain512.GenerateMap(Terrain1024);
            TerrainMap.GenerateMap(Terrain512, bpoint);
        };
    }
}
