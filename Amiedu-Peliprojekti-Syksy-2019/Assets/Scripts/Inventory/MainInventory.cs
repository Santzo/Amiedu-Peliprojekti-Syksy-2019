using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MainInventory : MonoBehaviour
{
    GraphicRaycaster gr;
    List<GameObject> resetObjs = new List<GameObject>();
    public void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
    }

    public void Start()
    {
        Transform[] objs = GetComponentsInChildren<Transform>(true);
        foreach (var obj in objs)
        {
            if (!(obj.GetComponent<IResetUI>() is null))
                resetObjs.Add(obj.gameObject);
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            gr.Raycast(ped, results);


        }
    }
}
