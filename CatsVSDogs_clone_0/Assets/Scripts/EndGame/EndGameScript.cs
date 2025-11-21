using System.Collections;
using UnityEngine;

public class EndGameScript : MonoBehaviour
{
    [SerializeField] private SimpleScore syncedScoreScript;

    [SerializeField] private float endAnimationTime;

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject hostJoinCanvas;

    [Header("Stuff to switch")]

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject endGameCamera;

    [SerializeField] private GameObject endGameAnimationCanvas;
    [SerializeField] private Animator endGameAnimator;

    [SerializeField] private PlayerPlatformMovementScript playerScript;

    [SerializeField] private GameObject quitGameCanvas;

    private void Update()
    {
        EndGame();
    }

    public void EndGame()
    {
        if (syncedScoreScript.score.Value == 60)
        {
            StartCoroutine(FinishGameAnimation());
            StartCoroutine(StopFinishGameAnimation());
        }
    }

    IEnumerator FinishGameAnimation()
    {
        playerScript.enabled = false;
        mainCamera.SetActive(false);
        endGameCamera.SetActive(true);
        endGameAnimationCanvas.SetActive(true);
        playerCanvas.SetActive(false);
        hostJoinCanvas.SetActive(false);
        endGameAnimator.SetBool("EndGame", true);
        yield return new WaitForSeconds(endAnimationTime);
        QuitGameScreen();
    }

    IEnumerator StopFinishGameAnimation()
    {
        yield return new WaitForSeconds(endAnimationTime);
        StopCoroutine(FinishGameAnimation());
    }

    public void QuitGameScreen()
    {
        Destroy(endGameAnimationCanvas);
        quitGameCanvas.SetActive(true);
    }
}
