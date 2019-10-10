using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : InventoryItems
{
    public WeaponType weaponType;
    public Hands hands;
    public float minDamage, maxDamage;
    public int bulletPerShot;
    public Sprite icon;
}
