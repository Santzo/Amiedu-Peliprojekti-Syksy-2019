using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [HideInInspector]
    public Image bar;
    private float _multiplier;

    private void Awake()
    {
        bar = transform.GetChild(1).GetComponent<Image>();
    }

    public void UpdateValue(float curValue)
    {
        bar.fillAmount = curValue * _multiplier;
    }

    public void ChangeValues(float curValue, float maxValue)
    {
        _multiplier = 1f / maxValue;
        bar.fillAmount = curValue * _multiplier;
    }

}
