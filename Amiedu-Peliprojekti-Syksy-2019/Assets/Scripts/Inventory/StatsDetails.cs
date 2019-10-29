using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsDetails : MonoBehaviour
{
    public TextMeshProUGUI[] details;

    private void Awake()
    {
        details = GetComponentsInChildren<TextMeshProUGUI>();
        UpdateStats(); 
    }


    public void UpdateStats()
    {
        Weapon curWep = CharacterStats.characterEquipment.weapon;
        string damageText = "No weapon equipped";
        if (curWep != null)
        {
            damageText = curWep.weaponType == WeaponType.Melee ? "Total damage per swing " : "Total damage per shot ";
            damageText += TextColor.Return("green") + Info.MinDamage + TextColor.Return() + " - " + TextColor.Return("green") + Info.MaxDamage;
        }
        details[0].text = damageText;
    }
}
