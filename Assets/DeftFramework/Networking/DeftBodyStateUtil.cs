﻿using UnityEngine;
using System.Collections;
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


public class DeftBodyStateUtil
{

  public static float SquaredPositionalDifference(DeftBodyState a, DeftBodyState b)
  {
    return Vector3.SqrMagnitude(a.position - b.position);
  }


  public static float SquaredPositionalDifference(GameObject a, DeftBodyState b)
  {
    return Vector3.SqrMagnitude(a.transform.position - b.position);
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

}
