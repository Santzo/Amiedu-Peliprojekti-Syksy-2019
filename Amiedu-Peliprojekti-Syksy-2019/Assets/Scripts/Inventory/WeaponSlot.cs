using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    Image icon;
    TextMeshProUGUI weaponName, weaponAmmo;

    private void Awake()
    {
        icon = transform.Find("WeaponIcon").GetComponent<Image>();
        weaponName = transform.Find("WeaponName").GetComponent<TextMeshProUGUI>();
        weaponAmmo = transform.Find("WeaponAmmo").GetComponent<TextMeshProUGUI>();
        icon.preserveAspect = true;
        UpdateWeaponSlot();
    }

    public void UpdateWeaponSlot()
    {
        Weapon temp = CharacterStats.characterEquipment.weapon;
        weaponName.text = temp != null ? temp.name : "No weapon equipped";
        weaponAmmo.text = temp != null ? WeaponAmmoText(temp) : "";
        icon.enabled = temp != null;
        if (temp != null)
        {
            Weapon wep = CharacterStats.characterEquipment.weapon;
            icon.sprite = wep.icon != null
                ? wep.icon
                : wep.obj.GetComponent<SpriteRenderer>().sprite;
            icon.material = wep.modifiedMat != null ? wep.modifiedMat : wep.material != null ? wep.material : wep.obj.GetComponent<SpriteRenderer>().sharedMaterial;
        }
    }
    public void UpdateAmmoText()
    {
        weaponAmmo.text = WeaponAmmoText(CharacterStats.characterEquipment.weapon);
    }

    private string WeaponAmmoText(Weapon temp)
    {
        switch (temp.weaponType)
        {
            case WeaponType.Flamethrower:
                return CharacterStats.gasAmmo.ToString();
            case WeaponType.Pistol:
                return CharacterStats.pistolAmmo.ToString();
            case WeaponType.Rifle:
            case WeaponType.Shotgun:
                return CharacterStats.rifleAmmo.ToString();
            default:
                return "";
        }
    }
}
