using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectUIScript : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private GameObject buttonUICanvas;
    [SerializeField] private LanDiscovery lanScript;

    private void Start()
    {
        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);
        lanScript = GetComponent<LanDiscovery>();
    }

    private void HostButtonOnClick()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started!");
            lanScript.BroadcastHost(); // start broadcasting!
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            CheckBeginOnline();
        }
        else
        {
            Debug.LogError("Failed to start host!");
        }
    }


    private void ClientButtonOnClick()
    {
        string ip;

#if UNITY_EDITOR
        // When testing in Unity multiplayer play mode, force localhost
        ip = "127.0.0.1";
#else
    // In actual LAN play, use discovered IP
    if (lanScript.detectedHostIP == null)
    {
        Debug.LogError("No host detected on LAN.");
        return;
    }
    ip = lanScript.detectedHostIP;
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
        const string targetScene = "LevelOne";

        int buildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scene/" + targetScene + ".unity");
        Debug.Log($"Loading {targetScene}, Build Index: {buildIndex}");

        if (buildIndex == -1)
        {
            Debug.LogError($"Scene '{targetScene}' not found in build settings!");
            return;
        }

        Debug.Log("Both players connected. Starting game...");
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
}
