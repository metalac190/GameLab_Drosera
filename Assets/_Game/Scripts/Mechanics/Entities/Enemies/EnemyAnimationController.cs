using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour {
    
    private EnemyBase enemyBase;

    public List<GameObject> hitboxes;
    public List<TrailRenderer> trails;

    private void Awake() {
        enemyBase = GetComponentInParent<EnemyBase>();
    }

    public void SetAttackDone() {
        enemyBase.attackDone = true;
    }

    public void EnableHitbox(int index) {
        hitboxes[index].SetActive(true);
    }

    public void DisableHitbox(int index) {
        hitboxes[index].SetActive(false);
    }

    public void EnableTrail(int index) {
        trails[index].emitting = true;
    }

    public void DisableTrail(int index) {
        trails[index].emitting = false;
    }

}