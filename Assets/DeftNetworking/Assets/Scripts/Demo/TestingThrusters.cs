using UnityEngine;
using System.Collections;

public class TestingThrusters : MonoBehaviour {

  public float thrusterPower;

  [RPC]
  public void Activate()
  {
    Debug.Log("Activating thrusters.");

    this.gameObject.rigidbody.AddForce(this.gameObject.transform.forward * thrusterPower);

  }

}
