using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MainInventory : MonoBehaviour
{
    GraphicRaycaster gr;
    List<IResetUI> resetObjs = new List<IResetUI>();
    public void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
    }

    public void Start()
    {
        Transform[] objs = GetComponentsInChildren<Transform>(true);
        foreach (var obj in objs)
        {
            IResetUI temp = obj.GetComponent<IResetUI>();
            if (!(temp is null))
                resetObjs.Add(temp);
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
            CheckResults(results[0].gameObject.name);
        }
    }

    private void CheckResults(string result)
    {
        foreach (var obj in resetObjs)
        {
            obj.Reset(result);
        
        }
    }
}
