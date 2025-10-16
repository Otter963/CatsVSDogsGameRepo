using UnityEngine;

public class BasicCameraFollowScript : MonoBehaviour
{
    [SerializeField] private float camFollowSpeed = 0.1f;

    [SerializeField] private Vector3 camOffset;

    [SerializeField] private GameObject player;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + camOffset, camFollowSpeed);
    }
}
