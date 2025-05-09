using System.Collections.Generic;
using Godot;
using NLog;
using testMove;
using testMove.SourceCode;

public partial class Game : Node
{
	// Called when the node enters the scene tree for the first time.

	private Connection _connection;
	
	private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();

	private Character? _character;
	
	private void MoveByMouse()
	{
		if (_character == null)
			return;
		var pos = _character.GetLocalMousePosition();
		var angle = Mathf.Snapped(pos.Angle(), Mathf.Pi / 4) / (Mathf.Pi / 4);
		int dir = Mathf.Wrap((int)angle, 0, 8);
		var direction = dir switch
		{
			0 => Direction.Right,
			1 => Direction.DownRight,
			2 => Direction.Down,
			3 => Direction.DownLeft,
			4 => Direction.Left,
			5 => Direction.UpLeft,
			6 => Direction.Up,
			7 => Direction.UpRight,
			_ => Direction.Right,
		};
		_character.Move(direction);
		;
	}

	public override void _Ready()
	{
		SetupNetwork();
	}

	private async void SetupNetwork()
	{
		_connection = await Connection.ConnectTo("127.0.0.1", 9999);
		_connection.WriteAndFlush(new LoginMessage());
	}
	
	public override void _Process(double delta)
	{
		HandleMessages();
	}

	private void HandleMessages()
	{
		if (_connection == null)
			return;
		List<object> messages = _connection.DrainMessages();
		foreach (var message in messages)
		{
			if (message is ShowMessage showMessage)
			{
				var player = Player.FromMessage(showMessage);
				AddChild(player);
			}
			else if (message is LoginOkMessage loginOkMessage)
			{
				
				var player = Character.FromMessage(loginOkMessage);
				_character = player;
				AddChild(player);
			}
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton button)
		{
			Logger.Debug("Index {}, Pressed {}..", button.ButtonIndex, button.Pressed);
			if (button.ButtonIndex == MouseButton.Right && button.Pressed)
				 MoveByMouse();
			else if (button.ButtonIndex == MouseButton.Right && !button.Pressed)
			{
				_character?.StopMove();
			}
		}
	}
}
