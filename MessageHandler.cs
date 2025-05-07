using System;
using DotNetty.Transport.Channels;

namespace testMove;

public class MessageHandler(Action<IMessage> handler) : SimpleChannelInboundHandler<IMessage>
{
    protected override void ChannelRead0(IChannelHandlerContext ctx, IMessage msg)
    {
        handler.Invoke(msg);
    }
}