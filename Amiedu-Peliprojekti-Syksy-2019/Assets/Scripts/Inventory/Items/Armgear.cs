using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Armgear")]
public class Armgear : InventoryItems
{
    public float defense;
    public float spectralDefense;
    public float fireDefense;
    public GearEffect[] gearEffects;
}