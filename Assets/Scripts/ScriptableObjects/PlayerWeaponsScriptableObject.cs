using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeapons", menuName = "ScriptableObject/PlayerWeapons")]
public class PlayerWeaponsScriptableObject : ScriptableObject
{
    public List<WeaponClassData> WeaponsList;
    
    [Serializable]
    public struct WeaponClassData
    {
        public WeaponType WeaponType;
        public GameObject Weapon;
    }
}
