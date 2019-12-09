using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private GameObject background;
    [HideInInspector]
    public float wsH, wsW;

    private void Awake()
    {
        GameSettings.ApplySettings();
        wsH = Camera.main.orthographicSize * 2;
        wsW = wsH / Screen.height * Screen.width;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        background = GameObject.Find("MenuBackground");
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        background.transform.localScale = new Vector3(wsW / sr.sprite.bounds.size.x, wsH / sr.sprite.bounds.size.y, 1);
        GameObject.Find("Zombie").GetComponent<Animator>().SetTrigger("Idle");
        GameObject.Find("ZombieShadow").GetComponent<Animator>().SetTrigger("Idle");
        Audio.PlayOnLoop("MainMenuMusic");
    }

    public void NewGame()
    {
        CharacterStats.ResetStats();
    }
   


}
