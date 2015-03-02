using UnityEngine;
using System.Collections;

//add to empty game object and attach in front (offset) of Blitz

public class GrenadeManager : MonoBehaviour {
	
	public int currentAmmo;
	public int maxAmmo;
	public float recharge;
	public float timer = 0;
	public GameObject prefab; //insert grenade prefab here
	public Vector3 offset; //unused
	public float magnitude; //throwing velocity
	
	void Start () {
		
	}
	//need to change input to controller
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
		{
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
			if (timer > recharge){
				currentAmmo ++;
				timer = 0;
			}
			timer += Time.deltaTime;
			
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
