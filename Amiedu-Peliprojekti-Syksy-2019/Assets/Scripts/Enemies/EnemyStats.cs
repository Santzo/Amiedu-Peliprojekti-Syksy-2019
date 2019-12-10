using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public float health;
    public float moveSpeed;
    public float physicalDefense;
    public float fireDefense;
    public float spectralDefense;
    public float hearingRange;
    [Tooltip("How long the enemy follows after losing LOS")]
    public float followTime;
    public EnemyDamage[] damage;

}

