#nullable enable
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace testMove;

public class Connection(string ip, int port)
{
    private IChannel? _channel;

    private readonly List<IMessage> _messages = new();

    public async void Close()
    {
        if (_channel == null)
            return;
        await _channel.CloseAsync();
    }

    public void WriteAndFlush(IMessage message)
    {
        _channel?.WriteAndFlushAsync(message);
    }

    private void OnMessageArrived(IMessage message)
    {
        lock (_messages)
        {
            _messages.Add(message);
        }
    }
    
    public List<IMessage> DrainMessages()
    {
        List<IMessage> messages = new List<IMessage>();
        lock (_messages)
        {
            messages.AddRange(_messages);
            _messages.Clear();
        }
        return messages;
    }

    private async Task Init()
    {
        Bootstrap bootstrap = new Bootstrap();
        bootstrap.Group(new SingleThreadEventLoop()).Handler(
            new ActionChannelInitializer<ISocketChannel>(c => c.Pipeline.AddLast(
                new LengthFieldPrepender(4), 
                new MessageEncoder(),
                new MessageDecoder(),
                new MessageHandler(OnMessageArrived)
                ))).Channel<TcpSocketChannel>();
        _channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public static async Task<Connection> ConnectTo(string ip, int port)
    {
        var connection = new Connection(ip, port);
        await connection.Init();
        return connection;
    }
    
}