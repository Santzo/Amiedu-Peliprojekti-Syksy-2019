using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
 
    public void OnClick(string obj)
    {
        switch (obj)
        {
            case "StartGame":
                {
                    SceneManager.LoadScene("Game");
                    break;
                }
        }
    }
}
