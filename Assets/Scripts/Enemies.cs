using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float _enemySpeed;
    
    void Update()
    {
        _enemySpeed = Random.Range(10.0f, 50.0f);        
        transform.Translate(Vector3.down*_enemySpeed*Time.deltaTime);       
    }
    
}
