using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    //public static List<Road> roadInstances = new List<Road>(3);
    public static Road[] roadInstances = new Road[3];
    private static Dictionary<string, int> roadTagIndexMap = new Dictionary<string, int>() {
        { "LeftRoad", 0},
        { "MiddleRoad", 1},
        { "RightRoad", 2},
    };

    public Vector3 roadPosition;
    public Vector3 roadScale;
    public Vector3 roadStarting;
    public Vector3 roadEnding;

    public void Awake()
    {
        int indexToPut = roadTagIndexMap[name];
        roadInstances[indexToPut] = this;
        setRoadInfo();

    }

    public void setRoadInfo()
    {
        roadPosition = transform.position;
        roadScale = transform.localScale;
        roadStarting.y = roadScale.y / 2 + roadPosition.y;
        roadEnding.y = roadPosition.y - roadScale.y / 2;
    }

    
}
