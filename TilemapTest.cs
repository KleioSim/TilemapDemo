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

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            BaseMap.GenerateMap();

            TerrainMap.GenerateMap(BaseMap);
        };
    }
}
