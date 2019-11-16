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
        anim.runtimeAnimatorController = References.rf.playerEquipment.overrider;
        anim.Rebind();
    }


}
