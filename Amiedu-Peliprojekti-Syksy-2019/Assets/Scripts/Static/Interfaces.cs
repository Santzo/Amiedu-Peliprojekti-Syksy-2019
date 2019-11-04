﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public interface ISpawn
{
    void Spawn();
}
public interface IResetUI
{
    void Reset(string result);
}
public interface IUIHandler
{
    void EntryEnter(int index);
    void EntryClick(int index, PointerEventData.InputButton button);
    void EntryLeave(int index);

}
public class UIItem
{
    public Animator anim;
    public TextMeshProUGUI text;
    public Transform trans;
}
public interface IUIObject: IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
 
}
public interface IMainMenuHandler
{
    void OnClick(Transform trans);
    void OnEnter(Transform trans);
    void OnExit(Transform trans);
}
public interface IEnemyState
{
    void OnStateEnter();
    void OnStateExecute();
    void OnStateExit();
}