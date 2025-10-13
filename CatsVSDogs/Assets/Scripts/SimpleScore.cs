using UnityEngine;
using Unity.Netcode;

public class SimpleScore : NetworkBehaviour
{
    public NetworkVariable<int> score = new NetworkVariable<int>();

    void Start()
    {
        score.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"Score changed to: {newValue}");
        };
    }

    void Update()
    {
        if (IsServer && Input.GetKeyDown(KeyCode.S))
        {
            score.Value += 10;
            Debug.Log("Server added 10 points");
        }
    }
}
