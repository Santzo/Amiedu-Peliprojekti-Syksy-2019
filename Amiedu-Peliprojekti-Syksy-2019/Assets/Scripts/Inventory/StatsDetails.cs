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

    private void OnEnable()
    {
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
        details[6].text = $"Movement Speed {TextColor.Return("green")}{Mathf.Round(CharacterStats.moveSpeed * 10f) / 10f} {TextColor.Return()}({TextColor.Return("yellow")}{MovementSpeedDesc(CharacterStats.moveSpeed)}{TextColor.Return()})";
        details[7].text = $"Strength: {TextColor.Return("green")}{CharacterStats.strength}";
        if (curWep != null)
        {
            details[0].text = curWep.weaponType == WeaponType.Melee ? "Total damage per swing " : "Total damage per shot ";
            details[0].text += TextColor.Return("green") + Info.StatsMinDamage + TextColor.Return() + " - " + TextColor.Return("green") + Info.StatsMaxDamage;
            //details[1].text = curWep.weaponType == WeaponType.Melee ? ""
        }
        
    }
    string MovementSpeedDesc(float speed)
    {
        if (speed > 10f) return "Sonic speed";
        if (speed > 9f) return "Extremely fast";
        if (speed > 8f) return "Really fast";
        if (speed > 7f) return "Fast";
        if (speed > 5.5f) return "Faster";
        if (speed > 4.5f) return "Normal";
        if (speed > 3f) return "Slower";
        if (speed > 2f) return "Quite slow";
        if (speed > 1f) return "Extremely slow";
        return "Snail speed";
     }
}
