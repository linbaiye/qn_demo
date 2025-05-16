using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;

namespace testMove;

public class MessageDecoder() : LengthFieldBasedFrameDecoder(short.MaxValue, 0, 4, 0, 4)
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();

    private object Decode(IByteBuffer frame)
    {
        MessageType messageType = (MessageType)frame.ReadInt();
        if (messageType == MessageType.Show)
            return ShowMessage.Create(frame.ReadInt(), frame.ReadInt(), frame.ReadInt());
        if (messageType == MessageType.LoginOk)
            return LoginOkMessage.Create(frame.ReadInt(), frame.ReadInt(), frame.ReadInt());
        if (messageType == MessageType.Move)
            return new MoveMessage(frame.ReadInt(), frame.ReadInt(), frame.ReadInt(), frame.ReadInt());
        if (messageType == MessageType.Remove)
            return new RemoveMessage(frame.ReadInt());
        return null;
    }
    
    protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
    {
        IByteBuffer frame = (IByteBuffer)base.Decode(context, input);
        if (frame == null)
        {
            return null;
        }
        var msg = Decode(frame);
        Logger.Debug("Received message {}.", msg);
        frame.Release();
        return msg;
    }
}