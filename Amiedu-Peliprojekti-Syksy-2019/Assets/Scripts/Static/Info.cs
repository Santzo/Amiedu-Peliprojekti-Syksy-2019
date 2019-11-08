using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info
{
    private static Canvas canvas = GameObject.Find("UIOverlay").GetComponent<Canvas>();
    public static Camera camera = Camera.main;
    public static GameObject player = GameObject.Find("Player");
    public static float CanvasScale { get { return canvas.scaleFactor; } }
    public static Transform content;
    public static PlayerEquipment playerEquipment = GameObject.Find("PlayerEquipment").GetComponent<PlayerEquipment>();
    public static float minDamage { get; private set; }
    public static float maxDamage { get; private set; }


    public static int SortingOrder(float yPos)
    {
        yPos *= 100;
        return Mathf.RoundToInt(-yPos);
    }

    public static int StatsMinDamage
    {
        get
        {
            float dmg = CharacterStats.characterEquipment.weapon != null ? (int)CharacterStats.characterEquipment.weapon.minDamage : 0;
            float statBonus = CharacterStats.dexterity * 0.05f;
            dmg += statBonus;
            minDamage = dmg;
            return Mathf.RoundToInt(dmg);
        }
    }
    public static int StatsMaxDamage
    {
        get
        {
            float dmg = CharacterStats.characterEquipment.weapon != null ? (int)CharacterStats.characterEquipment.weapon.maxDamage : 0;
            float statBonus = CharacterStats.dexterity * 0.05f;
            dmg += statBonus;
            maxDamage = dmg;
            return Mathf.RoundToInt(dmg);
        }
    }
    public static float CalculateDamage(EnemyStats enemyStats)
    {
        float dmg = Random.Range(minDamage, maxDamage);
        float defense = enemyStats.defense;

        float totalDmg = dmg - defense > 1f ? dmg - defense : 1f;
        return totalDmg;
    }
}
