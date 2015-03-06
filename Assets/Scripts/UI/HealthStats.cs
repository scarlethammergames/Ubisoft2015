using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthStats : MonoBehaviour {

	public GameObject gameManager;
	private GameManager gameManagerStats;
	private int playerID;
	private Text stats;
	private bool characterSelected;

	// Use this for initialization
	void Start () {
		gameManagerStats = gameManager.GetComponent<GameManager> ();
		characterSelected = false;
		stats = this.GetComponent<Text> ();
	}
	public void StartStats(int playerID) {
		this.playerID = playerID;
		characterSelected = true;
	}
	// Update is called once per frame
	void Update () {
		if (playerID>=0) {
			stats.text = gameManagerStats.playerCurrentHealth[playerID].ToString() + "/" + gameManagerStats.playerTotalHealth[playerID].ToString();
		}
	}
}
