using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    public bool coinCollected = false;

    [SerializeField] private SimpleScore syncedScoreScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CoinCollection();
        }
    }

    public void CoinCollection()
    {
        syncedScoreScript.score.Value += 10;
        coinCollected = true;
        gameObject.SetActive(false);
    }
}
