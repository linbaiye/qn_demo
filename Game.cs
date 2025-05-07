using System.Collections.Generic;
using System.Threading;
using Godot;
using NLog;
using testMove;
using testMove.SourceCode;

public partial class Game : Node
{
	// Called when the node enters the scene tree for the first time.

	private Connection _connection;
	
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();

	public override void _Ready()
	{
		SetupNetwork();
	}

	private async void SetupNetwork()
	{
		_connection = await Connection.ConnectTo("127.0.0.1", 9999);
		_connection.WriteAndFlush(new LoginMessage());
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleMessages();
	}

	private void HandleMessages()
	{
		if (_connection == null)
			return;
		List<IMessage> messages = _connection.DrainMessages();
		foreach (var message in messages)
		{
			if (message is ShowMessage showMessage)
			{
				Logger.Debug("Received message {}.", showMessage);
				var player = Player.FromMessage(showMessage);
				AddChild(player);
			}
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
	}
}
