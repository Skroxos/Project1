using UnityEngine;

public class SocketCompatibility : MonoBehaviour
{
    public enum SocketType
    {
        Top,
        Bottom,
        Left,
        Right
    }
   
    public SocketType socketType;
    public SocketType compatibleWith;
    public bool isOccupied;
   
    public bool IsCompatible(SocketCompatibility otherSocket)
    {
        return otherSocket.socketType == compatibleWith && !otherSocket.isOccupied;
    }
    
    public void OccupySocket()
    {
        isOccupied = true;
    }
}