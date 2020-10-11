using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInteractable : InteractableBase
{
    public override bool Interact(PlayerBase player)
    {
        if (!base.Interact(player)) return false;
        VFX();
        return true;
    }
}
