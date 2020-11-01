using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour {
    
    private EnemyBase enemyBase;

    private void Awake() {
        enemyBase = GetComponentInParent<EnemyBase>();
    }

    public void SetAttackDone() {
        enemyBase.attackDone = true;
    }

}