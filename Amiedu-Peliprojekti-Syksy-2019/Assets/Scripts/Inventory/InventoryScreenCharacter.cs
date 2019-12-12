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
        var particles = GetComponentsInChildren<ParticleSystem>();
        var lights = GetComponentsInChildren<Light>();
        foreach (var par in particles)
        {
            if (par.transform.parent.parent.name != "Lightsource" && par.transform.parent.parent.name != "TwoHandedLightSource") par.gameObject.SetActive(false);
        }
        foreach (var light in lights)
            light.gameObject.SetActive(false);

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
            else
            {
                var ls = obj.transform.GetFromAllChildren("TwoHandedLightSource").GetComponentInChildren<ParticleSystem>().transform;
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
