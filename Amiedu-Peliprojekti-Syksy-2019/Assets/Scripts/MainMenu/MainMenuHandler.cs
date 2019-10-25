using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour, IMainMenuHandler
{
    public void OnClick(Transform trans)
    {
       switch (trans.name)
        {
            case "StartGame":
                {
                    SceneManager.LoadScene("Game");
                    break;
                }
        }
    }

    public void OnEnter(Transform trans)
    {
       
    }

    public void OnExit(Transform trans)
    {
        
    }
}
