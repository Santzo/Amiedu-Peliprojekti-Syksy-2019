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
    Coroutine updatingAmount;
    int oldAmount;
    int amount;
    void Awake()
    {
        essence = GetComponent<TextMeshProUGUI>();
        essence.text = CharacterStats.Essence.ToString();
        oldAmount = CharacterStats.Essence;
    }
    private void Start()
    {
        updateEssence = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        updateEssence.gameObject.SetActive(false);
        Events.onEssenceChanged += UpdateEssenceUI;
    }

    private void UpdateEssenceUI(int _essence)
    {
        updateEssence.gameObject.SetActive(false);
        int amountIncreased = _essence - oldAmount;
        if (updatingAmount == null) updatingAmount = StartCoroutine(UpdateAmount());
        updateEssence.text = "+" + amountIncreased;
        updateEssence.gameObject.SetActive(true);
    }

    IEnumerator UpdateAmount()
    {
        amount = oldAmount;
        while (amount < CharacterStats.Essence)
        {
            int amountToAdd = Mathf.Max((CharacterStats.Essence - amount) / 20, 1);
            amount += amountToAdd;
            essence.text = amount.ToString();
            yield return null;
        }
        essence.text = CharacterStats.Essence.ToString();
        updatingAmount = null;
        oldAmount = CharacterStats.Essence;
        amount = CharacterStats.Essence;
    }
}
