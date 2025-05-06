using System;
using Godot;

namespace testMove;

public static class MessageFactory
{

    private static byte[] GetBytes(int n)
    {
        byte[] bytes = BitConverter.GetBytes(n);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }
    

    public static byte[] Login()
    {
        return new MoveMessage().ToBytes();
    }
    
}