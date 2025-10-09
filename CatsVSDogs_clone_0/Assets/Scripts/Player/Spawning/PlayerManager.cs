using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/*References:
Title: Local Multiplayer and Split-Screen - new input and Cinemachine 
Author: One Wheel Studio
Date: 2022, February 14
Code version: 6000.0.51f1
Availability: https://www.youtube.com/watch?v=l9HrraxtdGY
*/

public class PlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindFirstObjectByType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        //using the parent of the prefab
        Transform playerPos = player.transform;
        playerPos.position = spawnPoints[players.Count - 1].position;


        /*
        //converting the (bit) layer mask into an integer
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        //setting the layer
        playerPos.GetComponentInChildren<CinemachineCamera>().gameObject.layer = layerToAdd;

        //adding the layer
        playerPos.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
        */
    }
}
