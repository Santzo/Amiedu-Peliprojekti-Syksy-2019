using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatDescription : MonoBehaviour
{
    TextMeshProUGUI[] text;
    private void Awake()
    {
        text = GetComponentsInChildren<TextMeshProUGUI>();
    }
    public void UpdateText(string title, string _text)
    {
        text[0].text = title;
        text[1].text = _text;
    }
}
