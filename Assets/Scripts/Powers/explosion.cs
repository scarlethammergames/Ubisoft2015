﻿using UnityEngine;
using System.Collections;

public class explosion : MonoBehaviour {
	public float radius;
	public float power;
	public float lift;
	public float delay;
	public GameObject effect;
	
	void Start(){
		StartCoroutine ("MyMethod");
	}

	IEnumerator MyMethod() {
		yield return new WaitForSeconds(delay);

		Instantiate(effect, transform.position, transform.rotation);
		Vector3 explosionPos = transform.position;

		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			// can add cases for non physics objects later
			PhysicsStatus ps = (PhysicsStatus) hit.GetComponent<PhysicsStatus>(); // grab physics status of object
			if( ps && ps.pushable ){ 
				Shatterable shatterable = hit.gameObject.GetComponent<Shatterable>();
				if(shatterable){
					shatterable.switchToFractured(); // shatter shatterables
				} else if(hit.attachedRigidbody){
					hit.GetComponent<PhysicsStatus>().pullable = true; // switch objects to pullable
				}
			}
			if (hit && hit.rigidbody){
				//and then still apply explosion force to any rigidbodies
				hit.rigidbody.AddExplosionForce(power, explosionPos, radius, lift, ForceMode.Impulse); 
			}
		}
		Destroy (gameObject);
	}
}
