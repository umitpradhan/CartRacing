using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public List<PlacedObjectData> objects = new();
}

[System.Serializable]
public class PlacedObjectData
{
    public string type;
    public Vector3 position;
    public Vector3 rotation;
}

public enum LevelObjectType
{
    Road,
    Obstacle,
    Decoration,
    SpawnPoint
}

