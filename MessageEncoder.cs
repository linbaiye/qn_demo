using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace testMove;

public class MessageEncoder : MessageToByteEncoder<IMessage>
{
    protected override void Encode(IChannelHandlerContext context, IMessage message, IByteBuffer output)
    {
        output.WriteBytes(message.ToBytes());
    }
}