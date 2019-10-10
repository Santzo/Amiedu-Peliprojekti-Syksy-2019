using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Sprite backgroundPanel;
    private GameObject background;
    [HideInInspector]
    public float wsH, wsW;

    private void Awake()
    {

        wsH = Camera.main.orthographicSize * 2;
        wsW = wsH / Screen.height * Screen.width;
        Debug.Log(Screen.height + ", " + Screen.width);

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
        background = GameObject.Find("Background");
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        background.transform.localScale = new Vector3(wsW / sr.sprite.bounds.size.x, wsH / sr.sprite.bounds.size.y, 1);
        
    }

    public void NewGame()
    {
        CharacterStats.ResetStats();
    }
   


}
