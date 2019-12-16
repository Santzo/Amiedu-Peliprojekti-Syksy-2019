using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [HideInInspector]
    public float deltaFixedTimer;
    public static EnemyManager instance;
    public AllRooms startRoom;
    private GameObject[] loadEnemies;
    [HideInInspector]
    public List<BaseEnemy> enemies;
    private int enemyCount;
    
    private void Awake()
    {
        instance = this;
        loadEnemies = Resources.LoadAll<GameObject>("Enemies");
    }

    public void GenerateEnemies(List<AllRooms> rooms)
    {
        enemies = new List<BaseEnemy>();
        foreach (var room in rooms)
        {
            if (room.roomType == RoomType.Start)
            {
                startRoom = room;
                continue;
            }
            
            int howManyEnemies = Random.Range(1, 3);
            for (int i = 0; i < howManyEnemies; i++)
            {
                int enemy = Random.Range(0, loadEnemies.Length);
                bool legitSpawn = false;
                Vector2Int spawnPoint = Vector2Int.zero;
                Vector2 playerPos = References.rf.playerEquipment.transform.position;
                while (!legitSpawn)
                {
                    spawnPoint = new Vector2Int(Random.Range(2, References.rf.levelGenerator.worldSizeX - 2), Random.Range(2, References.rf.levelGenerator.worldSizeY - 2));
                    legitSpawn = PathRequestManager.instance.grid.NodeFromWorldPoint(spawnPoint).walkable == 2;
                    Vector2 tooClose = new Vector2(Mathf.Abs(spawnPoint.x - playerPos.x), Mathf.Abs(spawnPoint.y - playerPos.y));
                    if (tooClose.x < 10 || tooClose.y < 10) legitSpawn = false;
                }
                Vector2 spawn = spawnPoint; 
                var obj = Instantiate(loadEnemies[enemy], spawn, Quaternion.identity);
                BaseEnemy _enemy = obj.GetComponent<BaseEnemy>();
                enemies.Add(_enemy);
                _enemy.gameObject.SetActive(false);
            }
        }
        enemyCount = enemies.Count;
    }

    private void FixedUpdate()
    {
        deltaFixedTimer += Time.fixedDeltaTime;
        for (int i = 0; i < enemyCount; i++)
        {
            enemies[i].OnFixedUpdate();
            if (enemies[i].isDead)
            {
                enemies.Remove(enemies[i]);
                enemyCount--;
            }
        }
    }
    public void ActivateEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
        }
    }

}
