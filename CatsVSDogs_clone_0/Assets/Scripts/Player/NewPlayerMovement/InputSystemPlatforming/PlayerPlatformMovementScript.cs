using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerPlatformMovementScript : NetworkBehaviour
{
    public Rigidbody2D playerRB;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator attackAnimator;
    private bool isFacingRight = true;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        //for when the player is idling
        if (horizontal == 0 && IsGrounded())
        {
            playerAnimator.SetBool("PlayerJump", false);
            playerAnimator.SetBool("PlayerFall", false);
            playerAnimator.SetBool("PlayerIdle", true);
        }
        else if (horizontal == 0 && !IsGrounded())
        {
            playerAnimator.SetBool("PlayerJump", true);
        }
        else if (horizontal == 0 && !IsGrounded() && playerRB.linearVelocity.y < 0f)
        {
            playerAnimator.SetBool("PlayerJump", false);
            playerAnimator.SetBool("PlayerFall", true);
        }

        //when the player is falling
        if (playerRB.linearVelocity.y < 0f && !IsGrounded())
        {
            playerAnimator.SetBool("PlayerFall", true);
        }

        //when the player moves
        playerRB.linearVelocity = new Vector2(horizontal * speed, playerRB.linearVelocity.y);

        //flipping the player
        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
            playerAnimator.SetBool("PlayerJump", true);
        }

        if (context.canceled && playerRB.linearVelocity.y > 0f)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.y * 0.5f);
            playerAnimator.SetBool("PlayerJump", false);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void PlayerMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        playerAnimator.SetBool("PlayerIdle", false);
        playerAnimator.SetBool("PlayerJump", false);
        playerAnimator.SetBool("PlayerFall", false);
    }

    public void PlayerAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attackAnimator.SetBool("Attack", true);
        }
        else
        {
            attackAnimator.SetBool("Attack", false);
        }
    }
}
