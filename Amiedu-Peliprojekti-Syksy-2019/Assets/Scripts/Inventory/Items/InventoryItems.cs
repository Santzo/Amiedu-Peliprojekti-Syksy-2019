using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class InventoryItems : ScriptableObject
{
    public int itemLevel;
    public float weight;
    public ItemType itemType;

    [TextArea]
    public string description;
}



public enum Hands
{
    OneHanded,
    TwoHanded
}
public enum ItemType
{
    Weapon,
    Head,
    Torso,
    Legs,
    Arms

}
public enum WeaponType
{
    Melee,
    Pistol,
    Rifle,
    Shotgun
}
