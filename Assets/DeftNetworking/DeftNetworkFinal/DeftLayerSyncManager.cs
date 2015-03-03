using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeftLayerSyncManager : MonoBehaviour {

    public Dictionary<int, GameObject> objectsInLayer;

    [RPC]
    public void SetObjectsInLayer()
    {

    }

	void Start () {
	
	}
	
	void FixedUpdate () {
	
	}
}
