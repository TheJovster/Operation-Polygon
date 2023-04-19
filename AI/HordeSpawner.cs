using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HordeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private int enemiesToSpawn = 15;
    [SerializeField] private float maxXOffset = 6f;
    [SerializeField] private float maxZOffset = 6f;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < enemiesToSpawn; i++) 
        {
            SpawnInstance();
        }
    }

    private void SpawnInstance() 
    {
        Vector3 randomSpawnPos = new Vector3(Random.Range(0f, maxXOffset), 0f, Random.Range(0f, maxZOffset));
        GameObject enemyInstance =Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + randomSpawnPos, Quaternion.identity);
        enemyInstance.transform.parent = this.transform;
        Debug.Log(transform.childCount);
    }
    
}
