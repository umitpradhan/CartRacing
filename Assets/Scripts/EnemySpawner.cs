using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(SpawnDelay());
    }
    public Vector3 EnemySpawn()
    {        
        int roadIndex = Random.Range(0, 3);
        Vector3 roadPoint = Road.roadInstances[roadIndex].roadPosition;
        Vector3 spawnPosition = Road.roadInstances[roadIndex].roadStarting;
        Vector3 position = new Vector3(roadPoint.x, spawnPosition.y, -0.6f);

        return position;
    }
    IEnumerator SpawnDelay()
    {
        while(true)
        {
            yield return null;
            //int roadIndex = Random.Range(0, 3);
            //Vector3 roadEndPoint = Road.roadInstances[roadIndex].roadScale;
            //ObjectPool.instance.SpawnFromPool("ENEMY", EnemySpawn(), Quaternion.identity);

            ObjectPool.instance.SpawnFromPool(randomTag(), EnemySpawn(), Quaternion.identity);

            yield return new WaitForSeconds(1.0f);
        }
    }


    private string randomTag()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0: return "ENEMY";
            case 1: return "DIAMONDS";
            case 2: return "JUICE UP";
            default: return "ENEMY";
        }
    }
}
