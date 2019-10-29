using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info
{
    private static Canvas canvas = GameObject.Find("UI").GetComponent<Canvas>();
    public static Camera camera = Camera.main;
    public static GameObject player = GameObject.Find("Player");
    public static float CanvasScale { get { return canvas.scaleFactor; } }
    public static Transform content; 
    public static PlayerEquipment playerEquipment = GameObject.Find("PlayerEquipment").GetComponent<PlayerEquipment>();

    public static int SortingOrder(float yPos)
    {
        yPos *= 100;
        return Mathf.RoundToInt(-yPos);
    }

    public static int MinDamage
    {
        get
        {
            int dmg = CharacterStats.characterEquipment.weapon != null ? (int)CharacterStats.characterEquipment.weapon.minDamage : 0;
            return dmg;
        }
    }
    public static int MaxDamage
    {
        get
        {
            int dmg = CharacterStats.characterEquipment.weapon != null ? (int)CharacterStats.characterEquipment.weapon.maxDamage : 0;
            return dmg;
        }
    }
}
