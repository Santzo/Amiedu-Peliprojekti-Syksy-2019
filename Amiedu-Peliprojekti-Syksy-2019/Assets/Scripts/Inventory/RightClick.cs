using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerExitHandler, IUIHandler
{

    public List<UIItem> uitem = new List<UIItem>();
    public int itemIndex;

    public void Awake()
    {
        uitem.UItemInitialize(transform);

    }

    public void EntryEnter(int index)
    {
        uitem[index].anim.SetBool("Hover", true);
    }

    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        if (button == PointerEventData.InputButton.Right)
            ObjectPooler.op.DeSpawn(gameObject);
        if (button == PointerEventData.InputButton.Left)
        {
            string buttonType = uitem[index].text.text;
            switch (buttonType)
            {
                case "Equip":
                    Events.onIconDoubleClick(itemIndex, InventoryManager.im.filteredItems[itemIndex]);
                    break;
                case "Discard":
                    Events.onDiscard = true;
                    GameObject obj = ObjectPooler.op.Spawn("DiscardItem");
                    obj.transform.SetParent(transform.parent, false);
                    obj.transform.position = transform.localPosition.x > 270 ? new Vector2(transform.position.x - 0.75f, transform.position.y): (Vector2) transform.position;
                    DiscardItems ditem = obj.GetComponent<DiscardItems>();
                    ditem.Spawn(itemIndex);

                    break;
            }
            ObjectPooler.op.DeSpawn(gameObject);
        }
    }
    public void EntryLeave(int index)
    {
        uitem[index].anim.SetBool("Hover", false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ObjectPooler.op.DeSpawn(gameObject);
    }
}
