using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroEnemyHitbox : Hitbox
{
    private void Awake()
    {
        damage = baseDamage;
        OnHit.AddListener(OnHitboxEntered);
    }

    private void OnHitboxEntered()
    {
        PlayerBase player = PlayerBase.instance;
        if (player == null)
            return;

        EnemyGroup currentGroup = player.currentRoom.GetComponentInChildren<EnemyGroup>();
        if(currentGroup == null)
        {
            Debug.Log("Could not find this room's EnemyGroup to agro enemies");
            return;
        }

        Debug.Log("AggroEnemyHitbox triggered!");
        currentGroup.TurnGroupAggressive.Invoke();

    }
}
