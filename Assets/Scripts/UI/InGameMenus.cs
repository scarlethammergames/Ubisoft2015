using UnityEngine;
using System.Collections;

public class InGameMenus : MonoBehaviour {

	public string mainMenuSceneName;
	public string gameSceneName;

	public void MainMenu() {
		Application.LoadLevel (mainMenuSceneName);
	}

	public void RestartGame() {
		Application.LoadLevel (gameSceneName);
//		GameObject.Find ("GameManager").GetComponent<DeftNetwork> ().enabled = true;
	}
}
