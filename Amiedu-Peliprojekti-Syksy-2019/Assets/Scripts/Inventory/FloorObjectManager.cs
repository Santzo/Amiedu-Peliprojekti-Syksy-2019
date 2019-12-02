using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class FloorObjectManager : MonoBehaviour
{
    [HideInInspector]
    public List<DynamicObject> dynamicObjects = new List<DynamicObject>();
    public static FloorObjectManager instance;

    private void Awake()
    {
        instance = this;
        Events.onGameFieldCreated += () => StartCoroutine(DoubleCheckSorting());
    }
    public void Add(DynamicObject obj)
    {
        dynamicObjects.Add(obj);
    }
    public void Remove(DynamicObject obj)
    {
        dynamicObjects.Remove(obj);
    }

    private void OnDisable()
    {
        Events.onGameFieldCreated -= () => StartCoroutine(DoubleCheckSorting());
    }
    void Update()
    {
        if (Time.frameCount % 5 != 0) return;
        foreach (var obj in dynamicObjects)
        {
            if (obj.rb.velocity.magnitude > 0.1f) obj.UpdateSortLayer();
        }
    }
    
    IEnumerator DoubleCheckSorting()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var obj in dynamicObjects)
        {
            obj.sgroup.sortingOrder = Info.SortingOrder(obj.transform.position.y);
        }

    }

}
