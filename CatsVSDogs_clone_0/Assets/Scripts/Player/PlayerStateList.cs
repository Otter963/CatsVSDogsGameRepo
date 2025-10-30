using UnityEngine;
using Unity.Netcode;

public class PlayerStateList : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    public bool isJumping = false;
    public bool isDashing = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight;
    public bool invincible;
}
