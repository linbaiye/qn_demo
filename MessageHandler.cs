using System;
using DotNetty.Transport.Channels;

namespace testMove;

public class MessageHandler(Action<object> handler) : SimpleChannelInboundHandler<object>
{
    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
    {
        handler.Invoke(msg);
    }
}