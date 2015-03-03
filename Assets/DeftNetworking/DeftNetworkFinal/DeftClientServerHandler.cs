using UnityEngine;
using System.Collections;

public class DeftClientServerHandler : MonoBehaviour
{

  public enum DeftNetworkRole { JOINING, CLIENT, HOST, HOSTING, UNASSIGNED };
  public DeftNetworkRole role = DeftNetworkRole.UNASSIGNED;
  public string gameName = "4f4w46f5ewf";
  public string roomName = "rfg486gsgr4";

  void OnGUI()
  {
    if (!Network.isClient && !Network.isServer)
    {
      if (GUI.Button(new Rect(10, 10, 100, 20), "Host"))
      {
        this.role = DeftNetworkRole.HOSTING;
      }
      if (GUI.Button(new Rect(10, 30, 100, 30), "Join"))
      {
        this.role = DeftNetworkRole.JOINING;
      }
    }
  }

  void FixedUpdate()
  {
    switch (this.role)
    {
      case DeftNetworkRole.HOSTING:

        break;
    }
  }
}
