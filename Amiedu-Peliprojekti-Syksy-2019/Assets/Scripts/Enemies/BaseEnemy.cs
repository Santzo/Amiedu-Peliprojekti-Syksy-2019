using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseEnemy : MonoBehaviour
{
    
    protected StateMachine state = new StateMachine();
    protected IEnemyState idleState;
    protected IEnemyState patrolState;
    protected IEnemyState aggressiveState;
    protected IEnemyState attackState;
    protected IEnemyState gotHitState;
    protected Vector2[] patrolPoints = new Vector2[3];
    protected EnemySprite[] sprites;
    Vector2[] path;
    Vector2 destination;
    Vector3 oriScale;
    private Transform sortingTransform;
    protected SortingGroup sGroup;
    private float minPathUpdateTime = 0.45f;
    protected Rigidbody2D rb;
    private float interval = 0.15f;
    protected int targetIndex = 0;

    public EnemyStats stats = new EnemyStats();


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sGroup = GetComponent<SortingGroup>();
        oriScale = transform.localScale;
        sortingTransform = transform.Find("SortingTransform");
        stats = Array.Find(EnemyStats.enemyStats, enemy => enemy.name == transform.name);


        idleState = new IdleState(this);                        // Set enemy states
        aggressiveState = new AggressiveState(this);
        patrolState = new PatrolState(this);
        gotHitState = new GotHitState(this);

        var spritesTransform = transform.Find("Sprites");
        sprites = new EnemySprite[spritesTransform.childCount];

        for (int i = 0; i< spritesTransform.childCount; i++)
        {
            var _transform = spritesTransform.GetChild(i);
            var _sprite = spritesTransform.GetChild(i).GetComponent<SpriteRenderer>();
            var _color = _sprite.color;
            sprites[i] = new EnemySprite(_transform, _sprite, _color);
        }
        state.ChangeState(idleState);
    }

    private void FixedUpdate()
    {
        if (path == null) return;
        if (targetIndex < path.Length - 1 || targetIndex == path.Length - 1 && rb.position != destination)
        {
            rb.position = ReturnNextPoint();
            sGroup.sortingOrder = Info.SortingOrder(sortingTransform.position.y);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.OverlapCircleAll(pos, 0.3f);
            foreach (var h in hit)
            {
                Debug.Log(h.name);
            }
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

    public void OnGetHit(float damage)
    {
        StartCoroutine(GetHit());
    }

    protected IEnumerator GetHit()
    {
        int loop = 0;
        while (loop < 3)
        {
            while (sprites[0].sr.color.a > 0.3f)
            {
                foreach (var sr in sprites)
                {
                    sr.sr.color = new Color(sr.sr.color.r - interval, sr.sr.color.g - interval, sr.sr.color.b - interval, sr.sr.color.a - interval);
                }
                yield return null;
            }
            while (sprites[0].sr.color.a < 1f)
            {
                foreach (var sr in sprites)
                {
                    sr.sr.color = new Color(sr.sr.color.r + interval, sr.sr.color.g + interval, sr.sr.color.b + interval, sr.sr.color.a + interval);
                }
                yield return null;
            }
            loop++;
        }

        yield return null;
    }


}
