using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //private float _speed = 5.0f;    
    //private Vector3 _playerBound;
    //private Vector3 roadWidth;
    //private float playerWidth;
    private int roadIndex = 1;
    //[SerializeField]
    //public GameObject outerRoad;

    void Start()
    {
        transform.position = new Vector3(0, 0, -0.6f);
        //PlayerBound();        
        //playerWidth = transform.GetComponent<Renderer>().bounds.extents.x;
        
    }
   
    void Update()
    {


        //float horizontalInput = Input.GetAxis("Horizontal");
        //Vector3 playerMov = new Vector3(horizontalInput*instantPosition.x, 0, 0);
        bool left = Input.GetKeyDown(KeyCode.A);
        bool right = Input.GetKeyDown(KeyCode.D);

        Debug.Log("pressed: "+ right +" , roadIndex: "+ roadIndex);
        if (right == true && roadIndex < 2)
        {
            Debug.Log("pressed right");
            roadIndex++;
            Vector3 instantPosition = Road.roadInstances[roadIndex].roadPosition;
            Debug.Log("roadIndex: " + roadIndex);
            transform.position = new Vector3(instantPosition.x,0,-0.6f);
            
        }
        
        if (left == true && roadIndex > 0)
        {
            Debug.Log("pressed left");
            roadIndex--;
            Vector3 instantPosition = Road.roadInstances[roadIndex].roadPosition;
            transform.position = new Vector3(instantPosition.x, 0, -0.6f);
        }
        //Vector3 newPlayerPos= playerMov * _speed * Time.deltaTime;        
        //transform.Translate(newPlayerPos);


    //    if (transform.position.x + playerWidth >= _playerBound.x)
    //    {
            
    //        transform.position = new Vector3(_playerBound.x - playerWidth, 0, -0.6f);
    //    }
    //    else if (transform.position.x - playerWidth <= (_playerBound.x * -1))
    //    {
    //        transform.position = new Vector3((_playerBound.x * -1) + playerWidth, 0, -0.6f);
    //    }

    //}
    //public float PlayerBound()
    //{
    //    int roadIndex = Random.Range(0, 3);
    //    _playerBound = Road.roadInstances[roadIndex].roadPosition;
        //roadWidth = outerRoad.transform.localScale;        
        //_playerBound.x = roadWidth.x + roadWidth.x / 2;
        //return _playerBound.x;
    }
}
