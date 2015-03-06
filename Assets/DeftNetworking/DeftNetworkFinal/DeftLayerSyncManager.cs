using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public struct DeftBodyState
{
  public double timestamp;
  public NetworkViewID id;
  public Vector3 position;
  public Vector3 velocity;
  public Quaternion rotation;
  public Vector3 angularVelocity;
}

public class DeftLayerSyncManager : MonoBehaviour
{

  public Dictionary<NetworkViewID, GameObject> objectsInLayer;
  public Queue<DeftBodyState> syncQueue;
  public int layer;
  public float hardSyncThreshold = 5.0f;

  public bool debug;

  public static byte[] MarshallDeftBodyState(DeftBodyState state)
  {
    int size = Marshal.SizeOf(state);
    byte[] arr = new byte[size];
    IntPtr ptr = Marshal.AllocHGlobal(size);
    Marshal.StructureToPtr(state, ptr, true);
    Marshal.Copy(ptr, arr, 0, size);
    Marshal.FreeHGlobal(ptr);
    return arr;
  }

  public static DeftBodyState UnMarshalDeftBodyState(byte[] arr)
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
        this.objectsInLayer[obj.networkView.viewID] = obj;
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
    Debug.Log("Updating deft body state for " + state.id.ToString());
    this.objectsInLayer[state.id].GetComponent<DeftSyncWorker>().goalState = state;
    this.objectsInLayer[state.id].GetComponent<DeftSyncWorker>().StartSync();
  }

  void BuildSyncQueue()
  {
    foreach (KeyValuePair<NetworkViewID, GameObject> entry in this.objectsInLayer)
    {
      DeftBodyState lastChecked = entry.Value.GetComponent<DeftSyncWorker>().lastCheckedState;
      if (DeftBodyStateUtil.Difference(entry.Value, lastChecked) > 1.0f || Time.time - lastChecked.timestamp > this.hardSyncThreshold)
      {
        if (debug)
        {
          Debug.Log("Syncing with " + (Time.time - lastChecked.timestamp) + " seconds passed and distance difference of " + DeftBodyStateUtil.Difference(entry.Value, lastChecked));
        }
        entry.Value.GetComponent<DeftSyncWorker>().lastCheckedState = DeftBodyStateUtil.BuildState(entry.Value);
        this.syncQueue.Enqueue(lastChecked);
      }
      else
      {
        if (debug)
        {
          Debug.Log("No need to sync " + lastChecked.id.ToString());
        }
      }
    }
    if (debug)
    {
      Debug.Log("Sync queue rebuilt with " + this.syncQueue.Count + " objects ready to sync.");
    }
  }

  void Awake()
  {
    foreach (NetworkView netView in this.GetComponents<NetworkView>())
    {
      netView.observed = this;
    }
    this.objectsInLayer = new Dictionary<NetworkViewID, GameObject>();
    this.syncQueue = new Queue<DeftBodyState>();
    this.SetObjectsInLayer();
  }

  void FixedUpdate()
  {
    if (Network.isServer)
    {
      if (this.syncQueue.Count > 0)
      {
        DeftBodyState state = DeftBodyStateUtil.BuildState(this.objectsInLayer[this.syncQueue.Dequeue().id]);
        byte[] bytes = MarshallDeftBodyState(state);
        if (debug)
        {
          Debug.Log("Sending " + state.id.ToString());
        }
        this.networkView.RPC("UpdateDeftBodyState", RPCMode.Others, bytes);
      }
      else
      {
        BuildSyncQueue();
      }
    }
  }
}
