using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonDoorScript : MonoBehaviour
{
    public bool buttonOneDown;

    [SerializeField] private GameObject buttonOneDownUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            buttonOneDown = true;
            buttonOneDownUI.SetActive(true);
        }
    }
}
