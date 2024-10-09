using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesListScriptableObject", menuName = "ScriptableObject/UpgradeListScriptableObject")]
public class UpgradesListScriptableObject : ScriptableObject
{
    public List<UpgradeData> Upgrades;
}
