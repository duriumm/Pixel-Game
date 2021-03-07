using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string eqName;
    public string description;
    public int damage;
    public int defense;
    public int durability;
    public int value;
    public Sprite itemIcon;

    [SerializeField]
    private AnimationClip[] animationClips;
    public enum EQUIPMENT_TYPE
    {
        HELMET,
        ARMOR,
        PANTS,
        BOOTS,
        SWORD,
        AXE,
        SPEAR,
        SHIELD,
        NECKLACE,
        RING
    }
    public Equipment(string eqName, string description, int damage, int defense, int durability, int value)
    {
        this.eqName = eqName;
        this.description = description;
        this.damage = damage;
        this.defense = defense;
        this.durability = durability;
        this.value = value;
    }

    public AnimationClip[] MyAnimationClips
    {
        get
        {
            return animationClips;
        }
    }
}
