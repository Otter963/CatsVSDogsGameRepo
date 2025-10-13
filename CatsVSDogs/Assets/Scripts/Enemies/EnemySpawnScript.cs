using UnityEngine;
using Unity.Netcode;

public class EnemySpawnScript : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public Vector3 spawnPosition = Vector3.zero;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        //anything in start must be moved here
    }

    private void Update()
    {
        if (!IsServer) return;

        //checking if something is happening
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            NetworkObject networkObject = enemyInstance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.Spawn(true);
            }
            else
            {
                Debug.LogWarning("Enemy prefab doesn't have a networkobject component!");
                Destroy(enemyInstance);
            }
        }
        else
        {
            Debug.LogWarning("Enemy prefab not assigned in the inspector");
        }
    }
}
