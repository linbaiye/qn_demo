using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;

namespace testMove;

public class MessageEncoder : MessageToByteEncoder<IMessage>
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    protected override void Encode(IChannelHandlerContext context, IMessage message, IByteBuffer output)
    {
        Logger.Debug("Write message.");
        output.WriteBytes(message.ToBytes());
    }
}