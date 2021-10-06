using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceUpSpawner : MonoBehaviour
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
        while (true)
        {
            yield return null;
            //int roadIndex = Random.Range(0, 3);
            //Vector3 roadEndPoint = Road.roadInstances[roadIndex].roadScale;
            ObjectPool.instance.SpawnFromPool("JUICE UP", EnemySpawn(), Quaternion.identity);
            yield return new WaitForSeconds(2.0f);
        }
    }
}