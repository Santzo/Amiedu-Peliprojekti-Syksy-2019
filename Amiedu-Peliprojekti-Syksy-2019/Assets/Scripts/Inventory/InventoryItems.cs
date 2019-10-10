using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class InventoryItems : ScriptableObject
{
    public float weight;
    public ItemType itemType;
    [TextArea]
    public string description;
}

[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : InventoryItems
{
    public WeaponType weaponType;
    public Hands hands;
    public float minDamage, maxDamage;
    public Sprite icon;
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
