using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryWeight : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        float totalWeight = 0f;
        CharacterStats.inventoryItems.ForEach(item => totalWeight += item.amount * item.item.weight);
        text.text = totalWeight + " / " + CharacterStats.weightLimit;
       
    }
}
