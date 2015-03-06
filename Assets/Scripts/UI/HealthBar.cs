using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public GameObject gameManager;
	private GameManager gameManagerStats;
	private int playerID = -1;
	private Image healthbar;
	private bool firstFill;

	void Start() {
		firstFill = true;
		gameManagerStats = gameManager.GetComponent<GameManager> ();
		//Set up health bar
		healthbar = GetComponent<Image> ();
		RectTransform healthRect = GetComponent<RectTransform> ();
		healthRect.sizeDelta = new Vector2 (Screen.width*0.3f, Screen.height*0.04f);
		healthRect.anchoredPosition = new Vector2 (Screen.width*0.08f, -Screen.height*0.05f);
	}
	public void StartHealthBar(int playerNumber) {
		this.playerID = playerNumber;
	}
	// Update is called once per frame
	void Update () {
		if (playerID>=0 && firstFill) {
			Debug.Log("filling up hb");
			healthbar.fillAmount = Mathf.MoveTowards (healthbar.fillAmount, 1.0f, Time.deltaTime * 0.8f);
			if (healthbar.fillAmount == 1f) firstFill=false;
		}
		if (playerID>=0) {
			float currentHealth = (float)gameManagerStats.playerCurrentHealth[playerID]/gameManagerStats.playerTotalHealth[playerID];
			if (currentHealth != healthbar.fillAmount) {
				healthbar.fillAmount = Mathf.MoveTowards (healthbar.fillAmount, currentHealth, Time.deltaTime * 0.5f);
			}
		}
	}
}
