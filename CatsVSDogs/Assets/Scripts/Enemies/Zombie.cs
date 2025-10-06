using UnityEngine;

public class Zombie : Enemy
{
    protected override void Start()
    {
        enemyRB.gravityScale = 12f;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(PlayerMovementScript.instance.transform.position.x, transform.position.y),
                speed * Time.deltaTime);
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
}
