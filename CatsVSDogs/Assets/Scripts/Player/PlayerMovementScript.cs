using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] public InputActionAsset playerInputActions;

    private InputAction m_jumpAction;

    [Header("Player Settings")]
    [SerializeField] private Rigidbody2D playerRB;

    [SerializeField] private float walkSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 45;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;

    [SerializeField] private float groundCheckY = 0.2f;

    [SerializeField] private float groundCheckX = 0.2f;

    [SerializeField] private LayerMask whatIsGround;

    [Header("Animator Settings")]
    [SerializeField] public Animator playerAnim;

    //creating a singleton of this script
    public static PlayerMovementScript instance;

    private float xAxis;

    private void Awake()
    {
        m_jumpAction = InputSystem.actions.FindAction("Jump");

        /*
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        */
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Move();
        Jump();
        FlipPlayer();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void FlipPlayer()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    private void Move()
    {
        playerRB.linearVelocity = new Vector2(walkSpeed * xAxis, playerRB.linearVelocity.y);
        playerAnim.SetBool("IsWalking", playerRB.linearVelocity.x != 0 && Grounded());
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) || 
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Jump()
    {
        //variable jump

        if (m_jumpAction.WasReleasedThisFrame() && playerRB.linearVelocity.y > 0)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 0);
        }

        if (m_jumpAction.IsPressed() && Grounded())
        {
            playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, jumpForce);
        }

        playerAnim.SetBool("IsJumping", !Grounded());
    }

    private void OnEnable()
    {
        playerInputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        playerInputActions.FindActionMap("Player").Disable();
    }
}
