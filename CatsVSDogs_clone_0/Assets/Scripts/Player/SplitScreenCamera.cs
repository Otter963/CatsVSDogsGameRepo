using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class SplitScreenCamera : MonoBehaviour
{
    private Camera playerCam;
    private int index;
    private int totalPlayers;

    private void Awake()
    {
        PlayerInputManager.instance.onPlayerJoined += HandlePlayerJoined;
    }

    private void HandlePlayerJoined(PlayerInput input)
    {
        totalPlayers = PlayerInput.all.Count;
        SetupCamera();
    }

    private void SetupCamera()
    {
        if (totalPlayers == 1)
        {
            playerCam.rect = new Rect(0, 0, 1, 1);
        }
        else if (totalPlayers == 2)
        {
            playerCam.rect = new Rect(index == 0 ? 0 : 0.5f, 0, 0.5f, 1);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        index = GetComponentInParent<PlayerInput>().playerIndex;
        totalPlayers = PlayerInput.all.Count;
        playerCam = GetComponent<Camera>();
        playerCam.depth = index;

        SetupCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
