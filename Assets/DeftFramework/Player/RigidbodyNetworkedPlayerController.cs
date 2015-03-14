using UnityEngine;
using System.Collections;
using GamepadInput;

public enum PlayerControllerState { WALKING, RUNNING, AIMING };
public enum MovementType { IMPULSE, SETVELOCITY, FORCE, THRUSTERS };

public class RigidbodyNetworkedPlayerController : MonoBehaviour
{

  public PlayerControllerState playerState;
  public MovementType movementType;

  public float baseSpeed = 2.0f;
  public float runSpeedMultiplier = 1.5f;
  public float aimSpeedMultiplier = 0.2f;
  public float horizontalAimingSpeed = 400f;
  public float verticalAimingSpeed = 400f;
  public float velocityDampingSpeed = 0.02f;
  public float impulseDampingSpeed = 0.02f;

  public float jumpHeight = 2.0f;
  public float jumpCooldown;
  float jumpCooldownTmp;

  public float relCameraPosMag = 1.5f;
  public Vector3 pivotOffset = new Vector3(1.5f, 0.0f, -2.0f);
  public Vector3 camOffset = new Vector3(1f, 3.5f, -7f);
  public Vector3 aimPivotOffset = new Vector3(1.0f, 0.0f, 0.0f);
  public Vector3 aimCamOffset = new Vector3(1f, 3f, -5.0f);
  public Vector3 runPivotOffset = new Vector3(1f, 0.0f, -2.0f);
  public Vector3 runCamOffset = new Vector3(1f, 3.5f, -8f);
  public float runFOV = 80f;
  public float aimFOV = 40f;
  public float FOV = 60f;

  public float smoothingTurn = 5.0f;
  public float smoothingAim = 10.0f;

  public Vector2 controllerMoveDirection;
  public Vector2 controllerLookDirection;
  public float exponentialControllerJoystickModifier = 3.0f;
  Vector3 moveDirection;

  public bool debug = true;

  public GamePad.Index padIndex;
  [HideInInspector]
  public GamepadState gamepadState;

  public bool isThisMachinesPlayer = false;
  public bool useGamePad = false;

  public Camera myCamera;
  Vector3 smoothPivotOffset;
  Vector3 smoothCamOffset;
  Vector3 targetPivotOffset;
  Vector3 targetCamOffset;
  float defaultFOV;
  float targetFOV;

  public bool showCrosshair = false;
  public Texture2D verticalTexture;
  public Texture2D horizontalTexture;

  void Awake()
  {
    if (debug)
    {
      Debug.Log(this.ToString() + " awake.");
    }
    this.padIndex = GamePad.Index.Any;
    if (Network.isClient || Network.isServer)
    {
      if (this.GetComponent<NetworkView>().isMine)
      {
        this.isThisMachinesPlayer = true;
      }
    }
    else
    {
      this.isThisMachinesPlayer = true;
    }
    if (this.isThisMachinesPlayer)
    {
      if (debug)
      {
        Debug.Log("This machine owns player " + this.ToString());
      }
      GrabCamera(Camera.main);
    }
  }

  void GrabCamera(Camera cam)
  {
    this.myCamera = cam;
    smoothPivotOffset = pivotOffset;
    smoothCamOffset = camOffset;
    Debug.Log("Camera has been reset by player");
    if (debug)
    {
      Debug.Log("Player grabbed camera " + cam.ToString());
    }
  }

  float angleH;
  float angleV;
  void AdjustCamera()
  {
    angleH += this.controllerLookDirection.x * this.horizontalAimingSpeed * Time.deltaTime;
    angleV += this.controllerLookDirection.y * this.verticalAimingSpeed * Time.deltaTime;
    //angleV = Mathf.Clamp(angleV, 80, 80);
    //angleH = Mathf.Clamp(angleH, -80, 80);
    Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
    if (this.GetComponent<Rigidbody>().velocity.magnitude > 0.2f)
    {
      aimRotation = Quaternion.Slerp(aimRotation, Quaternion.Euler(this.transform.forward), this.velocityDampingSpeed);
    }
    Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
    this.myCamera.transform.rotation = aimRotation;
    if (this.playerState == PlayerControllerState.AIMING)
    {
      targetPivotOffset = aimPivotOffset;
      targetCamOffset = aimCamOffset;
      targetFOV = aimFOV;
    }
    else if (this.playerState == PlayerControllerState.RUNNING)
    {
      targetPivotOffset = runPivotOffset;
      targetCamOffset = runCamOffset;
      targetFOV = runFOV;
    }
    else
    {
      targetPivotOffset = pivotOffset;
      targetCamOffset = camOffset;
      targetFOV = FOV;
    }
    this.myCamera.fieldOfView = Mathf.Lerp(this.myCamera.fieldOfView, targetFOV, Time.deltaTime);

    #region Collisions
    Vector3 baseTempPosition = this.transform.position + camYRotation * targetPivotOffset;
    Vector3 tempOffset = targetCamOffset;
    for (float zOffset = targetCamOffset.z; zOffset < 0; zOffset += 0.5f)
    {
      tempOffset.z = zOffset;
      if (DoubleViewingPosCheck(baseTempPosition + aimRotation * tempOffset))
      {
        targetCamOffset.z = tempOffset.z;
        break;
      }
    }
    #endregion

    smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, 10f * Time.deltaTime);
    smoothCamOffset = Vector3.Lerp(smoothCamOffset, targetCamOffset, 10f * Time.deltaTime);

    this.myCamera.transform.position = this.transform.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
  }


  Texture2D temp;
  public float spread;
  public float minSpread = 20.0f;
  public float maxSpread = 40.0f;
  void DrawCrossHair()
  {
    GUIStyle verticalT = new GUIStyle();
    GUIStyle horizontalT = new GUIStyle();
    verticalT.normal.background = verticalTexture;
    horizontalT.normal.background = horizontalTexture;
    spread = Mathf.Clamp(spread, minSpread, maxSpread);
    Vector2 pivot = new Vector2(Screen.width / 2, Screen.height / 2);
    GUI.Box(new Rect((Screen.width - 2) / 2, (Screen.height - spread) / 2 - 14, 2, 14), temp, horizontalT);
    GUIUtility.RotateAroundPivot(45, pivot);
    GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - 2) / 2, 14, 2), temp, verticalT);
    GUIUtility.RotateAroundPivot(0, pivot);
    GUI.Box(new Rect((Screen.width - 2) / 2, (Screen.height + spread) / 2, 2, 14), temp, horizontalT);
  }

  void OnGUI()
  {
    if (this.playerState == PlayerControllerState.AIMING && this.isThisMachinesPlayer)
    {
      this.DrawCrossHair();
    }
  }

  bool DoubleViewingPosCheck(Vector3 checkPos)
  {
    return ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);
  }

  bool ViewingPosCheck(Vector3 checkPos)
  {
    RaycastHit hit;
    if (Physics.Raycast(checkPos, this.transform.position - checkPos, out hit, relCameraPosMag))
    {
      if (hit.transform != this && !hit.transform.GetComponent<Collider>().isTrigger)
      {
        return false;
      }
    }
    return true;
  }

  bool ReverseViewingPosCheck(Vector3 checkPos)
  {
    RaycastHit hit;
    if (Physics.Raycast(this.transform.position, checkPos - this.transform.position, out hit, relCameraPosMag))
    {
      if (hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
      {
        return false;
      }
    }
    return true;
  }

  void Update()
  {
    #region TimerMaintenance
    jumpCooldownTmp -= Time.deltaTime;
    #endregion

    #region GatherInput
    if (this.isThisMachinesPlayer)
    {
      if (this.useGamePad)
      {
        this.gamepadState = GamePad.GetState(this.padIndex);
        this.controllerMoveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, this.padIndex);
        this.controllerLookDirection = GamePad.GetAxis(GamePad.Axis.RightStick, this.padIndex);
        this.controllerMoveDirection.y = Mathf.Pow(this.controllerMoveDirection.y, this.exponentialControllerJoystickModifier);
        this.controllerMoveDirection.x = Mathf.Pow(this.controllerMoveDirection.x, this.exponentialControllerJoystickModifier);
        this.controllerLookDirection.y = Mathf.Pow(this.controllerLookDirection.y, this.exponentialControllerJoystickModifier);
        this.controllerLookDirection.x = Mathf.Pow(this.controllerLookDirection.x, this.exponentialControllerJoystickModifier);
      }
      else
      {
        Debug.Log("Gathering keyboard.");
        this.controllerMoveDirection = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
        this.controllerLookDirection = new Vector2(Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1), Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1));
      }
    }
    #endregion

    #region SettingPlayerState
    if (this.gamepadState.LeftTrigger > 0.20f)
    {
      this.playerState = PlayerControllerState.AIMING;
    }
    else if (this.gamepadState.LeftStick)
    {
      this.playerState = PlayerControllerState.RUNNING;
    }
    else
    {
      this.playerState = PlayerControllerState.WALKING;
    }
    #endregion

    Vector3 forward = this.myCamera.transform.TransformDirection(Vector3.forward);
    forward = forward.normalized;
    this.moveDirection = this.controllerMoveDirection.y * forward + this.controllerMoveDirection.x * new Vector3(forward.z, 0, -forward.x);

    AdjustCamera();

    #region RunningActionByState
    forward = this.myCamera.transform.TransformDirection(Vector3.forward);
    forward = forward.normalized;
    this.moveDirection = this.controllerMoveDirection.y * forward + this.controllerMoveDirection.x * new Vector3(forward.z, 0, -forward.x);
    moveDirection *= this.baseSpeed;
    switch (this.playerState)
    {
      case PlayerControllerState.WALKING:
        break;
      case PlayerControllerState.RUNNING:
        moveDirection *= this.runSpeedMultiplier;
        break;
      case PlayerControllerState.AIMING:
        moveDirection *= this.aimSpeedMultiplier;
        break;
    }
    #endregion

    #region Movement
    if (this.moveDirection.magnitude > 0.05f)
    {
      switch (this.movementType)
      {
        case MovementType.SETVELOCITY:
          this.GetComponent<Rigidbody>().velocity = Vector3.Lerp(this.GetComponent<Rigidbody>().velocity, this.moveDirection, this.velocityDampingSpeed * Time.deltaTime);
          this.transform.forward = Vector3.Lerp(this.transform.forward, this.GetComponent<Rigidbody>().velocity.normalized, this.velocityDampingSpeed * Time.deltaTime);
          this.GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(this.GetComponent<Rigidbody>().angularVelocity, Vector3.zero, this.velocityDampingSpeed * Time.deltaTime);
          break;
        case MovementType.IMPULSE:
          this.GetComponent<Rigidbody>().AddForce(this.moveDirection * this.GetComponent<Rigidbody>().mass * this.impulseDampingSpeed, ForceMode.Impulse);
          this.transform.forward = Vector3.Lerp(this.transform.forward, this.moveDirection, this.velocityDampingSpeed * Time.deltaTime);
          this.GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(this.GetComponent<Rigidbody>().angularVelocity, Vector3.zero, this.velocityDampingSpeed * Time.deltaTime);
          break;
        case MovementType.THRUSTERS:
          this.GetComponent<Thrusters>().ActivatePrimaryMovementThrusters(this.moveDirection);
          break;
      }

    }
    #endregion
  }

  public bool isGrounded()
  {
    return Physics.Raycast(this.transform.position, Vector3.down, (this.GetComponent<CapsuleCollider>().height / 2.0f) + 0.1f);
  }
}
