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
        Debug.Log(loadEnemies.Length);
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
                Vector2 spawnPoint = Vector2.zero;
                while (!legitSpawn)
                {
                    spawnPoint = new Vector2(Random.Range(2, References.rf.levelGenerator.worldSizeX - 2), Random.Range(2, References.rf.levelGenerator.worldSizeY - 2));
                    legitSpawn = PathRequestManager.instance.grid.NodeFromWorldPoint(spawnPoint).walkable == 2;
                }
                Debug.Log(PathRequestManager.instance.grid.NodeFromWorldPoint(spawnPoint).walkable);
                var obj = Instantiate(loadEnemies[enemy], spawnPoint, Quaternion.identity);
                enemies.Add(obj.GetComponent<BaseEnemy>());
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
 

}
