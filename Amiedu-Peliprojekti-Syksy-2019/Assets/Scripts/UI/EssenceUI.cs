using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EssenceUI : MonoBehaviour
{
    TextMeshProUGUI essence;
    TextMeshProUGUI updateEssence;
    int oldAmount;
    void Awake()
    {
        essence = GetComponent<TextMeshProUGUI>();
        updateEssence = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        updateEssence.gameObject.SetActive(false);
        essence.text = CharacterStats.Essence.ToString();
        oldAmount = CharacterStats.Essence;
    }
    private void Start()
    {
        Events.onEssenceChanged += UpdateEssenceUI;
    }

    private void UpdateEssenceUI(int _essence)
    {
        updateEssence.gameObject.SetActive(false);
        int amountIncreased = _essence - oldAmount;
        UpdateAmount(_essence, oldAmount);
        oldAmount = CharacterStats.Essence;
        updateEssence.text = "+" + amountIncreased;
        updateEssence.gameObject.SetActive(true);
    }

    async void UpdateAmount(int newAmount, int oldAmount)
    {
        while (oldAmount < newAmount)
        {
            Debug.Log("Blee");
            int amountToAdd = Mathf.Max((newAmount - oldAmount) / 20, 1);
            oldAmount += amountToAdd;
            essence.text = oldAmount.ToString();
            await Task.Yield();
        }
        if (oldAmount > newAmount) oldAmount = newAmount;
        essence.text = oldAmount.ToString();

    }
}
