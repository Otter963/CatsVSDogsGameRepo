using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovementScript : NetworkBehaviour
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

    [SerializeField] private float jumpBufferFrames;

    [Header("Dash Settings")]
    private bool canDash = true;
    private bool dashed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private float playerGravity;
    [SerializeField] private GameObject dashEffect;
    [Space(5)]

    [Header("Attack Settings")]
    bool attack = false;
    [SerializeField] private float timeBetweenAttack;
    private float timeSinceAttack;
    private InputAction m_attackAction;
    [SerializeField] private Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] private GameObject slashEffect;

    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil Settings")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health Settings")]
    public int playerHealth;
    public int maxPlayerHealth;
    [SerializeField] private GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    [SerializeField] private SpriteRenderer playerSR;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;

    [SerializeField] private float groundCheckY = 0.2f;

    [SerializeField] private float groundCheckX = 0.2f;

    [SerializeField] private LayerMask whatIsGround;

    [Header("Animator Settings")]
    [SerializeField] public Animator playerAnim;

    //Player states

    [HideInInspector] public PlayerStateList pState;

    //creating a singleton of this script
    public static PlayerMovementScript instance;

    private float xAxis;
    private float yAxis;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    
    private void Awake()
    {
        player = playerInputActions.FindActionMap("Player");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_dashAction = InputSystem.actions.FindAction("Dash");
        m_attackAction = InputSystem.actions.FindAction("Attack");

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

        Health = maxPlayerHealth;
    }
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        GetInputs();
        UpdateJumpVariables();

        if (pState.isDashing) return;
        FlipPlayer();
        Move();
        Jump();
        StartDash();
        Attack();
        RestoreTimeScale();
        FlashWhileInvincible();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = m_attackAction.WasPressedThisFrame();
    }

    void FlipPlayer()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        playerRB.linearVelocity = new Vector2(walkSpeed * xAxis, playerRB.linearVelocity.y);
        playerAnim.SetBool("IsWalking", playerRB.linearVelocity.x != 0 && Grounded());
    }

    public void StartDash()
    {
        if (IsClient && m_dashAction.IsPressed() && canDash && !dashed)
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

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            playerAnim.SetTrigger("IsAttacking");

            if (yAxis == 0 || yAxis < 0 && Grounded()) //side attack
            {
                Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slashEffect, sideAttackTransform);
            }
            else if (yAxis > 0) //up attack
            {
                 Hit(upAttackTransform, upAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 90, upAttackTransform);
            }
            else if (yAxis < 0 && !Grounded()) //down attack
            {
                Hit(downAttackTransform, downAttackArea, ref pState.recoilingX, recoilXSpeed);
                SlashEffectAtAngle(slashEffect, -90, downAttackTransform);
            }
        }
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        if (objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
                hitEnemies.Add(e);
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                playerRB.linearVelocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                playerRB.linearVelocity = new Vector2(recoilXSpeed, 0);
            }
        }
        if (pState.recoilingY)
        {
            playerRB.gravityScale = 0;
            if (yAxis < 0)
            {
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, recoilYSpeed);
            }
            else
            {
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            playerRB.gravityScale = playerGravity;
        }

        //stop recoil

        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        playerAnim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    void FlashWhileInvincible()
    {
        playerSR.material.color = pState.invincible ? 
            Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            Time.timeScale += Time.deltaTime * restoreTimeSpeed;
        }
        else
        {
            Time.timeScale = 1;
            restoreTime = true;
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    public int Health
    {
        get { return playerHealth; } 
        set
        {
            if (playerHealth != value)
            {
                Health = Mathf.Clamp(value, 0, maxPlayerHealth);
            }
        }
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
            else if (!Grounded() && airJumpCounter < maxAirJumps && m_jumpAction.WasPressedThisFrame()) //double jump
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

        if (m_jumpAction.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
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
