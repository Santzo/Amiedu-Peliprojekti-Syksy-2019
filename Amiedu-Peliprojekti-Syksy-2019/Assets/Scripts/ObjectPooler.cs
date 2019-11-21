using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public Transform _parent;
        public int size;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public static ObjectPooler op;

    // Start is called before the first frame update
    void Awake()
    {
        if (op==null) op = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool._parent);
                obj.name = pool.prefab.name;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }



    public GameObject Spawn(string tag, Vector3? position = null,  Quaternion? rotation = null, Transform parent = null, bool willSpawn = false)
    {
        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        obj.transform.position = position ?? new Vector3(0f,0f,0f);
        obj.transform.rotation = rotation ?? Quaternion.Euler(0f, 0f, 0f);
        poolDictionary[tag].Enqueue(obj);
        ISpawn spawn = obj.GetComponent<ISpawn>();
        if (!(spawn is null)) spawn.Spawn();
        return obj;
    }

    public GameObject SpawnUI(string tag, Vector3? position = null, Transform parent = null)
    {
        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        obj.transform.SetParent(parent, false);
        obj.transform.position = position ?? new Vector3(0f, 0f, 0f);
        poolDictionary[tag].Enqueue(obj);
        ISpawn spawn = obj.GetComponent<ISpawn>();
        if (!(spawn is null)) spawn.Spawn();
        return obj;
    }
    public GameObject SpawnDialogueBox(GameObject head, params Dialogue[] dialogue)
    {
        Events.onDialogueBox = true;
        GameObject obj = poolDictionary["DialogueBox"].Dequeue();
        obj.SetActive(true);
        obj.transform.SetParent(References.rf.uiOverlay, false);
        poolDictionary["DialogueBox"].Enqueue(obj);
        DialogueBox db = obj.GetComponent<DialogueBox>();
        References.rf.currentDialogueBox = db;
        db.currentDialogue = dialogue;
        var newHead = Instantiate(head, db.head.transform.parent);
        newHead.transform.position = db.head.transform.position;
        newHead.transform.localScale = db.head.transform.localScale;
        var sr = newHead.GetComponentsInChildren<SpriteRenderer>();
        foreach (var s in sr)
            s.sortingLayerName = "UI";
        newHead.name = "Head";
        Destroy(db.head);
        db.head = newHead;
        return obj;
    }

    public GameObject DeSpawn(GameObject obj)
    {
        obj.SetActive(false);
        return obj;
    }

 
}
