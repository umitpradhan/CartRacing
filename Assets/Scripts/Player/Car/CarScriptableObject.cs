using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarScriptableObject", menuName = "ScriptableObjects/CarScriptableObject")]
public class CarScriptableObject : ScriptableObject
{
    //public List<CarStatEntry> CarStats;

    public CarType Type;
    public float MaxSpeed;
    public float Acceleration;
    public float BrakeForce;
    public float SteeringSpeed;
    public float UTurnSpeed;

    [Header("Drift Settings")]
    public float NormalGrip;
    public float SharpTurnGrip;
    public float ManualDriftGrip;
    public float DriftThreshold;


    //public CarType Type;
    //public int Fuel;
    ////public MonkeyView Prefab;
    ////public float RotationSpeed;
    ////public ProjectileType projectileType;
    ////public float Range;
    //public int Cost;
    ////public List<BloonType> AttackableBloons;
    ////public float AttackRate;
}

//[System.Serializable]
//public struct CarStatEntry
//{
//    public CarType Type;
//    public float MaxSpeed;
//    public float Acceleration;
//    public float BrakeForce;
//    public float SteeringSpeed;
//    public float UTurnSpeed;

//    [Header("Drift Settings")]
//    public float NormalGrip;
//    public float SharpTurnGrip;
//    public float ManualDriftGrip;
//    public float DriftThreshold;
//}