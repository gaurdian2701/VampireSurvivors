using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerXpControllerScriptableObject", menuName = "ScriptableObject/PlayerXpControllerScriptableObject")]
public class PlayerXpControllerScriptableObject : ScriptableObject
{
    public int StartingPlayerXpToNextLevel;
    public int PlayerMaxLevel;
}
