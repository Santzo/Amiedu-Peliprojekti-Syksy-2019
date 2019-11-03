using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class InventoryItems : ScriptableObject
{
    public int itemLevel;
    public float weight;
    public GameObject obj;
    [Tooltip("Optional sprite if you want the inventory icon to look different")]
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
    [Tooltip("How long the effect lasts (Leave this at 0 if you want the effect to be immediate / permanent)")]
    public float duration;
}
[System.Serializable]
public class GearEffect
{
    [Tooltip("Individual effect of the item")]
    public _GearEffect effect;
    [Tooltip("The total amount of points for the effect")]
    public float amount;
}

public enum Hands
{
    One_handed,
    Two_handed
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
    Raises_Strength,
    Raises_Constitution,
    Raises_Dexterity,
    Raises_Adaptability,
    Raises_Luck,
}
public enum _GearEffect
{
    Movement_Speed,
    Light_Radius,
    Increases_Health,
    Increases_Stamina,
    Increases_Strength,
    Increases_Constitution,
    Increases_Dexterity,
    Increases_Adaptability,
    Increases_Luck
}
