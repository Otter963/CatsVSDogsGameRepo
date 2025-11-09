using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectUIScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private GameObject buttonUICanvas;

    [Header("LAN Discovery")]
    [SerializeField] private LanDiscovery lanScript;

    private void Start()
    {
        if (lanScript == null) lanScript = GetComponent<LanDiscovery>();

        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);
    }

    #region Host
    private void HostButtonOnClick()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started!");
            lanScript.BroadcastHost(); // start broadcasting

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            // Host spawns its player object automatically (make sure prefab is registered)
            CheckBeginOnline();
        }
        else
        {
            Debug.LogError("Failed to start host!");
        }
    }
    #endregion

    #region Client
    private void ClientButtonOnClick()
    {
        string ip = "127.0.0.1"; // default to localhost for editor

#if !UNITY_EDITOR
        ip = lanScript.GetDetectedHostIP();
        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogError("No host detected on LAN. Please wait and try again.");
            return;
        }
#endif

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip;

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log($"Client started! Connecting to {ip}");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            Debug.LogError("Failed to start client!");
        }
    }
    #endregion

    #region Network Callbacks
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
        CheckBeginOnline();
    }

    private void CheckBeginOnline()
    {
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            BeginOnline();
        }
    }

    private void BeginOnline()
    {
        Debug.Log("Both players connected. Starting game...");

        // Load networked scene (make sure it's in Build Settings!)
        if (NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("LevelOne", LoadSceneMode.Single);
        }

        // Hide UI
        if (buttonUICanvas != null)
        {
            buttonUICanvas.SetActive(false);
        }
    }
    #endregion
}