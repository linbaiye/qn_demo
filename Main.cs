using Godot;
using System;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	private Sprite2D _sprite;
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse mouse && mouse.IsPressed())
			_sprite.Position += new Vector2(10, 10);
	}
}
