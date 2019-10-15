using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ItemDetails : MonoBehaviour
{
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemType;
    private TextMeshProUGUI itemDescription;
    private Transform info;
    private TextMeshProUGUI[] details;
    private Animator anim;

    public void Awake()
    {
        itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemType = transform.Find("ItemType").GetComponent<TextMeshProUGUI>();
        itemDescription = transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        info = transform.Find("Info");

        details = new TextMeshProUGUI[info.childCount];
        for (int i = 0; i < info.childCount; i++)
        {
            details[i] = info.GetChild(i).GetComponent<TextMeshProUGUI>();
            details[i].text = "";
        }
    }



    public void DisplayDetails(InventoryItems item)
    {
        itemName.text = item.name;
        itemDescription.text = item.description;
        itemType.text = item.itemType.ToString() + " - Level " + item.itemLevel;

        if (item is Weapon)  // WEAPONS HERE
        {
            Weapon _item = item as Weapon;
            itemType.text = _item.hands.ToString().Replace("_", " ") + " " + _item.weaponType + " - Level " + item.itemLevel;
            details[0].text = "Damage " + TextColor.Return("yellow") + _item.minDamage + " - " + _item.maxDamage;
            details[2].text = "Critical hit chance " + TextColor.Return("yellow") + _item.criticalHitChance + TextColor.Return() + "%";
            if (_item.weaponType == WeaponType.Melee)
            {
                details[1].text = "Weapon swings " + TextColor.Return("yellow") + _item.fireRate + TextColor.Return() + " times per second";

            }
            else
            {
                details[1].text = "Weapon fires " + TextColor.Return("yellow") + _item.fireRate + TextColor.Return() + " times per second";
                details[3].text = "Clip size " + TextColor.Return("yellow") + _item.clipSize;
                details[4].text = "Reload time <color=yellow>" + _item.reloadTime + "<color=white> seconds";
                details[5].text = "Weapon shoots <color=yellow>" + _item.bulletPerShot + "<color=white> bullets per shot";
            }
        }

        else if (item is Consumable) // CONSUMABLES HERE
        {
            Consumable _item = item as Consumable;
            for (int i = 0; i < _item.itemEffect.Length; i++)
            {
                details[i].text = ItemEffectText(_item.itemEffect[i]);
            }
        }
    }

    private void OnEnable()
    {
        details.EmptyTextArray();
    }


    private string ItemEffectText(ItemEffect effect)
    {
        switch (effect.effect)
        {
            case Effect.Heals:
                return effect.duration > 0
                ? effect.effect.ToString().Replace("_", " ") + " you by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + " points in " +
                TextColor.Return("yellow") + effect.duration + TextColor.Return() + " seconds."
                : effect.effect.ToString().Replace("_", " ") + " you by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + " points " + TextColor.Return("yellow") + "immediately"
                + TextColor.Return() + ".";
            default:
                {
                    string name = effect.effect.ToString();
                    int pos = name.IndexOf("_");
                    string eff = name.Substring(0, pos);
                    string attribute = name.Substring(pos + 1, name.Length - pos - 1);
                    return effect.duration > 0
                    ? eff + TextColor.Return("green") + " " + attribute + TextColor.Return() + " by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + " points for " +
                    TextColor.Return("yellow") + effect.duration + TextColor.Return() + " seconds."
                    : eff + TextColor.Return("green") + " " + attribute + TextColor.Return() + " by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + " points " +
                    TextColor.Return("green") + "permanently" + TextColor.Return() + ".";
                }
        }
    }

}
