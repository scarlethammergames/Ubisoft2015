﻿using UnityEngine;
using System.Collections;

public class StatusUpdate : MonoBehaviour {

	TextMesh status;
	Camera myCamera;

	public string normalText;
	public string chasingText;


	// Use this for initialization
	void Start ()
	{
	
		myCamera = Camera.main;

		status = gameObject.GetComponent<TextMesh> ();

		updateText (false);

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
		
		}
		else
		{

			status.color = Color.yellow;
			status.text = normalText;

		}

	}

}
