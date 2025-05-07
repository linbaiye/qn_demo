using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;

namespace testMove;

public class MessageDecoder() : LengthFieldBasedFrameDecoder(short.MaxValue, 0, 4, 0, 4)
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    
    protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
    {
        IByteBuffer frame = (IByteBuffer)base.Decode(context, input);
        if (frame == null)
        {
            return null;
        }
        MessageType messageType = (MessageType)frame.ReadInt();
        if (messageType == MessageType.Show)
        {
            Logger.Debug("Received message {}.", messageType);
            var message = ShowMessage.Create(frame.ReadInt(), frame.ReadInt(), frame.ReadInt());
            frame.Release();
            return message;
        }
        frame.Release();
        return null;
    }
}