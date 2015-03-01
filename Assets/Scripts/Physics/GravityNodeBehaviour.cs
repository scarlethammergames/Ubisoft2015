using UnityEngine;
using System.Collections;

public enum GravityNodeType { Push, Pull, Lift }

public class GravityNodeBehaviour : MonoBehaviour
{

  public float magnitudeBirth;
  public float magnitudeDeath;
  public float magnitudeRatio;
  public float radiusBirth;
  public float radiusDeath;
  public float radiusRatio;
  public float duration;
  public GravityNodeType type;

  public bool timed;

  float currentRadius;
  float currentMagnitude;


  void Start()
  {

  }

  void FixedUpdate()
  {
    if (timed)
    {
      currentRadius = Mathf.Lerp(magnitudeBirth, magnitudeDeath, radiusRatio * Time.deltaTime);
      currentMagnitude = Mathf.Lerp(radiusBirth, radiusDeath, radiusRatio * Time.deltaTime);
      duration -= Time.deltaTime;
    }
  }

  void OnTriggerStay(Collider other)
  {
    if (other.rigidbody.useGravity)
    {
      if (other.attachedRigidbody)
      {
        PhysicsStatus status = other.gameObject.GetComponent<PhysicsStatus>();
        Vector3 direction = Vector3.Normalize(other.transform.position - this.transform.position);
        switch (type)
        {
          case GravityNodeType.Lift:
            if (status == null || status.liftable)
            {
              other.rigidbody.AddForce(direction * (currentMagnitude / Vector3.Magnitude(other.transform.position - this.transform.position)), ForceMode.Impulse);
            }
            break;
          case GravityNodeType.Pull:
            if (status == null || status.liftable)
            {
              other.rigidbody.AddForce(direction * (currentMagnitude / Vector3.Magnitude(other.transform.position - this.transform.position)), ForceMode.Impulse);
            }
            break;
          case GravityNodeType.Push:
            if (status == null || status.liftable)
            {
              other.rigidbody.AddForce(direction * (currentMagnitude / Vector3.Magnitude(other.transform.position - this.transform.position)), ForceMode.Impulse);
            }
            break;
        }
      }
    }
  }

}
