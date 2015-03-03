﻿using UnityEngine;
using System.Collections;
using GamepadInput;

//add to empty game object and attach in front (offset) of Blitz

public class GrenadeManager : MonoBehaviour {
	
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


	void Start () {
		controller = GameObject.FindGameObjectWithTag("Player").GetComponent<DeftPlayerController>();
	}

	void Update () 
	{
		trigger = controller.gamepadState.RightShoulder;
		cooldown -= Time.deltaTime;

		if (trigger && cooldown<=0.0)
		{
			trigger = false;
			cooldown = triggerCooldown;
			if (currentAmmo > 0)
			{
				throwGrenade();
				currentAmmo --;
			}
		}
	}

	void FixedUpdate()
	{
		if (currentAmmo < maxAmmo) 
		{
			if (rechargeTimer > recharge){
				currentAmmo ++;
				rechargeTimer = 0;
			}
			rechargeTimer += Time.deltaTime;
			
		}
	}
	
	void throwGrenade()
	{
		GameObject clone;
		clone = Instantiate (prefab, transform.position + offset, transform.rotation) as GameObject;
		Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		forward = forward.normalized;
		clone.rigidbody.velocity = (new Vector3(forward.x * magnitude, 0, forward.z * magnitude));
	}
}
