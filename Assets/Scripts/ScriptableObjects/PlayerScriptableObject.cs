using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObject/Player")]
public class PlayerScriptableObject : ScriptableObject
{
    public int PlayerMaxHealth;
    public float PlayerMovementSpeed;
}
