using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogOfWar : MonoBehaviour
{

    public RenderTexture fogOfWarMain;

    public RenderTexture fogOfWarSecondary;
    public RectTransform fogSize;
    public Material image;
    public Camera fogOfWarMainCamera;
    public Camera fogOfWarSecondaryCamera;
    private float padding = 7.5f;


    private void Awake()
    {
        Events.onFieldInitialized += GameFieldInitialized;
    }

    private void GameFieldInitialized(LevelManager.AllRooms obj)
    {
        Vector2 gameFieldSize = obj.end - obj.start;
        Vector2 position = obj.start - new Vector2(padding * 0.5f, padding * 0.5f);

        fogSize.sizeDelta = gameFieldSize + new Vector2(padding, padding);
        fogSize.position = position;
        int x = Mathf.RoundToInt(fogSize.sizeDelta.x * 7f);
        int y = Mathf.RoundToInt(fogSize.sizeDelta.y * 7f);
        fogOfWarMain = new RenderTexture(x, y, 0, RenderTextureFormat.Default);
        fogOfWarSecondary = new RenderTexture(x, y, 0, RenderTextureFormat.Default);



        Vector2 cameraPos = position + fogSize.sizeDelta * 0.5f;
        fogOfWarMainCamera.transform.position = new Vector3(cameraPos.x, cameraPos.y, -10f);
        fogOfWarSecondaryCamera.transform.position = new Vector3(cameraPos.x, cameraPos.y, -10f);
        fogOfWarMainCamera.orthographicSize = (fogSize.sizeDelta.x + fogSize.sizeDelta.y) / 20f;
        fogOfWarSecondaryCamera.orthographicSize = (fogSize.sizeDelta.x + fogSize.sizeDelta.y) / 20f;

        fogOfWarMainCamera.targetTexture = fogOfWarMain;
        fogOfWarSecondaryCamera.targetTexture = fogOfWarSecondary;

        image.SetTexture("_MainTex", fogOfWarMain);
        image.SetTexture("_SecondaryTex", fogOfWarSecondary);

        fogOfWarMainCamera.orthographicSize = fogSize.sizeDelta.y * 0.5f;
        fogOfWarSecondaryCamera.orthographicSize = fogSize.sizeDelta.y * 0.5f;

    }
 

    private void OnDisable()
    {
        Events.onFieldInitialized -= GameFieldInitialized;

    }

}
