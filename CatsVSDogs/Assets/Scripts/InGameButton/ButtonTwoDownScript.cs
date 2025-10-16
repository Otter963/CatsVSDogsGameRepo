using UnityEngine;

public class ButtonTwoDownScript : MonoBehaviour
{
    public bool buttonTwoDown;

    [SerializeField] private GameObject buttonTwoDownUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            buttonTwoDown = true;
            buttonTwoDownUI.SetActive(true);
        }
    }
}
