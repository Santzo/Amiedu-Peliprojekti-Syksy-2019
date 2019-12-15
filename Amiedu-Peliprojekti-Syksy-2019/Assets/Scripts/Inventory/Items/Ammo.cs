using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Ammo")]
public class Ammo: InventoryItems
{
    public AmmoType ammoType;
    public enum AmmoType
    {
        Pistol,
        Rifle,
        Gasoline
    }
}
