using System.Collections.Generic;
using UnityEngine;

public class SocketCompatibility : MonoBehaviour
{
    public enum SocketType
    {
        Top,
        Bottom,
        Left,
        Right,
        Floor,
        RampTop,
        RampBottom
    }

    public SocketType socketType;
    public List<SocketType> compatibleWith = new List<SocketType>();


    public bool IsCompatible(SocketCompatibility otherSocket)
    {
        return compatibleWith.Contains(otherSocket.socketType);
    }
}
    