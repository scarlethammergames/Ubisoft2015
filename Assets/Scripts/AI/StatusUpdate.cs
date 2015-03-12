using UnityEngine;
using System.Collections;

public class StatusUpdate : MonoBehaviour {

	TextMesh status;
	string normalText;
	string chasingText;

	// Use this for initialization
	void Start () {
	
		status = gameObject.GetComponent<TextMesh> ();

		normalText = "?!SEARCHING?!?";
		chasingText = "!?GOTCHA?!";

		updateText (false);

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
