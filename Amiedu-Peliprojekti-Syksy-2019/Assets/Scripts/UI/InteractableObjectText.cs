using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObjectText : MonoBehaviour
{
    public TextMeshPro text;
    private float increment = 0.025f;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }
    
    public void ToggleTextActive(bool activate)
    {
        StopAllCoroutines();
        if (activate) StartCoroutine(_ActivateText());
        else StartCoroutine(_DeactivateText());
    }

    private void OnEnable()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
    }

    IEnumerator _ActivateText()
    {
        while (text.color.a < 1f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + increment);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
    }


    IEnumerator _DeactivateText()
    {
        while (text.color.a > 0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - increment);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        ObjectPooler.op.DeSpawn(gameObject);
    }
}
