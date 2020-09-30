using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : PlayerBase
{
    GunnerPrimaryFire _primaryFire;
    GunnerAltFire _altFire;
    GunnerGrenade _grenade;
    GunnerDOTGrenade _dotGrenade;

    new void Awake()
    {
        base.Awake();
        _primaryFire = GetComponent<GunnerPrimaryFire>();
        _altFire = GetComponent<GunnerAltFire>();
        //_grenade = GetComponent<GunnerGrenade>();
        //_dotGrenade = GetComponent<GunnerDOTGrenade>();
    }

    protected override void Attacking()
    {
        if (ammo > 0) //have ammo
        {
            if ((shootButtonKey || shootButtonGamepad == 1) && !altFireButton)
            {
                _primaryFire.Fire();
            }
            else if (altFireButton)
            {
                _altFire.Fire();
            }
            currentState = PlayerState.Neutral;
        }
        else //no ammo
        {
            currentState = PlayerState.Reloading;
        }
    }
}
