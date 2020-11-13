using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class InteractableBase : MonoBehaviour
{
    [Header("Generic Interactable")]
    [Tooltip("For additional triggers to happen when this is interacted with")]
    public UnityEvent OnInteract;
    public UnityEvent OnApproach;
    public UnityEvent OnLeave;

    [SerializeField]
    protected int _maxUses = 1;
    protected int _uses;

    [Header("VFX")]
    [SerializeField]
    protected GameObject effect;
    [SerializeField]
    protected float effectDuration;

    private void Awake()
    {
        _uses = _maxUses;
    }

    public virtual bool Interact(PlayerBase player)
    {
        if (_uses <= 0) return false;

        Debug.Log(player.name + " interacted with " + name);
        OnInteract?.Invoke();
        _uses--;

        return true;
    }

    /// <summary>
    /// Deactivate the gameobject after time seconds
    /// </summary>
    /// <param name="time"></param>
    public void DelayedDeactivate(float time)
    {
        StartCoroutine(Deactivate(time));
    }

    private IEnumerator Deactivate(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    protected virtual void VFX()
    {
        if (effect != null && VFXSpawner.vfx != null)
            VFXSpawner.vfx.SpawnVFX(effect, effectDuration, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_uses > 0) OnApproach?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerBase>() == null) return;
        other.GetComponent<PlayerBase>().InteractTarget = this;
        if (other.GetComponent<PlayerBase>().InteractTarget == null)
        {
            other.GetComponent<PlayerBase>().InteractTarget = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerBase>() == null) return;
        other.GetComponent<PlayerBase>().InteractTarget = null;
        if (_uses > 0) OnLeave.Invoke();
    }
}
