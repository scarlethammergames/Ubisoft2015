using UnityEngine;
using System.Collections;
using GamepadInput;

public class Vacuum : MonoBehaviour {

	public float magnitude;
	private float trigger;
	private DeftPlayerController controller;
	public bool affectEverything;
	private ParticleEmitter effect;

	void Start () {
		controller = GameObject.FindGameObjectWithTag("Player").GetComponent<DeftPlayerController>();
		effect = GetComponentInChildren<ParticleEmitter> ();
		effect.emit = false;
	}

	/*
	void Update () {
		trigger = controller.gamepadState.RightTrigger;
		if (trigger > 0) 
		{
		}
	}*/

	void OnTriggerStay(Collider other)
	{
		trigger = controller.gamepadState.RightTrigger;
		if (trigger > 0) {
			effect.emit = true;
			if (other.rigidbody.useGravity) {
				PhysicsStatus status = other.gameObject.GetComponent<PhysicsStatus> ();
				Vector3 direction = Vector3.Normalize (other.transform.position - this.transform.parent.position);
				if (status == null || status.pullable || affectEverything) {
					other.rigidbody.AddForce (direction * (-1 * magnitude), ForceMode.Impulse);
				}
			}
		} else { 
			effect.emit = false;
		}

	}

}
