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
    }

    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);
    }
}
