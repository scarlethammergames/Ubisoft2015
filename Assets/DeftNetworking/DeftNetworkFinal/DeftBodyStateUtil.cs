using UnityEngine;
using System.Collections;

public class DeftBodyStateUtil
{

    public static float Difference(DeftBodyState a, DeftBodyState b)
    {
        return Vector3.Distance(a.position, b.position);
    }


    public static float Difference(GameObject a, DeftBodyState b)
    {
        return Vector3.Distance(a.transform.position, b.position);
    }

    public static DeftBodyState BuildState(GameObject obj)
    {
        DeftBodyState state = new DeftBodyState();
        state.id = obj.networkView.viewID;
        state.position = obj.transform.position;
        state.rotation = obj.transform.rotation;
        state.velocity = obj.rigidbody.velocity;
        state.angularVelocity = obj.rigidbody.angularVelocity;
        state.timestamp = Time.time;
        return state;
    }

}
