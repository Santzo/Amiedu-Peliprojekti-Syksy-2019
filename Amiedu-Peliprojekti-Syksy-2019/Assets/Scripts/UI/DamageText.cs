using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private Vector2 speed;
    public TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        transform.position += (Vector3) speed * Time.deltaTime;
    }

    private void OnEnable()
    {
        speed = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(0.5f, 0.8f));
    }
    public void DisableText()
    {
        ObjectPooler.op.DeSpawn(gameObject);
    }
}
