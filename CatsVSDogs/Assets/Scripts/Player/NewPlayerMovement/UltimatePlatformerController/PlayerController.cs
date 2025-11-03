using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] public InputActionAsset playerInputActions;

    public static Vector2 playerInputMovement;

    private InputActionMap player;

    private InputAction m_moveAction;

    private InputAction m_jumpAction;

    [Header("References")]
    public PlayerMovementStats moveStats;
    [SerializeField] private Collider2D feetColl;
    [SerializeField] private Collider2D bodyColl;

    private Rigidbody2D playerRB;

    //movement variables:
    private Vector2 moveVelocity;
    private bool isFacingRight;

    //collision check variables:
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isGrounded;
    private bool bumpedHead;

    private void Awake()
    {
        player = playerInputActions.FindActionMap("Player");
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");

        isFacingRight = true;

        playerRB = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CollisionChecks();

        if (isGrounded)
        {
            Move(moveStats.groundAcceleration, moveStats.groundDeceleration, playerInputMovement);
        }
        else
        {
            Move(moveStats.airAcceleration, moveStats.airDeceleration, playerInputMovement);
        }
    }

    #region

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            //check if player needs to turn
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (m_moveAction.WasPerformedThisFrame())
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * moveStats.maxWalkSpeed;
            }

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            playerRB.linearVelocity = new Vector2(moveVelocity.x, moveVelocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            playerRB.linearVelocity = new Vector2(moveVelocity.x, playerRB.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, moveStats.groundDetectionRayLen);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, moveStats.groundDetectionRayLen, moveStats.groundLayer);

        if (groundHit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CollisionChecks()
    {
        IsGrounded();
    }

    #endregion
}
