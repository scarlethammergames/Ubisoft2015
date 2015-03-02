using System.Collections;
using System.Reflection;
using GamepadInput;
using UnityEngine;


public enum PlayerState { aiming, walking, running, sprinting, jumping };


/// <summary>
/// 
/// The player state is checked with the following precedence:
///   1. Aiming
///   2. Running
///   3. Sprinting
///   4. Walking
/// 
/// </summary>
public class DeftPlayerController : MonoBehaviour
{
    public float speedAim = 1.0f;
    public float speedWalk = 2.0f;
    public float speedRun = 4.0f;
    public float speedSprint = 7.0f;
    public float jumpHeight = 5.0f;
    public float jumpCooldown = 1.0f;
    public float smoothingTurn = 2.0f;
    public float smoothingAim = 5.0f;

    public float smooth = 20f;

    public float playerHeight;
    public float playerWidth;

    public bool debug;
    public bool useGamepad;
    public bool singlePlayer;

    public bool isGrounded;
    public PlayerState state;

    public bool inverted = false;
    float invertTimer = 0;

    GamePad.Index pad_index = GamePad.Index.One;

    float speed_current;
    Vector3 move_direction;
    Vector3 forward;
    Vector3 last_input;

    Animator animator;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
        if (networkView.isMine || singlePlayer)
        {
            //Camera.main.GetComponent<DeftPlayerCamera>().player = this.gameObject;
            //Camera.main.GetComponent<DeftPlayerCamera>().Reset();
        }
        Debug.Log("PLAYER IS AWAKE");
    }



    /// <summary>
    /// Variables stored here for performance purposes and if anothere class would like to observe.
    /// </summary>
    public bool controllerJump;
    public bool controllerRun;
    public bool controllerSprint;
    public float controllerAim;
    public Vector2 controllerMoveDirection;
    public Vector2 controllerLookDirection;
    public Vector2 dpadDown;

    void Update()
    {
        if (networkView.isMine || singlePlayer)
        {
            if (useGamepad)
            {
                controllerJump = GamePad.GetButtonDown(GamePad.Button.A, pad_index);
                controllerRun = GamePad.GetButton(GamePad.Button.LeftStick, pad_index);
                controllerSprint = GamePad.GetButton(GamePad.Button.RightStick, pad_index);
                controllerAim = GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, pad_index);
                dpadDown = GamePad.GetAxis(GamePad.Axis.Dpad, pad_index);
            }
            else
            {
                controllerJump = Input.GetKeyDown(KeyCode.Space);
                controllerRun = Input.GetKey(KeyCode.LeftShift);
                controllerSprint = Input.GetKey(KeyCode.C);
                //controllerAim = Input.GetMouseButton(1);
            }

            invertTimer += Time.deltaTime;

            // invert y axis if down on dpad is pressed
            if (dpadDown.y < 0 && invertTimer > 1)
            {
                if (inverted)
                    inverted = false;
                else
                    inverted = true;

                invertTimer = 0;
            }


            if (controllerAim > 0.20f)
            {
                this.state = PlayerState.aiming;
            }
            else if (controllerJump)
            {
                this.state = PlayerState.jumping;
            }
            else if (controllerSprint && controllerRun)
            {
                this.state = PlayerState.sprinting;
            }
            else if (controllerRun)
            {
                this.state = PlayerState.running;
            }
            else
            {
                this.state = PlayerState.walking;
            }
            this.controllerMoveDirection = new Vector3(0, 0, 0);
            this.controllerLookDirection = new Vector3(0, 0, 0);
            if (useGamepad)
            {
                this.controllerMoveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, pad_index);
                this.controllerLookDirection = GamePad.GetAxis(GamePad.Axis.RightStick, pad_index);
            }
            else
            {
                this.controllerMoveDirection = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
                this.controllerLookDirection = new Vector2(Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1), Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1));
            }

            if (inverted)
                this.controllerLookDirection.y *= -1;
        }
    }

    void FixedUpdate()
    {
        if (debug)
        {
            foreach (FieldInfo info in this.gameObject.GetComponent<DeftPlayerController>().GetType().GetFields())
            {
                if (info.Name.Contains("controller_"))
                    Debug.Log(info.Name + ": " + info.GetValue(this.gameObject.GetComponent<DeftPlayerController>()));
            }
            Debug.Log("CURRENT STATE: " + this.state.ToString());
        }

        Animate();
        // last jump

        // get forward direction
        forward = Camera.main.transform.TransformDirection(Vector3.forward);
        forward = forward.normalized;

        this.move_direction = this.controllerMoveDirection.y * forward + this.controllerMoveDirection.x * new Vector3(forward.z, 0, -forward.x);

        if (this.move_direction.x != 0 || this.move_direction.z != 0)
        {
            last_input = move_direction;
        }

        switch (this.state)
        {
            case PlayerState.aiming:
                {
                    speed_current = speedAim;
                    break;
                }
            case PlayerState.jumping:
                {
                    if (speed_current > 0)
                    {
                        rigidbody.velocity = new Vector3(0, jumpHeight, 0);
                    }
                    break;
                }
            case PlayerState.sprinting:
                {
                    speed_current = speedSprint;
                    break;
                }
            case PlayerState.running:
                {
                    speed_current = speedRun;
                    break;
                }
            default:
                {
                    speed_current = speedWalk;
                    break;
                }
        }

        // change forward direction
        Vector3 last_input_without_y = new Vector3(last_input.x, 0, last_input.z);
        Vector3 forward_without_y = new Vector3(transform.forward.x, 0, transform.forward.z);

        transform.forward = Vector3.Lerp(forward_without_y, last_input_without_y, smooth * Time.deltaTime);

        //this.rigidbody.AddForce(this.move_direction * speed_current);
        Vector3 move_without_y = new Vector3(this.move_direction.x, 0, this.move_direction.z);
        this.rigidbody.velocity = new Vector3(move_direction.x * speed_current, rigidbody.velocity.y, move_direction.z * speed_current);

        // THIS IS A MASSIVE HACK VERY BAD WILL FIX SOON AFTER I GET SOME MILK!!
        if (this.controllerMoveDirection.x == 0 && this.controllerMoveDirection.y == 0)
        {
            this.rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }
    }

    public void Animate()
    {

    }


    public bool CalculateIsGrounuded()
    {
        return Physics.Raycast(transform.position, Vector3.down, (this.playerHeight / 2.0f) + 0.05f);
    }

}
