using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : InventoryItems
{
    public WeaponType weaponType;
    public AnimationClip attackAnimation;
    public Hands hands;
    public float physicalMin, physicalMax, fireMin, fireMax, spectralMin, spectralMax;
    [Tooltip("How many shots / swings (melee) per second.")]
    public float attackRate;
    public float staminaCost;
    [Range(0, 100)]
    public float criticalHitChance;
    public int bulletPerShot;
    public GearEffect[] gearEffects;
}
