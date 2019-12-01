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
    private List<Transform> uitems = new List<Transform>();
    private void Awake()
    {
        details = GetComponentsInChildren<TextMeshProUGUI>();
        images = GetComponentsInChildren<Image>();
        uitems.SimpleUIHandlerInitialize(transform);
    }

    void Start() => UpdateStats();
    
    private void OnEnable()
    {
        foreach (var image in images)
             image.enabled = false;
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
        details[3].text = $"Ammo {TextColor.Return("yellow")}Pistol {TextColor.Return("green")}{CharacterStats.pistolAmmo} {TextColor.Return("yellow")} - Rifle {TextColor.Return("green")}{CharacterStats.rifleAmmo}{TextColor.Return("yellow")} - Gas {TextColor.Return("green")}{CharacterStats.gasAmmo}";
        details[4].text = $"Total Defense {TextColor.Return("yellow")}{CharacterStats.totalPhysicalDefense}{TextColor.Return()} / {TextColor.Return("purple")}{CharacterStats.totalSpectralDefense}{TextColor.Return()} / {TextColor.Return("red")}{CharacterStats.totalFireDefense}";
        details[5].text = $"Sight Radius {TextColor.Return("green")}{Mathf.Round(Info.SightRadius * 10f) / 10f} {TextColor.Return()}({TextColor.Return("yellow")}{SightRadiusDesc(Info.SightRadius)}{TextColor.Return()})";
        details[6].text = $"Movement Speed {TextColor.Return("green")}{Mathf.Round(CharacterStats.moveSpeed * 10f) / 10f} {TextColor.Return()}({TextColor.Return("yellow")}{MovementSpeedDesc(CharacterStats.moveSpeed)}{TextColor.Return()})";
        details[7].text = $"Strength {TextColor.Return("green")}{CharacterStats.strength}";
        details[8].text = $"Dexterity {TextColor.Return("green")}{CharacterStats.dexterity}";
        details[9].text = $"Constitution {TextColor.Return("green")}{CharacterStats.constitution}";
        details[10].text = $"Perception {TextColor.Return("green")}{CharacterStats.perception}";
        details[11].text = $"Luck {TextColor.Return("green")}{CharacterStats.luck}";
        
        if (curWep != null)
        {
            details[0].text = $"Total damage {TextColor.Return("green")}{Info.StatsMinDamage}{TextColor.Return()}-{TextColor.Return("green")}{Info.StatsMaxDamage}";
            details[1].text = $"Total critical hit chance {TextColor.Return("green")}{Mathf.Round(Info.totalCriticalHitChance * 10f) / 10f}{TextColor.Return()}%.";
            details[2].text = $"Atk speed {TextColor.Return("green")}{Mathf.Round(Info.totalAttackSpeed * 100f) / 100f}{TextColor.Return()} times per second.";
        }
    }

    string SightRadiusDesc(float speed)
    {
        if (speed > 5.5f) return "Great";
        if (speed > 4.5f) return "Good";
        if (speed > 3f) return "Pretty Good";
        if (speed > 2f) return "Normal";
        if (speed > 1f) return "Limited";
        return "Pitch Black";
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
        if (CharacterStats.characterEquipment.weapon == null && index < 3) return;
        images[index].enabled = true;
        Vector2 pos = transform.GetChild(index).position;
        obj = ObjectPooler.op.SpawnUI("StatDescription", new Vector2(pos.x, pos.y + 3.5f), transform.parent.parent);
        StatDescription _text = obj.GetComponent<StatDescription>();
        switch (index)
        {
            case 0:
                _text.UpdateText("Total Damage", "Total damage describes how much total damage you will deal with a single swing or a shot of your weapon. This number includes all bonuses from your attributes and items.");
                break;
            case 1:
                _text.UpdateText("Total Critical Hit Chance", "Total critical hit chance of your attacks. This number includes all positive and negative effects from your items and attributes.");
                break;
            case 2:
                _text.UpdateText("Total Attack Speed", "The amount of times your attack can deal damage per second. This number includes all positive and negative effects from your items and attributes.");
                break;
            case 3:
                _text.UpdateText("Total Ammunition", $"Pistol ammo: {TextColor.Return("yellow")}{CharacterStats.pistolAmmo}\n{TextColor.Return()}Rifle / Shotgun ammo: {TextColor.Return("yellow")}{CharacterStats.rifleAmmo}{TextColor.Return()}\nFlamethrower gas: {TextColor.Return("yellow")}{CharacterStats.gasAmmo}");
                break;
            case 4:
                _text.UpdateText("Total Defense", $"Total combined defense value from all your gear and items. \n\n{TextColor.Return("yellow")}Physical Defense {CharacterStats.totalPhysicalDefense}\n{TextColor.Return("purple")}Spectral Defense {CharacterStats.totalSpectralDefense}\n{TextColor.Return("red")}Fire Defense {CharacterStats.totalFireDefense}");
                break;
            case 5:
                _text.UpdateText("Total Sight Radius", "Total sight radius of your character. This number includes all positive and negative effects from your items and attributes.");
                break;
            case 6:
                _text.UpdateText("Movement Speed", "Movement speed tells you the current movement speed of your character. This number includes all positive and negative effects from your items and attributes.");
                break;
            case 7:
                _text.UpdateText("Strength", $"Strength affects how much damage you deal with {TextColor.Return("green")}melee{TextColor.Return()} weapons. Strength also {TextColor.Return("green")}increases{TextColor.Return()} your carrying capacity. It also {TextColor.Return("yellow")}slightly increases{TextColor.Return()} your maximum health and stamina.");
                break;
            case 8:
                _text.UpdateText("Dexterity", $"Dexterity affects how much damage you deal with {TextColor.Return("green")}ranged{TextColor.Return()} weapons. Dexterity also {TextColor.Return("green")}increases{TextColor.Return()} your attack speed with all weapons.");
                break;
            case 9:
                _text.UpdateText("Constitution", $"Constitution {TextColor.Return("green")}heavily increases{TextColor.Return()} both your maximum health and stamina. It also {TextColor.Return("yellow")}slightly increases{TextColor.Return()} your carrying capacity."); 
                break;
            case 10:
                _text.UpdateText("Perception", $"Perception {TextColor.Return("green")}heavily increases{TextColor.Return()} your chances of scoring a critical hit with any weapon. It also {TextColor.Return("yellow")}slightly increases{TextColor.Return()} your sight range.");
                break;
            case 11:
                _text.UpdateText("Luck", $"Luck {TextColor.Return("green")}increases{TextColor.Return()} your chances of finding weapons, armor and other items. It also {TextColor.Return("yellow")}very slightly increases{TextColor.Return()} your damage and critical hit chance.");
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
