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

	private Player _player;
	
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
				_player = Player.FromMessage(showMessage);
				AddChild(_player);
			}
			else if (message is LoginOkMessage loginOkMessage)
			{
				var player = Character.FromMessage(loginOkMessage, _connection);
				AddChild(player);
			}
			else if (message is MoveMessage moveMessage)
			{
				_player?.Move(moveMessage);
			}
			else if (message is RemoveMessage removeMessage)
			{
				if (_player != null && _player.Id == removeMessage.Id)
					RemoveChild(_player);
			}
		}
	}
}
