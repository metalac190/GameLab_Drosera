using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectInfo : MonoBehaviour
{
    // general
    [SerializeField] bool isLocked;
    public bool IsLocked
    {
        get => isLocked;
        set => IsLocked = true;
    }

    [SerializeField] Sprite backgroundImage;
    public Sprite BackgroundImage
    {
        get => backgroundImage;
    }

    [SerializeField] Sprite lockedImage;
    public Sprite LockedImage
    {
        get => lockedImage;
    }

    // character
    [SerializeField] Sprite characterImage;
    public Sprite CharacterImage
    {
        get => characterImage;
        set => characterImage = value;
    }

    [SerializeField] int health;
    public int Health
    {
        get => health;
        set => health = value;
    }

    [SerializeField] string damage;
    public string Damage
    {
        get => damage;
        set => damage = value;
    }

    // weapon
    [SerializeField] string[] weaponName;
    public string[] WeaponName
    {
        get => weaponName;
        set => weaponName = value;
    }

    [SerializeField] string[] speed;
    public string[] Speed
    {
        get => speed;
        set => speed = value;
    }

    [SerializeField] int[] ammo;
    public int[] Ammo
    {
        get => ammo;
        set => ammo = value;
    }
}
