using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour, IMainMenuHandler
{
    private GameObject settingsReference;
    private SettingsMenu settingsMenuScript;

    private void Awake()
    {
        settingsReference = transform.Find("Settings").gameObject;
        settingsMenuScript = settingsReference.GetComponent<SettingsMenu>();
    }

    public void OnClick(Transform trans)
    {
       switch (trans.name)
        {
            case "StartGame":
                SceneManager.LoadScene("Game");
                break;

            case "LoadGame":
                Debug.Log("Load Game");
                break;

            case "Settings":
                settingsMenuScript.OpenSettingsMenu();
                break;

            case "QuitGame":
                Application.Quit();
                break;
        }
    }

    public void OnEnter(Transform trans)
    {
       
    }

    public void OnExit(Transform trans)
    {
        
    }
}
