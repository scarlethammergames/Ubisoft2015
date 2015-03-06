using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public enum PlayerCharacter {  Syphen, Blitz };

public class GameManager : MonoBehaviour
{
  public int[] playerCurrentHealth;
  public int[] playerTotalHealth;
  public int numbersOfDepots;
  public int[] depotCapacity;
  public int[] depotCurrentStock;
  public int[] depotResourceValue;
  public GameObject[] depots;
  public int killerAttackDamage;
  public int feederAttackDamage;
  public GameObject[] depotUI_objects;
  public GameObject eventSystemObject;
  public GameObject gameOverFirstSelected;
  public GameObject gameOverWindow;
  public GameObject winText;
  public GameObject loseText;

  private int depotsFull;
  private EventSystem eventSystem;

  void Start()
  {
    gameOverWindow.SetActive(false);
    winText.SetActive(false);
    loseText.SetActive(false);
    eventSystem = eventSystemObject.GetComponent<EventSystem>();
    depotsFull = 0;
    //Disable all depots except the first one
    foreach (GameObject d in depots)
    {
      d.SetActive(false);
    }
    foreach (GameObject d in depotUI_objects)
    {
      d.SetActive(false);
    }
    depots[0].SetActive(true);
    depotUI_objects[0].SetActive(true);
  }
  public void StartHealthBar(string name) {
		GameObject hb = GameObject.Find ("HealthBar");
		GameObject hs = GameObject.Find ("HealthStats");
		if (name.Contains ("Syphen")) {
			hb.GetComponent<HealthBar> ().StartHealthBar (0);
			hs.GetComponent<HealthStats> ().StartStats (0);
		} else {
			hb.GetComponent<HealthBar> ().StartHealthBar (1);
			hs.GetComponent<HealthStats> ().StartStats (1);
		}
  }
  public void StartGrenadeBar() {
		GameObject.Find ("GrenadeBar").GetComponent<GrenadeBar>().StartGrenadeUI();
  }

  public void decreaseHealth(string targetPlayerName, string attackerName)
  {
    int damage;
    if (attackerName.Contains("Feeder"))
    {
      damage = feederAttackDamage;
    }
    else
    {
      damage = killerAttackDamage;
    }
    if (targetPlayerName.Contains("Syphen"))
    {
      playerCurrentHealth[0] -= damage;
    }
    else
    {
      playerCurrentHealth[1] -= damage;
    }
    if (playerCurrentHealth[0] <= 0 || playerCurrentHealth[1] <= 0)
    {
      gameOver();
    }
  }

  //Increases resource count of a certain depot
  public void increaseResourceCount(int depotNumber)
  {
    depotCurrentStock[depotNumber] += depotResourceValue[depotNumber];
  }
  public void depotFull()
  {
    depotsFull++;
    if (depotsFull == numbersOfDepots)
    {
      lastDepotFull();
    }
    else
    {
      //Activate next depot
      Debug.Log("A Depot is FULL");
      depots[depotsFull].SetActive(true);
      depotUI_objects[depotsFull].SetActive(true);
    }
  }
  public void lastDepotFull()
  {
    Debug.Log("YOU WIN.");
    gameOverWindow.SetActive(true);
    winText.SetActive(true);
    eventSystem.SetSelectedGameObject(gameOverFirstSelected);
  }
  public void gameOver()
  {
    Debug.Log("YOU DIED.");
    gameOverWindow.SetActive(true);
    loseText.SetActive(true);
    eventSystem.SetSelectedGameObject(gameOverFirstSelected);
  }


}
