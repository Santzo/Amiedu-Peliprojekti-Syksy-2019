using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu: MonoBehaviour, IMainMenuObject
{
    public Animator anim;
    public IMainMenuHandler mainMenuHandler;

    public void Awake()
    {
        anim = GetComponent<Animator>();
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
   
}
