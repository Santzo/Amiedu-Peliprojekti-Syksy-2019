using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseEnemy : MonoBehaviour
{
    protected EnemyStats stats = new EnemyStats();
    protected StateMachine state = new StateMachine();
    protected IEnemyState idleState;
    protected IEnemyState patrolState;
    protected IEnemyState aggressiveState;
    protected IEnemyState attackState;
    protected Vector2[] patrolPoints = new Vector2[3];
    Vector2[] path;
    Vector2 destination;
    Vector3 oriScale;
    private Transform testi;
    protected SortingGroup sGroup;
    private float minPathUpdateTime = 0.45f;
    protected Rigidbody2D rb;
    protected int targetIndex = 0;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sGroup = GetComponent<SortingGroup>();
        oriScale = transform.localScale;
        testi = GameObject.Find("Test").transform;
        stats = Array.Find(EnemyStats.enemyStats, enemy => enemy.name == transform.name);
        idleState = new IdleState(this);
        aggressiveState = new AggressiveState(this);
        state.ChangeState(idleState);
    }

    private void FixedUpdate()
    {
        if (path == null) return;
        if (targetIndex < path.Length - 1 || targetIndex == path.Length - 1 && rb.position != destination)
        {
            rb.position = ReturnNextPoint();
            sGroup.sortingOrder = Info.SortingOrder(rb.position.y);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
                transform.localScale = destination.x > rb.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
            }
        }
    }

    Vector2 ReturnNextPoint()
    {
        if (rb.position == destination && targetIndex < path.Length - 1)
        {
            targetIndex++;
            destination = path[targetIndex];
            transform.localScale = destination.x > rb.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
        }
        return Vector2.MoveTowards(rb.position, destination, stats.moveSpeed * Time.deltaTime);
    }

}
