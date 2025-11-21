using UnityEngine;
using Unity.Netcode;
using TMPro;

//note that at the moment, this only works on the host's side, the client cannot add to the score
public class SimpleScore : NetworkBehaviour
{
    //the variable you want across host and client

    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //the score text, I used TextMeshPro, but should work with normal text too
    public TextMeshProUGUI scoreText;

    [SerializeField] private CollectCoin coinCollection;

    private void Start()
    {
        score.OnValueChanged += OnScoreChanged;
    }

    public override void OnNetworkSpawn()
    {
        //Updating the UI to the zero initalized as before
        scoreText.text = score.Value.ToString();
    }

    private void OnScoreChanged(int oldValue, int newValue)
    {
        Debug.Log($"Score changed from {oldValue} to {newValue}");
        //changing the text to the value, since it's an integer
        scoreText.text = newValue.ToString();
    }

    void Update()
    {
        if (IsServer && coinCollection.coinCollected == true)
        {
            coinCollection.coinCollected = false;
            Debug.Log("Server added 10 points");
        }
    }
}
