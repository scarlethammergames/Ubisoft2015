using UnityEngine;
using System.Collections;
using GamepadInput;
using System.Runtime.InteropServices;
using System;

//add to empty game object and attach in front (offset) of Blitz

public struct GrenadeThrowParameters
{
  public Vector3 forward;
  public Vector3 position;
  public Quaternion rotation;
}

public class GrenadeManager : MonoBehaviour
{

  public int currentAmmo;
  public int maxAmmo;
  public float recharge;
  public float rechargeTimer = 0;
  public GameObject prefab; //insert grenade prefab here
  public Vector3 offset; //unused
  public float magnitude; //throwing velocity

  private bool trigger;
  public float triggerCooldown = 1; //adjustable fire rate
  private float cooldown = 0; // temp var for cooldown after trigger
  private DeftPlayerController controller;

  public static byte[] MarshallGrenadeThrowParameters(GrenadeThrowParameters state)
  {
    int size = Marshal.SizeOf(state);
    byte[] arr = new byte[size];
    IntPtr ptr = Marshal.AllocHGlobal(size);
    Marshal.StructureToPtr(state, ptr, true);
    Marshal.Copy(ptr, arr, 0, size);
    Marshal.FreeHGlobal(ptr);
    return arr;
  }

  public static GrenadeThrowParameters UnMarshallGrenadeThrowParameters(byte[] arr)
  {
    GrenadeThrowParameters state = new GrenadeThrowParameters();
    int size = Marshal.SizeOf(state);
    IntPtr ptr = Marshal.AllocHGlobal(size);
    Marshal.Copy(arr, 0, ptr, size);
    state = (GrenadeThrowParameters)Marshal.PtrToStructure(ptr, state.GetType());
    Marshal.FreeHGlobal(ptr);
    return state;
  }

  public GrenadeThrowParameters BuildGrenadeThrowParameters()
  {
    GrenadeThrowParameters state = new GrenadeThrowParameters();
    state.forward = Camera.main.transform.TransformDirection(Vector3.forward);
    state.forward = state.forward.normalized;
    state.position = this.transform.position;
    state.rotation = this.transform.rotation;
    return state;
  }

  void Start()
  {
    //		controller = GameObject.FindGameObjectWithTag("Player").GetComponent<DeftPlayerController>();
    controller = transform.parent.gameObject.GetComponent<DeftPlayerController>();
  }

  void Update()
  {
    if (networkView.isMine)
    {
      trigger = controller.gamepadState.RightShoulder || (controller.gamepadState.RightTrigger > 0.20f);
      cooldown -= Time.deltaTime;

      if (trigger && cooldown <= 0.0)
      {
        trigger = false;
        cooldown = triggerCooldown;
        if (currentAmmo > 0)
        {
          byte[] state = MarshallGrenadeThrowParameters(BuildGrenadeThrowParameters());
          networkView.RPC("throwGrenade", RPCMode.Others);
          throwGrenade();
          currentAmmo--;
        }
      }
    }
  }

  void FixedUpdate()
  {
    if (currentAmmo < maxAmmo)
    {
      if (rechargeTimer > recharge)
      {
        currentAmmo++;
        rechargeTimer = 0;
      }
      rechargeTimer += Time.deltaTime;

    }
  }

  [RPC]
  void throwGrenade()
  {
    GrenadeThrowParameters state = BuildGrenadeThrowParameters();
    GameObject clone;
    clone = Instantiate(prefab, state.position + state.forward, state.rotation) as GameObject;
    clone.rigidbody.velocity = state.forward * magnitude;
  }
}
