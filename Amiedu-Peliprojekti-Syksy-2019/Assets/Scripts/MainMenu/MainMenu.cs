using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Animator anim;
    public MainMenuHandler mainMenuHandler;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        mainMenuHandler = transform.parent.gameObject.GetComponent<MainMenuHandler>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mainMenuHandler.OnClick(eventData.pointerPress.name);
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
