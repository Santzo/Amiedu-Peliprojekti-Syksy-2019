using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Animator[] anim;
    private Color[] colors;
    private int current;

    private void Awake()
    {
        anim = new Animator[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            anim[i] = transform.GetChild(i).GetComponent<Animator>();
        }
    }
    private void OnEnable()
    {
        Events.onItemHover += ItemHover;
        Events.onItemLeaveHover += ItemLeave;
        foreach (var ani in anim)
            ani.SetBool("Hover", false);
    }



    private void OnDisable()
    {
        Events.onItemHover -= ItemHover;
        Events.onItemLeaveHover += ItemLeave;
       
    }
    private void ItemHover(int index, Vector2 pos)
    {
        current = (int)InventoryManager.im.filteredItems[index].itemType;
        if (current < 6) anim[current].SetBool("Hover", true);

    }
    private void ItemLeave()
    {
        if (current < 6) anim[current].SetBool("Hover", false);
    }


}
