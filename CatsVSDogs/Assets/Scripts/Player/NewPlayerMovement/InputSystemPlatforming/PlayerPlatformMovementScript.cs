using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlatformMovementScript : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 16f;
    private bool isFacingRight = true;

    // Update is called once per frame
    void Update()
    {
        playerRB.linearVelocity = new Vector2(horizontal * speed, playerRB.linearVelocity.y);

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
        }

        if (context.canceled && playerRB.linearVelocity.y > 0f)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.y * 0.5f);
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
    }
}
