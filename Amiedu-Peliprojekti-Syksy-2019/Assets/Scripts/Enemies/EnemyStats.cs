using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats
{
    public string name;
    public float health;
    public float moveSpeed;

    public static EnemyStats[] enemyStats = new EnemyStats[] {
        new EnemyStats { name = "Zombie", health = 20f, moveSpeed = 2.5f } };
}

