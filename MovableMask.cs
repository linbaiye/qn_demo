using Godot;
using System;

public partial class MovableMask : TileMapLayer
{
    public override void _Ready()
    {
        foreach (var cell in GetUsedCells())
        {
            GD.Print(cell.X + ":" + cell.Y);
        }
    }
}
