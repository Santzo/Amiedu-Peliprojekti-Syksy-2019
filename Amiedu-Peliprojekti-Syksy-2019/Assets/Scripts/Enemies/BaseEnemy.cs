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
    protected EnemySprite[] sprites;
    Vector3 oriScale;
    private float minPathUpdateTime = 0.45f;
    private float interval = 0.15f;
    private Collider2D[] colliders;
    [HideInInspector]
   internal Vector2[] patrolPoints = new Vector2[3];
    [HideInInspector]
    internal Transform sortingTransform;
    [HideInInspector]
    internal Transform top;
    [HideInInspector]
    internal Transform spritesTransform;
    [HideInInspector]
    internal SortingGroup sGroup;
    [HideInInspector]
    internal Vector2 destination;
    [HideInInspector]
    internal Vector2[] path;
    public EnemyStats stats = new EnemyStats();
    [HideInInspector]
    internal int targetIndex = 0;
    [HideInInspector]
    internal Rigidbody2D rb;
    [HideInInspector]
    internal bool hasBeenHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sGroup = GetComponent<SortingGroup>();
        sortingTransform = transform.Find("SortingTransform");
        top = transform.Find("Top");
        Events.onGameFieldCreated += RandomizePatrolPath;

        idleState = new IdleState(this);                        // Set enemy states
        aggressiveState = new AggressiveState(this);
        patrolState = new PatrolState(this);
        gotHitState = new GotHitState(this);

        spritesTransform = transform.Find("Sprites");
        oriScale = spritesTransform.localScale;
        colliders = spritesTransform.GetComponentsInChildren<Collider2D>();
        var spriteChilds = spritesTransform.GetComponentsInChildren<SpriteRenderer>();
        sprites = new EnemySprite[spriteChilds.Length];
        for (int i = 0; i < spriteChilds.Length; i++)
        {
            var _transform = spriteChilds[i].transform;
            var _sprite = spriteChilds[i];
            var _color = spriteChilds[i].color;
            sprites[i] = new EnemySprite(_transform, _sprite, _color);
        }
        AddToEnemyHitBoxList();
    }

    private void FixedUpdate()
    {
        if (state.currentState != null) state.currentState.OnStateFixedUpdate();
    }

    internal void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            if (newPath.Length == 0) RandomizePatrolPath();
            if (newPath.Length > 0)
            {
                path = newPath;
                targetIndex = 0;
                destination = path[0];
                spritesTransform.localScale = destination.x > rb.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
            }
        }
    }

    internal Vector2 ReturnNextPoint()
    {
        if (rb.position == destination && targetIndex < path.Length - 1)
        {
            targetIndex++;
            destination = path[targetIndex];
            spritesTransform.localScale = destination.x > rb.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
        }
        Debug.Log(destination);
        return Vector2.MoveTowards(rb.position, destination, stats.moveSpeed * Time.deltaTime);
    }

    public void OnGetHit(int damage)
    {
        if (Events.onInventory) return;
        GameObject obj = ObjectPooler.op.Spawn("DamageText", top.position);
        obj.GetComponent<DamageText>().text.text = $"{TextColor.Return("green")}{damage.ToString()}";
        StopCoroutine("GetHit");
        StartCoroutine("GetHit");
    }

    protected IEnumerator GetHit()
    {
        int loop = 0;
        foreach (var sr in sprites)
        {
            sr.sr.color = sr.oriColor;
        }
        while (loop < 3)
        {
            while (sprites[0].sr.color.r > 0.3f)
            {
                foreach (var sr in sprites)
                {
                    sr.sr.color = new Color(sr.sr.color.r - interval, sr.sr.color.g - interval, sr.sr.color.b - interval, sr.sr.color.a);
                }
                yield return null;
            }
            while (sprites[0].sr.color.r < 1f)
            {
                foreach (var sr in sprites)
                {
                    sr.sr.color = new Color(sr.sr.color.r + interval, sr.sr.color.g + interval, sr.sr.color.b + interval, sr.sr.color.a);
                }
                yield return null;
            }
            loop++;
        }

        yield return null;
    }
    private void RandomizePatrolPath()
    {
        Node spawnPoint = PathRequestManager.instance.grid.NodeFromWorldPoint(rb.position);
        spawnPoint = PathRequestManager.instance.grid.GetWalkableNeighbor(spawnPoint);
        rb.position = PathRequestManager.instance.grid.WorldPointFromNode(spawnPoint);

        Node pointOne = PathRequestManager.instance.grid.NodeFromWorldPoint(new Vector2(transform.position.x + UnityEngine.Random.Range(1f, 15f), transform.position.y));
        Node pointTwo = PathRequestManager.instance.grid.NodeFromWorldPoint(new Vector2(transform.position.x, transform.position.y + UnityEngine.Random.Range(1f, 15f)));
        Node pointThree = PathRequestManager.instance.grid.NodeFromWorldPoint(new Vector2(transform.position.x + UnityEngine.Random.Range(-1f, -15f), transform.position.y));
       
        pointOne = PathRequestManager.instance.grid.GetWalkableNeighbor(pointOne);
        pointTwo = PathRequestManager.instance.grid.GetWalkableNeighbor(pointTwo);
        pointThree = PathRequestManager.instance.grid.GetWalkableNeighbor(pointThree);

        patrolPoints[0] = PathRequestManager.instance.grid.WorldPointFromNode(pointOne);
        patrolPoints[1] = PathRequestManager.instance.grid.WorldPointFromNode(pointTwo);
        patrolPoints[2] = PathRequestManager.instance.grid.WorldPointFromNode(pointThree);

        state.ChangeState(patrolState);
    }

    void AddToEnemyHitBoxList()
    {
        foreach (var col in colliders)
            Info.AddEnemyHitbox(col);
        Events.onEnemyHitboxesUpdated(Info.enemyHitboxes);
    }
    void RemoveFromEnemyHitBoxList()
    {
        foreach (var col in colliders)
            Info.RemoveEnemyHitbox(col);
        Events.onEnemyHitboxesUpdated(Info.enemyHitboxes);
    }
    public IEnumerator HasBeenHit(float time)
    {
        hasBeenHit = true;
        yield return new WaitForSeconds(time);
        hasBeenHit = false;
    }

}
