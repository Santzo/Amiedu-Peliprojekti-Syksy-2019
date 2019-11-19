using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryScreenCharacter : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void UpdateEquipment(GameObject gear)
    {
        Destroy(transform.GetChild(0).gameObject);
        var obj = Instantiate(gear, transform);
        obj.name = "Chestgear";
        obj.transform.SetAsFirstSibling();
        if (CharacterStats.characterEquipment.lightSource != null)
        {
            var weapon = CharacterStats.characterEquipment.weapon;

            if (weapon == null || weapon != null && weapon.hands == Hands.One_handed)
            {
                var ls = obj.transform.GetFromAllChildren("Lightsource").GetComponentInChildren<ParticleSystem>().transform;
                if (ls != null)
                {
                    ls.transform.localScale = ls.transform.localScale * 2.5f;
                }
            }
        }
        anim.runtimeAnimatorController = References.rf.playerEquipment.overrider;
        anim.Rebind();
    }


}
