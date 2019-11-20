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

    private void GameFieldInitialized(Vector2 start, Vector2 end)
    {
        Vector2 gameFieldSize = end - start;
        Vector2 position = start;
        Vector2 pathFindingField = gameFieldSize;
       
        fogSize.sizeDelta = gameFieldSize;
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
        Events.onGameFieldCreated();
        var extraColliders = FindObjectsOfType<Collider2D>();
        foreach (var col in extraColliders)
        {
            if (col.gameObject.layer == 21) Destroy(col.gameObject);
        }
        StartCoroutine(CreateGrid(start, end, pathFindingField));
    }
 

    private void OnDisable()
    {
        Events.onFieldInitialized -= GameFieldInitialized;

    }
    IEnumerator CreateGrid(Vector2 start, Vector2 end, Vector2 pathFindingField)
    {
        yield return null;
        FindObjectOfType<Grid>().InitializeGrid((end.x + start.x) / 2f, (end.y + start.y) / 2f, pathFindingField.x, pathFindingField.y);
    }
}
