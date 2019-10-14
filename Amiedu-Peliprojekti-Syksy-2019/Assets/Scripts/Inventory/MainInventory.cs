using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MainInventory : MonoBehaviour
{
    GraphicRaycaster gr;
    public void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            if (results.Count > 0) Events.onUIClick(results[0].gameObject.name);
            else Events.onUIClick("");
        }
    }

}
