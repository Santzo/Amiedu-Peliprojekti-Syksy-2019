using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ItemDetails : MonoBehaviour
{
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemType;
    private TextMeshProUGUI equipped;
    private TextMeshProUGUI itemDescription;
    private Transform info;
    private TextMeshProUGUI[] details;
    private Animator anim;

    public void Awake()
    {
        itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemType = transform.Find("ItemType").GetComponent<TextMeshProUGUI>();
        equipped = transform.Find("Equipped").GetComponent<TextMeshProUGUI>();
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



    public void DisplayDetails(InventoryItems item, bool _equipped = false)
    {
        itemName.text = item.name;
        itemDescription.text = item.description;
        itemType.text = item.GetType().ToString() + " - Level " + item.itemLevel;
        equipped.enabled = _equipped;

        if (item is Weapon)  // WEAPONS HERE
        {
            Debug.Log("RAA");
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
            if (_item.gearEffects != null && _item.gearEffects.Length > 0)
            {
                for (int i = 0; i < _item.gearEffects.Length; i++)
                {
                    details[i + 6].text = GearEffectText(_item.gearEffects[i]);
                }
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
        Events.onItemDragStart += () => gameObject.SetActive(false);
        details.EmptyTextArray();
    }

    private void OnDisable()
    {
        Events.onItemDragStart -= () => gameObject.SetActive(false);
    }

    private string GearEffectText(GearEffect effect)
    {
        ReturnEffectAndAttribute(effect.effect.ToString(), out string eff, out string attribute);
        switch (effect.effect)
        {
            case _GearEffect.Movement_Speed:
                return effect.amount > 0
                ? TextColor.Return("green") + "Increases " + TextColor.Return() + "movement speed by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + "%."
                : TextColor.Return("red") + "Decreases " + TextColor.Return() + "movement speed by " + TextColor.Return("yellow") + Mathf.Abs(effect.amount) + TextColor.Return() + "%.";
            default:
                return effect.amount > 0
                ? eff + TextColor.Return("green") + " " + attribute + TextColor.Return() + " by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + " points."
                : "Decreases" + TextColor.Return("red") + " " + attribute + TextColor.Return() + " by " + TextColor.Return("yellow") + Mathf.Abs(effect.amount) + TextColor.Return() + " points.";
        }
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
                    ReturnEffectAndAttribute(effect.effect.ToString(), out string eff, out string attribute);
                    return effect.duration > 0
                    ? eff + TextColor.Return("green") + " " + attribute + TextColor.Return() + " by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + " points for " +
                    TextColor.Return("yellow") + effect.duration + TextColor.Return() + " seconds."
                    : eff + TextColor.Return("green") + " " + attribute + TextColor.Return() + " by " + TextColor.Return("yellow") + effect.amount + TextColor.Return() + " points " +
                    TextColor.Return("green") + "permanently" + TextColor.Return() + ".";
                }
        }
    }
    private void ReturnEffectAndAttribute(string wholeName, out string effect, out string attribute)
    {
        int pos = wholeName.IndexOf("_");
        effect = wholeName.Substring(0, pos);
        attribute = wholeName.Substring(pos + 1, wholeName.Length - pos - 1);
    }

}
