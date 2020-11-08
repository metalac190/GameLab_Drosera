using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGroup : MonoBehaviour {

    public UnityEvent OnEnemyDamage, OnShotFired, GrabHyperseed;
    public UnityEvent TurnGroupAggressive, TurnGroupPassive;
    public UnityEvent OnPlayerEnter, OnPlayerExit;

    private void Awake() {
        AddDebugCalls();

        EnemyBase[] enemies = GetComponentsInChildren<EnemyBase>(true);
        foreach(EnemyBase enemy in enemies) {
            // When hyperseed is grabbed - all enemies turn aggressive
            GrabHyperseed.AddListener(() => {
                enemy.TurnAggressiveHyperseed.Invoke();
            });
            // Testing - all enemies turn aggressive
            TurnGroupAggressive.AddListener(() => {
                enemy.TurnAggressive.Invoke();
            });
            // Testing - all enemies turn passive
            TurnGroupPassive.AddListener(() => {
                enemy.ForceIdle();
            });

            // Stop/resume aggro when player exits/enters a room
            OnPlayerExit.AddListener(() => {
                enemy.ForceIdle();
            });
            OnPlayerEnter.AddListener(() => {
                enemy.ResetEnemy();
            });
        }

        // Scurriers aggro in a group when one is damaged
        enemies = GetComponentsInChildren<Scurrier>(true);
        foreach(Scurrier enemy in enemies) {
            OnEnemyDamage.AddListener(() => {
                enemy.TurnAggressive.Invoke();
            });
        }

        // Brawlers all aggro when an attack/ability is used in the room
        enemies = GetComponentsInChildren<Brawler>(true);
        foreach(Brawler enemy in enemies) {
            OnShotFired.AddListener(() => {
                enemy.TurnAggressive.Invoke();
            });
        }

        // Remove listeners after events are called to prevent chain-calling
        // Once enemies turn aggressive they stay that way for ever, so no need for re-calling anyways
        GrabHyperseed.AddListener(() => {
            OnEnemyDamage.RemoveAllListeners();
            OnShotFired.RemoveAllListeners();
        });
        OnEnemyDamage.AddListener(() => {
            OnEnemyDamage.RemoveAllListeners();
        });
        OnShotFired.AddListener(() => {
            OnShotFired.RemoveAllListeners();
        });
    }

    private void AddDebugCalls() {
        OnShotFired.AddListener(() => {
            Debug.Log("Shot fired detected in " + transform.parent.parent.name);
        });
        OnEnemyDamage.AddListener(() => {
            Debug.Log("Enemy damaged detected in " + transform.parent.parent.name);
        });
    }

}
