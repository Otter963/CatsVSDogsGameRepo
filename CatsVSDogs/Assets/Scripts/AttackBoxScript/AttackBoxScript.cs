using UnityEngine;

public class AttackBoxScript : MonoBehaviour
{
    [SerializeField] private Enemy enemyScript;
    [SerializeField] private float destroyEnemyDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attackable"))
        {
            enemyScript.EnemyHit(destroyEnemyDamage);
        }
    }
}
