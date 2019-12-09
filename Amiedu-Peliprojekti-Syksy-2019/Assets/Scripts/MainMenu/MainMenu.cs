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
    MainMenuHandler menuState;

    public void Awake()
    {
        menuState = transform.parent.GetComponent<MainMenuHandler>();
        anim = GetComponent<Animator>();
        bg = GetComponentInChildren<Image>();
        mainMenuHandler = transform.parent.GetComponent<IMainMenuHandler>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (mainMenuHandler.newKeyBeingSet || menuState.menuState == MainMenuHandler.MenuState.AudioSettings && transform.GetSiblingIndex() < 4) return;
        Audio.PlaySound("Click", 1, 0.8f);
        mainMenuHandler.OnClick(transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mainMenuHandler.newKeyBeingSet || menuState.menuState == MainMenuHandler.MenuState.AudioSettings && transform.GetSiblingIndex() < 4) return;
        Audio.PlaySound("Select", 0.8f, 0.9f);
        anim.SetBool("Hover", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
         if (mainMenuHandler.newKeyBeingSet || menuState.menuState == MainMenuHandler.MenuState.AudioSettings && transform.GetSiblingIndex() < 4) return;
        anim.SetBool("Hover", false);
    }
    private void OnDisable()
    {
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0f);
        anim.SetBool("Hover", false);   
    }
    private void OnMouseOver()
    {
        Debug.Log("Mouse is over " + name);
    }
}
