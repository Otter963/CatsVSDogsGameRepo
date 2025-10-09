using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;

    [SerializeField] protected float recoilLength;
    [SerializeField] float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    protected float recoilTimer;
    protected Rigidbody2D enemyRB;
    [SerializeField] protected float enemyDamage;

    [SerializeField] protected PlayerMovementScript player;
    [SerializeField] protected float speed;

    protected virtual void Awake()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerMovementScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            enemyRB.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerMovementScript.instance.pState.invincible)
        {
            EnemyAttack();
        }
    }

    protected virtual void EnemyAttack()
    {
        PlayerMovementScript.instance.TakeDamage(enemyDamage);
    }
}
