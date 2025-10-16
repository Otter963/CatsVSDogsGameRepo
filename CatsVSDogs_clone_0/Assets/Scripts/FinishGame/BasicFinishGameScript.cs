using UnityEngine;

public class BasicFinishGameScript : MonoBehaviour
{
    [SerializeField] private GameObject gameWinUI;

    [SerializeField] private GameObject buttonDownUI;

    [SerializeField] private PlayerMovementScript playerMovementScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameWinUI.SetActive(true);
            buttonDownUI.SetActive(false);
            playerMovementScript.enabled = false;
            Time.timeScale = 0;
        }
    }
}
