using UnityEngine;
using UnityEngine.InputSystem;

/*References:
Title: Local Multiplayer and Split-Screen - new input and Cinemachine 
Author: One Wheel Studio
Date: 2022, February 14
Code version: 6000.0.51f1
Availability: https://www.youtube.com/watch?v=l9HrraxtdGY
*/

public class ToggleOnPlayerJoin : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindFirstObjectByType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += ToggleThis;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= ToggleThis;
    }

    private void ToggleThis(PlayerInput player)
    {
        this.gameObject.SetActive(false);
    }
}
