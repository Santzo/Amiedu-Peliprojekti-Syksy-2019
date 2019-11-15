using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chestgear")]
public class Chestgear : InventoryItems
{
    public float defense;
    public float spectralDefense;
    public float fireDefense;
    public GearEffect[] gearEffects;
}

