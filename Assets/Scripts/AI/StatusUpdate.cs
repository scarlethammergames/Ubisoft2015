using UnityEngine;
using System.Collections;

public class StatusUpdate : MonoBehaviour {

	TextMesh status;
	string normalText;
	string chasingText;
	Camera myCamera;

	// Use this for initialization
	void Start ()
	{
	
		myCamera = Camera.main;

		status = gameObject.GetComponent<TextMesh> ();

		normalText = "?!SEARCHING?!?";
		chasingText = "!?GOTCHA?!";

		updateText (false);

		//transform.RotateAround(gameObject.transform.position, new Vector3(0,1,0), 180f

	}

	void Update()
	{

		gameObject.transform.LookAt (myCamera.transform.position);
		gameObject.transform.RotateAround (transform.position, transform.up, 180f);
	}

	public void updateText(bool playerClose)
	{

		if(playerClose)
		{
			status.color = Color.red;
			status.text = chasingText;
		
		}else{

			status.color = Color.yellow;
			status.text = normalText;

		}
	}

}
