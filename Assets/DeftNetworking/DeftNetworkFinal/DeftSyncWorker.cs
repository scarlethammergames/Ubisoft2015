using UnityEngine;
using System.Collections;

public enum SyncWorkerType { snap, simplesmoothing, firstorder, secondorder, adaptivehigherorder };

public class DeftSyncWorker : MonoBehaviour
{

    public DeftBodyState goalState;
    public DeftBodyState lastCheckedState;
    public bool moveToState;
    public float duration = 1.0f;
    public bool debug = true;
    public SyncWorkerType workerType = SyncWorkerType.firstorder;
    float durationTmp;

    public void StartSync()
    {
        this.durationTmp = duration;
    }

    void FixedUpdate()
    {
        if (durationTmp > 0)
        {
            switch (this.workerType)
            {
                case SyncWorkerType.firstorder:
                    FirstOrderSync(this.goalState);
                    break;
            }
        }
    }

    void FirstOrderSync(DeftBodyState state)
    {
        float lerpSpeed = this.duration / Time.fixedDeltaTime;
        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, state.position, lerpSpeed);
        this.gameObject.rigidbody.velocity = Vector3.Lerp(this.gameObject.rigidbody.velocity, state.velocity, lerpSpeed);
        this.gameObject.rigidbody.rotation = Quaternion.Slerp(this.gameObject.rigidbody.rotation, state.rotation, lerpSpeed);
        this.gameObject.rigidbody.angularVelocity = Vector3.Lerp(this.gameObject.rigidbody.angularVelocity, state.angularVelocity, lerpSpeed);
        if (debug)
        {
            Debug.Log(durationTmp + ": moving " + state.id + " to " + state.position.ToString());
        }
    }
}
