using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leggear")]
public class Leggear : InventoryItems
{
    public float defense;
    public float spectralDefense;
    public float fireDefense;
    public GearEffect[] gearEffects;
}
