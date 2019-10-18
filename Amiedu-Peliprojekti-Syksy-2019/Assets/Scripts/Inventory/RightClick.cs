using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerExitHandler
{
    [HideInInspector]
    public TextMeshProUGUI[] line;
    private Animator[] anim;
    [HideInInspector]
    public int itemIndex;


    private void Awake()
    {
        line = new TextMeshProUGUI[transform.childCount];
        anim = new Animator[transform.childCount];

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Transform child = transform.GetChild(i);
            anim[i] = child.GetComponent<Animator>();
            line[i] = child.GetChild(0).GetComponent<TextMeshProUGUI>();
            child.GetComponent<RightClickEntry>().rightClick = this;
            
        }
    }

    public void EntryEnter(int index)
    {
        anim[index].SetBool("Hover", true);
    }

    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        if (button == PointerEventData.InputButton.Right)
            ObjectPooler.op.DeSpawn(gameObject);
        if (button == PointerEventData.InputButton.Left)
        {
            string buttonType = line[index].text;
            switch (buttonType)
            {
                case "Equip":
                    Events.onIconDoubleClick(itemIndex, InventoryManager.im.filteredItems[itemIndex]);
                    break;
            }
            ObjectPooler.op.DeSpawn(gameObject);
        }
    }
    public void EntryLeave(int index)
    {
        anim[index].SetBool("Hover", false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ObjectPooler.op.DeSpawn(gameObject);
    }
}
