using System.Collections;

using System.Collections.Generic;
using System.Threading.Tasks;
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
        foreach (var obj in dynamicObjects)
        {
            if ((obj.lastPos - (Vector2) obj.transform.position).sqrMagnitude > 0.01f)
            {
                Debug.Log("Updated object");
                obj.lastPos = obj.transform.position;
                obj.sgroup.sortingOrder = Info.SortingOrder(obj.transform.position.y);
            }

        }
    }
    
    IEnumerator DoubleCheckSorting()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var obj in dynamicObjects)
        {
            obj.sgroup.sortingOrder = Info.SortingOrder(obj.transform.position.y);
            obj.lastPos = obj.transform.position;
        }
    }
  

}
