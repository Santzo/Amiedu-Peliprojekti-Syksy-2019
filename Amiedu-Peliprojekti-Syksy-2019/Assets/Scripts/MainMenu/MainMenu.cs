using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu: MonoBehaviour, IUIObject
{
    public Animator anim;
    private Image bg;
    public IMainMenuHandler mainMenuHandler;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        bg = GetComponentInChildren<Image>();
        mainMenuHandler = transform.parent.gameObject.GetComponent<IMainMenuHandler>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mainMenuHandler.OnClick(transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool("Hover", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("Hover", false);
    }
    private void OnDisable()
    {
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0f);
        anim.SetBool("Hover", false);   
    }

}
