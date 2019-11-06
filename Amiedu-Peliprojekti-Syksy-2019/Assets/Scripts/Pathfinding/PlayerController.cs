using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Camera cam;
    Rigidbody2D rb;
    Vector2 destination;
    float moveSpeed = 50f;
    Vector2[] path;
    int targetIndex;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        destination = rb.position;
    }

    private void FixedUpdate()
    {
        if (path == null) return;
        if (targetIndex < path.Length - 1 || targetIndex == path.Length - 1 && rb.position != destination)
            rb.position = ReturnNextPoint();
     
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(pos, Vector2.zero);

            PathRequestManager.RequestPath(new PathRequest(false, rb.position, pos, OnPathFound));

        }
    }

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            if (newPath.Length > 0)
            {
                path = newPath;
                targetIndex = 0;
                destination = path[0];
            }
        }
    }

    Vector2 ReturnNextPoint()
    {
        if (rb.position == destination && targetIndex < path.Length - 1)
        {
            targetIndex++;
            destination = path[targetIndex];
        }
        return Vector2.MoveTowards(rb.position, destination, moveSpeed * Time.deltaTime);
    }

    
}
