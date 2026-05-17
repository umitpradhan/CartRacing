using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObjects/PlayerScriptableObject")]
public class PlayerScriptableObject : ScriptableObject
{
    public int Money; 
    public int Health;
    public List<CarScriptableObject> CarScriptableObjects;
    //public List<ProjectileScriptableObject> ProjectileScriptableObjects;
    //public ProjectileView ProjectilePrefab;
}
