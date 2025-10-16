using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectUIScript : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private GameObject buttonUICanvas;

    private void Start()
    {
        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);
    }

    private void HostButtonOnClick()
    {
        NetworkManager.Singleton.StartHost();
        buttonUICanvas.SetActive(false);
    }

    private void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
        buttonUICanvas.SetActive(false);
    }
}
