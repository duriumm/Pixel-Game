using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WORK WITH THIS FOR CUSTOM WEAPONS
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class WeaponData : ScriptableObject
{
    public new string name;
    public string description;
    public SpriteRenderer weaponsGraphics;
    public int damage;
    public int value;
    public int durability;
}
