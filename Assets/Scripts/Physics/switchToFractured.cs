using UnityEngine;
using System.Collections;

public class switchToFractured : MonoBehaviour {
    public GameObject fractureSet;
    public float threshold;
    
    private bool hasCollided = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnCollisionEnter(Collision col) {
        if (!hasCollided && col.rigidbody) {
            float velocity = col.rigidbody.velocity.sqrMagnitude;
            if (velocity > threshold)
            {
				PhysicsStatus ps = this.GetComponent<PhysicsStatus>();
				if (ps && (ps.pullable || ps.pushable))
                {
                    this.gameObject.SetActive(false);
                    fractureSet.gameObject.SetActive(true);
                    hasCollided = true;
                }
            }
        }
    }
}
