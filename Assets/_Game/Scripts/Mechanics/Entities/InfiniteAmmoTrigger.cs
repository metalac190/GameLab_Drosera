using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteAmmoTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Gunner player = other.GetComponent<Gunner>();
        if(player)
        {
            player.SetInfiniteAmmo(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Gunner player = other.GetComponent<Gunner>();
        if (player)
        {
            player.SetInfiniteAmmo(false);
        }
    }
}
