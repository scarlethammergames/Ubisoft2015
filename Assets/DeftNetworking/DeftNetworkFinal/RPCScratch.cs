using UnityEngine;
using System.Collections;

public class RPCScratch : MonoBehaviour {

  public int i;

  [RPC]
  void setI(int newI)
  {
    this.i = newI;
    Debug.Log("Set i to " + this.i);
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
    if (i % 25 == 0)
    {
      if(Network.isServer)
      {
        this.networkView.RPC("setI", RPCMode.Others, this.i);
      }
    }
    i++;
	}
}
