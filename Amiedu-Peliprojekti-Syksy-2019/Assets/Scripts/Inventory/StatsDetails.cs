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
        for (int i = 0; i < details.Length; i++)
        {
            details[i].text = "";
        }
        details[0].text = "No weapon equipped";
   
        if (curWep != null)
        {
            details[0].text = curWep.weaponType == WeaponType.Melee ? "Total damage per swing " : "Total damage per shot ";
            details[0].text += TextColor.Return("green") + Info.StatsMinDamage + TextColor.Return() + " - " + TextColor.Return("green") + Info.StatsMaxDamage;
            //details[1].text = curWep.weaponType == WeaponType.Melee ? ""
        }
        
    }
}
