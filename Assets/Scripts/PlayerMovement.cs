using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Animator anim;

    [Header("Input")]
    [SerializeField] private int playerIndex;
    private Controls controls;
    private InputActionMap inputActions;
    private InputAction joystickAction;
    private InputAction parryAction;
    private Joystick joystick;

    //:::::::::::::::::::::

    [Header("Parry")]
    [SerializeField] public float parryWindowDuration = 0.25f;
    [SerializeField] private float parryCooldown = 0.5f;
    public float parryWindowStartTime = -1;    
    private float parryWindowEndTime = - 1;
    private float parryCooldownEndTime = -1;

    public bool isParryWindowActive => Time.time <= parryWindowEndTime;
    public bool isParryOnCooldown => Time.time <= parryCooldownEndTime;



    private Vector2 joystickVector;

    //::::::::::::::::::::::

    [SerializeField] private float moveSpeed = 10;

    Rigidbody2D rb2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        controls = new Controls();

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        joystick = Joystick.all[playerIndex];

        //Make input action asset only take input from specified gamepad
        InputDevice[] devices = new InputDevice[1];
        devices[0] = joystick;
        controls.devices = devices;

        //Setup each input action
        inputActions = controls.PlayerNormal;
        joystickAction = controls.PlayerNormal.PlayerMovement;
        parryAction = controls.PlayerNormal.Aim;

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        joystickVector = joystickAction.ReadValue<Vector2>();

        //Move logic
        MovePlayer(joystickVector.x);

        //::::::::::::::::::

        //Aim/Parry logic
        if (parryAction.WasPressedThisFrame() && !isParryOnCooldown)
        {
            parryWindowStartTime  = Time.time;
            parryWindowEndTime = Time.time + parryWindowDuration;
            parryCooldownEndTime = Time.time + parryCooldown;

            if (isParryWindowActive) anim.SetTrigger("PadleTrigger");
        }

        Debug.LogError(Gamepad.all.Count);
    }

    void MovePlayer(float Xaxis)
    {
        rb2D.linearVelocityX = Xaxis * moveSpeed;
    }

    public void BeginGame()
    {
        GetComponent<PlayerMovement>().enabled = true;  
    }

}
