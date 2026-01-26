using System.Collections.Generic;
using UnityEngine;

public class BuildPiece : MonoBehaviour
{ 
    public List<Transform> sockets;
   public SocketCompatibility GetSocketAtPosition(Vector3 position)
   {
       foreach (Transform socketTransform in sockets)
       {
           if (Vector3.Distance(socketTransform.position, position) < 0.1f)
           {
               return socketTransform.GetComponent<SocketCompatibility>();
           }
       }
       return null;
   }
   
   public List<SocketCompatibility> GetAllSockets()
   {
       List<SocketCompatibility> socketList = new List<SocketCompatibility>();
       foreach (Transform socketTransform in sockets)
       {
           SocketCompatibility socket = socketTransform.GetComponent<SocketCompatibility>();
           if (socket != null)
           {
               socketList.Add(socket);
           }
       }
       return socketList;
   }
}