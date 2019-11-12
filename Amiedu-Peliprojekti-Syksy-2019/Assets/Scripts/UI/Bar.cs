using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [HideInInspector]
    public Image bar;
    private TextMeshProUGUI text;
    private float _multiplier;

    private void Awake()
    {
        bar = transform.GetChild(1).GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateValue(float curValue)
    {
        bar.fillAmount = curValue * _multiplier;
        UpdateText();
    }

    public void ChangeValues(float curValue, float maxValue)
    {
        _multiplier = 1f / maxValue;
        bar.fillAmount = curValue * _multiplier;
        UpdateText();
    }
    void UpdateText()
    {
        if (name == "HealthBar")
        {
            int maxHealth = Mathf.RoundToInt(CharacterStats.maxHealth);
            int health = Mathf.Clamp(Mathf.RoundToInt(CharacterStats.health), 0, maxHealth);
            text.text = $"{health} / {maxHealth}";
        }
        else
        {
            int maxStamina = Mathf.RoundToInt(CharacterStats.maxStamina);
            int stamina = Mathf.Clamp(Mathf.RoundToInt(CharacterStats.stamina), 0, maxStamina);
            text.text = $"{stamina} / {maxStamina}";
        }
    }

}
