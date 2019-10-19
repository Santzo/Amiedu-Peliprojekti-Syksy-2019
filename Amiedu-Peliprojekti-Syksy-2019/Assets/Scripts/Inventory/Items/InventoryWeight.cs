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
        Events.onInventoryChange += UpdateWeight;
        UpdateWeight();
       
    }

    private void OnDisable()
    {
        Events.onInventoryChange -= UpdateWeight;
    }

    void UpdateWeight()
    {
        float totalWeight = 0f;
        CharacterStats.inventoryItems.ForEach(item => totalWeight += item.amount * item.item.weight);
        text.text = totalWeight + " / " + CharacterStats.weightLimit;
    }
}
