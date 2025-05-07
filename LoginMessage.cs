using System.Text.Json;

namespace testMove;

public class LoginMessage : AbstractMessage
{
    public override MessageType MessageType => MessageType.Login;
    
    public override byte[] ToBytes()
    {
        return GetBytes((int)MessageType);
    }
}