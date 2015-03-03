using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public struct DeftBodyState
{
    public double timestamp;
    public int id;
    public Vector3 position;
    public Vector3 velocity;
    public Quaternion rotation;
    public Vector3 angularVelocity;
}

public class DeftLayerSyncManager : MonoBehaviour
{

    public Dictionary<int, GameObject> objectsInLayer;
    public Queue<DeftBodyState> syncQueue;
    public int layer;

    public bool debug;

    byte[] MarshallDeftBodyState(DeftBodyState state)
    {
        int size = Marshal.SizeOf(state);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(state, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    DeftBodyState UnMarshalDeftBodyState(byte[] arr)
    {
        DeftBodyState state = new DeftBodyState();
        int size = Marshal.SizeOf(state);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(arr, 0, ptr, size);
        state = (DeftBodyState)Marshal.PtrToStructure(ptr, state.GetType());
        Marshal.FreeHGlobal(ptr);
        return state;
    }

    [RPC]
    public void SetObjectsInLayer()
    {
        this.objectsInLayer.Clear();
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.layer == this.layer)
            {
                this.objectsInLayer[obj.GetInstanceID()] = obj;
            }
        }
        if (debug)
        {
            Debug.Log(this.objectsInLayer.Count + " objects tracked in layer " + this.layer);
        }
    }

    [RPC]
    public void UpdateDeftBodyState(byte[] bytes)
    {
        DeftBodyState state = UnMarshalDeftBodyState(bytes);
        FirstOrderSync(this.objectsInLayer[state.id], state);
    }

    void FirstOrderSync(GameObject obj, DeftBodyState state)
    {
        obj.transform.position = Vector3.Lerp(obj.transform.position, state.position, 0.5f);
        obj.rigidbody.velocity = Vector3.Lerp(obj.rigidbody.velocity, state.velocity, 0.5f);
        obj.rigidbody.rotation = Quaternion.Slerp(obj.rigidbody.rotation, state.rotation, 0.5f);
        obj.rigidbody.angularVelocity = Vector3.Lerp(obj.rigidbody.angularVelocity, state.angularVelocity, 0.5f);
    }

    void BuildSyncQueue()
    {
        foreach (KeyValuePair<int, GameObject> entry in this.objectsInLayer)
        {
            DeftBodyState state = new DeftBodyState();
            state.position = entry.Value.transform.position;
            state.velocity = entry.Value.rigidbody.velocity;
            state.rotation = entry.Value.rigidbody.rotation;
            state.angularVelocity = entry.Value.rigidbody.angularVelocity;
            state.id = entry.Key;
            this.syncQueue.Enqueue(state);
        }
    }

    void Awake()
    {
        foreach (NetworkView netView in this.GetComponents<NetworkView>())
        {
            netView.observed = this;
        }
    }

    void FixedUpdate()
    {
        if (this.syncQueue.Count > 0)
        {
            DeftBodyState state = this.syncQueue.Dequeue();
            state.timestamp = Time.time;
            this.networkView.RPC("UpdateDeftBodyState", RPCMode.Others, state);
        }
        else
        {
            BuildSyncQueue();
        }
    }
}
