using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RightClick : MonoBehaviour
{
    [HideInInspector]
    public TextMeshProUGUI[] line;
    private Animator[] anim;


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

    public void EntryClick(int index)
    {
        Debug.Log(line[index].text);
    }
    public void EntryLeave(int index)
    {
        anim[index].SetBool("Hover", false);
    }
}
