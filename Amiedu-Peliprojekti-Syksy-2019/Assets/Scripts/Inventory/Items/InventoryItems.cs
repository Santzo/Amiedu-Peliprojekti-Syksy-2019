using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class InventoryItems : ScriptableObject
{
    public int itemLevel;
    public float weight;
    public ItemType itemType;
    public Sprite icon;
    public Color colorTint = new Color(1f,1f,1f,1f);
    public float iconScale = 1;
    [TextArea]
    public string description;
}

[System.Serializable]
public class ItemEffect
{
    [Tooltip("Individual effect of the item")]
    public Effect effect;
    [Tooltip("The total amount of points for the effect")]
    public float amount;
    [Tooltip("How long the effect lasts")]
    public float duration;
}

public enum Hands
{
    One_handed,
    Two_handed
}
public enum ItemType
{
    Weapon,
    Headgear,
    Chestpiece,
    Legguard,
    Armguard,
    Consumable

}
public enum WeaponType
{
    Melee,
    Pistol,
    Rifle,
    Shotgun
}
public enum Effect
{
    Heals,
    Raises_strength,
    Raises_constitution,
    Raises_dexterity,
    Raises_adaptability,
    Raises_luck,

}
