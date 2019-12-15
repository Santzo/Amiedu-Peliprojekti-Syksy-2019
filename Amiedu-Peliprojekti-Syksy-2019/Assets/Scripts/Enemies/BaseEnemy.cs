using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BaseEnemy : MonoBehaviour
{
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public EnemyAttack currentAttack;
    protected AudioSource audioSource;
    public AnimationClip walk, idle;
    public AnimationClip[] death;
    public Animator anim;
    LayerMask layers;
    public StateMachine state = new StateMachine();
    public IEnemyState idleState;
    public IEnemyState patrolState;
    public IEnemyState aggressiveState;
    public IEnemyState attackState;
    public IEnemyState gotHitState;
    protected EnemySprite[] sprites;
    Vector3 oriScale;

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

    public  Vector2[] path;
    public EnemyStats stats = new EnemyStats();
    [HideInInspector]
    internal int targetIndex = 0;
    [HideInInspector]
    internal Rigidbody2D rb;
    [HideInInspector]
    internal bool hasBeenHit;
    public AnimatorOverrideController overrideController;

    public int _animMoveSpeed, _animAttackSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sGroup = GetComponent<SortingGroup>();
        anim = GetComponent<Animator>();
        sortingTransform = transform.Find("SortingTransform");
        top = transform.Find("Top");
        idleState = new IdleState(this);                        // Set enemy states
        aggressiveState = new AggressiveState(this);
        patrolState = new PatrolState(this);
        gotHitState = new GotHitState(this);


        spritesTransform = transform.Find("Sprites");
        oriScale = spritesTransform.localScale;
        colliders = spritesTransform.GetComponentsInChildren<Collider2D>();
        Collider2D[] attackCol = colliders.Where(col => col.tag == "EnemyAttackBox").ToArray();
        attackState = new AttackState(this, GetComponent<Collider2D>(), attackCol);

        var spriteChilds = spritesTransform.GetComponentsInChildren<SpriteRenderer>();
        sprites = new EnemySprite[spriteChilds.Length];
        for (int i = 0; i < spriteChilds.Length; i++)
        {
            var _transform = spriteChilds[i].transform;
            var _sprite = spriteChilds[i];
            var _color = spriteChilds[i].color;
            sprites[i] = new EnemySprite(_transform, _sprite, _color);
        }

        _animMoveSpeed = Animator.StringToHash("MoveSpeed");
        _animAttackSpeed = Animator.StringToHash("AttackSpeed");

        anim.SetFloat(_animMoveSpeed, stats.moveSpeed * 0.5f);
        overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;
        anim.runtimeAnimatorController = overrideController;
        overrideController["BaseWalk"] = walk;
        overrideController["BaseIdle"] = idle;
        if (death.Length > 0) overrideController["BaseDeath"] = death[0];
        layers = LayerMask.GetMask("StaticWall", "PlayerHitbox");
        stats.moveSpeed += Random.Range(-0.2f, 0.2f);
        stats.minPathUpdateTime += Random.Range(-0.2f, 0.2f);

    }
    protected virtual void Start()
    {
        AddToEnemyHitBoxList();
        audioSource = Audio.AddAudioSource();
        RandomizePatrolPath();
    }
    public virtual void OnFixedUpdate()
    {
        if (state.currentState != null) state.currentState.OnStateFixedUpdate();
    }

    internal void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (isDead) return;
        if (pathSuccessful)
        {
            if (newPath?.Length == 0)
            {
                if (state.currentState == patrolState)
                {
                    RandomizePatrolPath();
                }
                else if (state.currentState == aggressiveState || state.currentState == attackState)
                {
                    state.ChangeState(aggressiveState);
                }

            }
            if (newPath?.Length > 0)
            {
                Debug.DrawLine(rb.position, newPath[0], Color.red, 1.5f);
                for (int i = 0; i < newPath.Length - 1; i++)
                {
                    Debug.DrawLine(newPath[i], newPath[i + 1], Color.green, 8f);
                }
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
            TurnEnemy(destination.x);
        }
        return Vector2.MoveTowards(rb.position, destination, stats.moveSpeed * Time.deltaTime);
    }

    public void TurnEnemy(float destinationX)
    {
        spritesTransform.localScale = destinationX > rb.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
    }
    public virtual void OnGetHit(int damage, bool crit = false)
    {
        if (Events.onInventory || isDead) return;
        audioSource.PlaySound(stats.audio.getHit);
        if (state.currentState != aggressiveState && state.currentState != attackState) state.ChangeState(aggressiveState);
        GameObject obj = ObjectPooler.op.Spawn("DamageText", top.position);
        DamageText dmg = obj.GetComponent<DamageText>();
        if (!crit) dmg.text.text = $"{TextColor.Green}{damage.ToString()}";
        else dmg.text.text = $"{TextColor.Yellow}{damage.ToString()}!";
        stats.health -= damage;
        if (stats.health > 0)
        {
            StopCoroutine("GetHit");
            StartCoroutine("GetHit");
        }
        else
        {
            audioSource.PlaySound(stats.audio.death);
            overrideController["Death"] = death[0];
            anim.SetTrigger("Death");
            CharacterStats.Essence += stats.essence;
            isDead = true;
            GetComponent<BaseEnemy>().enabled = false;
            foreach (var col in GetComponentsInChildren<Collider2D>())
            {
                col.enabled = false;
            }
            //Destroy(gameObject);
        }
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
        AllRooms startRoom = References.rf.enemyManager.startRoom;

            Node pointOne = PathRequestManager.instance.grid.NodeFromWorldPoint(new Vector2(transform.position.x + UnityEngine.Random.Range(5f, 20f), transform.position.y));
            Node pointTwo = PathRequestManager.instance.grid.NodeFromWorldPoint(new Vector2(transform.position.x, transform.position.y + UnityEngine.Random.Range(5f, 20f)));
            Node pointThree = PathRequestManager.instance.grid.NodeFromWorldPoint(new Vector2(transform.position.x + UnityEngine.Random.Range(-5f, -20f), transform.position.y));

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
            if (!col.CompareTag("EnemyAttackBox")) Info.AddEnemyHitbox(col);
        Events.onEnemyHitboxesUpdated(Info.enemyHitboxes);
    }
    void RemoveFromEnemyHitBoxList()
    {
        foreach (var col in colliders)
            if (!col.CompareTag("EnemyAttackBox")) Info.RemoveEnemyHitbox(col);
        Events.onEnemyHitboxesUpdated(Info.enemyHitboxes);
    }
    public IEnumerator HasBeenHit(float time)
    {
        hasBeenHit = true;
        yield return new WaitForSeconds(time);
        hasBeenHit = false;
    }
    public void CheckPlayerAggro()
    {
        if (CheckRangeToPlayer(stats.hearingRange))
        {
            state.ChangeState(aggressiveState);
            return;
        }
        if (!CheckLineOfSightToPlayer()) return;
        if (!CheckIfInSightRadius(stats.sightRadius)) return;
        state.ChangeState(aggressiveState);
    }
    public bool CheckIfInSightRadius(float sRadius)
    {
        float angle = Vector2.Angle(top.transform.right * Mathf.Sign(spritesTransform.localScale.x), References.rf.playerMovement.transform.position - transform.position);
        if (angle > sRadius) return false;
        return true;
    }

    public bool CheckLineOfSightToPlayer()
    {
        Vector3 player = References.rf.playerMovement.head.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(top.position, player - top.position, stats.sightRange, layers);
        if (!hit) return false;
        if (hit.transform.parent.name != "Player") return false;
        return true;
    }
    public bool CheckRangeToPlayer(float distance)
    {
        Vector3 player = References.rf.playerMovement.head.transform.position;
        if ((top.transform.position - player).sqrMagnitude > distance)
            return false;
        return true;
    }
    public float DistanceToPlayer()
    {
        Vector3 player = References.rf.playerMovement.head.transform.position;
        return (top.transform.position - player).sqrMagnitude;
    }
    public void ApplyForwardForce()
    {
        AttackState _state = state.currentState as AttackState;
        _state.SetTarget();
        _state.applyForce = true;
        audioSource.PlaySound(currentAttack.audio);

    }
    public void PlayFootStep(float volume = 1f)
    {
        float distance = DistanceToPlayer();
        if (distance > 40f) return;
        volume = Mathf.Clamp((40f - distance) * 0.03f, 0.15f, 1f);
        audioSource.PlaySound(stats.audio.walk, volume, Random.Range(0.9f, 1f));
    }
    //private void OnDrawGizmos()
    //{
    //    var node = PathRequestManager.instance.grid.NodeFromWorldPoint(rb.position);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(node.worldPosition, new Vector2(0.5f, 0.5f));
    //}
}
