using UnityEngine;
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
			if (hit && hit.rigidbody){
				hit.rigidbody.AddExplosionForce(power, explosionPos, radius, lift, ForceMode.Impulse); //if we hit any rigidbodies then add force based off our power, the position of the explosion object
			}
		}
		Destroy (gameObject);
	}
}
