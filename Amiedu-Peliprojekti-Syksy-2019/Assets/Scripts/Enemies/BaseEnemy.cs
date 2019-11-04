using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
 

    protected EnemyStats stats = new EnemyStats();
    protected StateMachine state = new StateMachine();
    protected IEnemyState idleState;
    protected IEnemyState patrolState;
    protected IEnemyState aggressiveState;
    protected IEnemyState attackState;
    protected Vector2[] patrolPoints = new Vector2[3];
    Vector3[] path;
    Vector2 direction;
    Vector3 oriScale;
    private Transform testi;
    private float minPathUpdateTime = 0.45f;
    protected Rigidbody2D rb;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        oriScale = transform.localScale;
        testi = GameObject.Find("Test").transform;
        stats = Array.Find(EnemyStats.enemyStats, enemy => enemy.name == transform.name);
        idleState = new IdleState(this);
        aggressiveState = new AggressiveState(this);
        state.ChangeState(idleState);
    }

    private void Start()
    {
        StartCoroutine(UpdatePath(testi));
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(direction.x, direction.y) * stats.moveSpeed;
    }

    private void Update()
    {
        state.currentState.OnStateExecute();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            state.ChangeState(aggressiveState);
        }
    }

    protected virtual void TurnDirection(Vector3 target, int interval = 0)
    {
        if (interval == 0) interval = 60;
        if (Time.frameCount % interval == 0)
        {
            if (transform.position.x > target.x && transform.localScale.x > 0f)
                transform.localScale = new Vector3(-oriScale.x, oriScale.y, transform.localScale.z);
            else if (transform.position.x < target.x && transform.localScale.x < 0f)
                transform.localScale = new Vector3(oriScale.x, oriScale.y, transform.localScale.z);
        }

    }
    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = waypoints;
            TurnDirection(path[0], 1);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    protected IEnumerator UpdatePath(Transform target)
    {
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        }
    }

    protected IEnumerator FollowPath()
    {
        int pathIndex = 0;
        while (pathIndex < path.Length)
        {
            if (Vector2.Distance(rb.position, path[pathIndex]) < 0.2f)
                pathIndex++;

            if (pathIndex >= path.Length)
                direction = new Vector2(0f, 0f);

            if (pathIndex < path.Length)
                direction = (path[pathIndex] - transform.position).normalized;
            yield return null;
        }
    }

}
