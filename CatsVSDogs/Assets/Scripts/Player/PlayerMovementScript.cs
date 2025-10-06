using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] public InputActionAsset playerInputActions;

    private InputActionMap player;

    private InputAction m_jumpAction;

    private InputAction m_dashAction;

    [Header("Player Settings")]
    [SerializeField] private Rigidbody2D playerRB;

    [SerializeField] private float walkSpeed;

    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 45;

    private float airJumpCounter = 0;
    [SerializeField] private float maxAirJumps;

    private float jumpBufferCounter = 0;

    [SerializeField] private int jumpBufferFrames;

    [Header("Dash Settings")]
    private bool canDash = true;
    private bool dashed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private float playerGravity;
    [SerializeField] private GameObject dashEffect;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;

    [SerializeField] private float groundCheckY = 0.2f;

    [SerializeField] private float groundCheckX = 0.2f;

    [SerializeField] private LayerMask whatIsGround;

    [Header("Animator Settings")]
    [SerializeField] public Animator playerAnim;

    //Player states

    private PlayerStateList pState;

    //creating a singleton of this script
    public static PlayerMovementScript instance;

    private float xAxis;

    private void Awake()
    {
        player = playerInputActions.FindActionMap("Player");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_dashAction = InputSystem.actions.FindAction("Dash");

        pState = GetComponent<PlayerStateList>();

        playerGravity = playerRB.gravityScale;

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
        UpdateJumpVariables();

        if (pState.isDashing) return;
        FlipPlayer();
        Move();
        Jump();
        StartDash();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void FlipPlayer()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void Move()
    {
        playerRB.linearVelocity = new Vector2(walkSpeed * xAxis, playerRB.linearVelocity.y);
        playerAnim.SetBool("IsWalking", playerRB.linearVelocity.x != 0 && Grounded());
    }

    public void StartDash()
    {
        if (m_dashAction.IsPressed() && canDash && !dashed)
        {
            Debug.Log("Dash pressed");
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.isDashing = true;
        playerAnim.SetTrigger("IsDashing");
        playerRB.gravityScale = 0;
        playerRB.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        playerRB.gravityScale = playerGravity;
        pState.isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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

        //when player lets go of jump button
        if (m_jumpAction.WasReleasedThisFrame() && playerRB.linearVelocity.y > 0)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, 0);

            pState.isJumping = false;
        }

        if (!pState.isJumping)
        {
            //when player presses jump button
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, jumpForce);

                pState.isJumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && m_jumpAction.IsPressed()) //double jump
            {
                pState.isJumping = true;

                airJumpCounter++;

                playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, jumpForce);
            }
        }

        playerAnim.SetBool("IsJumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.isJumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (m_jumpAction.IsPressed())
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    private void OnEnable()
    {
        playerInputActions.FindActionMap("Player").Enable();
        player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.FindActionMap("Player").Disable();
        player.Disable();
    }
}
