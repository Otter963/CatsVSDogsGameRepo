using UnityEngine;

public class DoorDisableScript : MonoBehaviour
{
    [SerializeField] private ButtonDoorScript buttonOne;
    [SerializeField] private ButtonTwoDownScript buttonTwo;
    [SerializeField] private GameObject door;

    private void Update()
    {
        if (buttonOne.buttonOneDown == true && buttonTwo.buttonTwoDown == true)
        {
            door.SetActive(false);
        }
    }
}
