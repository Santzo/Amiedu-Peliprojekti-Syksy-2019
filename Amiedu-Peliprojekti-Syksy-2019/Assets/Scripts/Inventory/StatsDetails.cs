using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsDetails : MonoBehaviour, IUIHandler
{
    private TextMeshProUGUI[] details;
    private Image[] images;
    private GameObject obj;

    private void Awake()
    {
        details = GetComponentsInChildren<TextMeshProUGUI>();
        images = GetComponentsInChildren<Image>();
        UpdateStats(); 
    }

    private void OnEnable()
    {
        foreach (var image in images)
            image.enabled = false;
        UpdateStats();
    }
    void OnDisable()
    {
        if (obj != null) ObjectPooler.op.DeSpawn(obj);
    }

    public void UpdateStats()
    {
        Weapon curWep = CharacterStats.characterEquipment.weapon;
        for (int i = 0; i < details.Length; i++)
        {
            details[i].text = "";
        }
        details[0].text = "No weapon equipped";
        details[6].text = $"Movement Speed: {TextColor.Return("green")}{Mathf.Round(CharacterStats.moveSpeed * 10f) / 10f} {TextColor.Return()}({TextColor.Return("yellow")}{MovementSpeedDesc(CharacterStats.moveSpeed)}{TextColor.Return()})";
        details[7].text = $"Strength {TextColor.Return("green")}{CharacterStats.strength}";
        details[8].text = $"Dexterity {TextColor.Return("green")}{CharacterStats.dexterity}";
        details[9].text = $"Constitution {TextColor.Return("green")}{CharacterStats.constitution}";
        details[10].text = $"Adaptability {TextColor.Return("green")}{CharacterStats.adaptability}";
        details[11].text = $"Luck {TextColor.Return("green")}{CharacterStats.luck}";
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

    public void EntryEnter(int index)
    {
        images[index].enabled = true;
        Vector2 pos = transform.GetChild(index).position;
        obj = ObjectPooler.op.SpawnUI("StatDescription", new Vector2(pos.x, pos.y + 3.5f), transform.parent.parent);
        StatDescription _text = obj.GetComponent<StatDescription>();
        switch (index)
        {
            case 0:
                _text.UpdateText("Total Damage", "Total damage describes how much total damage you will deal with a single swing or a shot of your weapon. This number includes all bonuses from your attributes and items.");
                break;
            case 6:
                _text.UpdateText("Movement Speed", "Movement speed tells you the current movement speed of your character. This number includes all positive and negative effects from your items.");
                break;
            case 7:
                _text.UpdateText("Strength", $"Strength affects how much damage you deal with {TextColor.Return("green")}melee{TextColor.Return()} weapons. Strength also {TextColor.Return("green")}increases{TextColor.Return()} your carrying capacity. It also {TextColor.Return("yellow")}slightly increases{TextColor.Return()} your maximum health and stamina.");
                break;
            case 8:
                _text.UpdateText("Dexterity", $"Dexterity affects how much damage you deal with {TextColor.Return("green")}ranged{TextColor.Return()} weapons. Dexterity also {TextColor.Return("green")}increases{TextColor.Return()} your attack speed with all weapons. It also {TextColor.Return("yellow")}very slightly increases{TextColor.Return()} your critical hit chances.");
                break;
            case 9:
                _text.UpdateText("Constitution", $"Constitution {TextColor.Return("green")}heavily increases{TextColor.Return()} both your maximum health and stamina. It also {TextColor.Return("yellow")}slightly increases{TextColor.Return()} your carrying capacity."); 
                break;
            default:
                _text.UpdateText("Blaa", "Blaa");
                break;   
        }
    }

    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        
    }

    public void EntryLeave(int index)
    {
        images[index].enabled = false;
        if (obj != null) ObjectPooler.op.DeSpawn(obj);
    }
}
