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

    [SerializeField] protected float speed;

    protected virtual void Awake()
    {
        enemyRB = GetComponent<Rigidbody2D>();
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

    public virtual void EnemyHit(float _damageDone)
    {
        health -= _damageDone;
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            EnemyAttack();
        }
    }

    protected virtual void EnemyAttack()
    {
        
    }
}
