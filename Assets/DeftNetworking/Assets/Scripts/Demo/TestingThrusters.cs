using UnityEngine;
using System.Collections;

public class TestingThrusters : MonoBehaviour {

  [RPC]
  public void Activate()
  {
    Debug.Log("Activating thrusters.");
  }

}
