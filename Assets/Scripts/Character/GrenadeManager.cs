﻿using UnityEngine;
using System.Collections;
using GamepadInput;
using System.Runtime.InteropServices;
using System;

//add to empty game object and attach in front (offset) of Blitz

public struct GrenadeThrowParameters
{
  public Vector3 forward;
  public Vector3 position;
  public Quaternion rotation;
}

public class GrenadeManager : MonoBehaviour
{
  public int currentAmmo;
  public int maxAmmo;
  public float recharge;
  public float rechargeTimer = 0;
  public GameObject prefab; //insert grenade prefab here
  public Vector3 offset; //unused
  public float magnitude; //throwing velocity

  private bool trigger;
  public float triggerCooldown = 1; //adjustable fire rate
  private float cooldown = 0; // temp var for cooldown after trigger
  private DeftPlayerController controller;

  private bool goingToFire = false;
	
  void Start()
  {
    controller = transform.parent.gameObject.GetComponent<DeftPlayerController>();
  }

  void Update()
  {
    if (networkView.isMine)
    {
      trigger = controller.gamepadState.RightShoulder || (controller.gamepadState.RightTrigger > 0.20f); 
      cooldown -= Time.deltaTime; // reduce cooldown timer

		if (trigger){
			controller.state = PlayerState.aiming; //let blitz aim (even if no ammo)
			goingToFire = true; // bool to indicate intent to fire
		} else if (!trigger){
			controller.state = PlayerState.idle; // on trigger release, revert to idle
			if (cooldown <= 0.0 && currentAmmo > 0 && goingToFire) // conditions for firing
			{
				Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
				forward = forward.normalized;
				networkView.RPC("throwGrenade", RPCMode.Others, forward, transform.position, transform.rotation);
				throwGrenade(forward, transform.position, transform.rotation); 
				currentAmmo--; // reduce ammo
				cooldown = triggerCooldown; // reset cooldown
			}
			goingToFire = false; // always remove intent to fire after releasing trigger
		}
    }
  }

  // method to recharge grenades when under max ammo
  void FixedUpdate()
  {
    if (currentAmmo < maxAmmo)
    {
      if (rechargeTimer > recharge)
      {
        currentAmmo++;
        rechargeTimer = 0;
      }
      rechargeTimer += Time.deltaTime;

    }
  }

  [RPC]
  void throwGrenade(Vector3 forward, Vector3 position, Quaternion rotation)
  {
    GameObject clone;
    clone = Instantiate(prefab, position + forward, rotation) as GameObject;
    clone.rigidbody.velocity = forward * magnitude;
  }
}
