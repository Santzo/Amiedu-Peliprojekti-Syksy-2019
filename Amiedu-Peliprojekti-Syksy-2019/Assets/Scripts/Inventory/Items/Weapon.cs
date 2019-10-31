using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : InventoryItems
{
    public WeaponType weaponType;
    public AnimationClip attackAnimation;
    public Hands hands;
    public float minDamage, maxDamage;
    [Tooltip("How many shots / swings (melee) per second.")]
    public float fireRate;
    public int clipSize;
    [Tooltip("Reload time in seconds")]
    public float reloadTime;
    [Range(0, 100)]
    public float criticalHitChance;
    public int bulletPerShot;
    public ItemEffect[] itemEffects;


}
