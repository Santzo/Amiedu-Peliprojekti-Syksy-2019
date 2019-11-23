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
    }
    public void Add(DynamicObject obj)
    {
        dynamicObjects.Add(obj);
    }
    public void Remove(DynamicObject obj)
    {
        dynamicObjects.Remove(obj);
    }

    void Update()
    {
        if (Time.frameCount % 5 != 0) return;
        foreach (var obj in dynamicObjects)
        {
            if (obj.rb.velocity.magnitude > 0.1f) obj.UpdateSortLayer();
        }
    }
}
