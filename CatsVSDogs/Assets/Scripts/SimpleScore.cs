using UnityEngine;
using Unity.Netcode;
using TMPro;

public class SimpleScore : NetworkBehaviour
{
    public NetworkVariable<int> score = new NetworkVariable<int>();

    public NetworkVariable<string> scoreStringText = new NetworkVariable<string>();

    public TextMeshProUGUI scoreText;

    void Start()
    {
        score.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"Score changed to: {newValue}");
            scoreStringText.Value = score.Value.ToString();
        };
    }

    void Update()
    {
        if (IsServer && Input.GetKeyDown(KeyCode.S))
        {
            score.Value += 10;
            scoreText.text = scoreStringText.ToString();
            Debug.Log("Server added 10 points");
        }
    }
}
