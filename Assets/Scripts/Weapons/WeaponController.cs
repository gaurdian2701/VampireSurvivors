using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponController : MonoBehaviour
{
    [FormerlySerializedAs("playerEquipmentData")] [SerializeField] private PlayerWeaponsScriptableObject playerWeaponsData;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerBodyTransform;

    private Dictionary<WeaponType, GameObject> weaponCacheData;
    private List<Weapon> equippedWeapons = new List<Weapon>();

    private static float spawnRadius = 0.3f;
    
    private void Awake()
    {
        FillCacheData();
    }

    private void Start()
    {
        SpawnWeapon(WeaponType.AXE);
    }

    private void FillCacheData()
    {
        weaponCacheData = new Dictionary<WeaponType, GameObject>();
        for (int i = 0; i < playerWeaponsData.WeaponsList.Count; i++)
        {
            PlayerWeaponsScriptableObject.WeaponClassData weaponClassData = playerWeaponsData.WeaponsList[i];
            weaponCacheData.Add(weaponClassData.WeaponType, weaponClassData.Weapon);
        }
    }

    public void SpawnWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
            default:
            case WeaponType.AXE :
                GameObject axe = Instantiate(weaponCacheData[WeaponType.AXE]);
                AxeController axeController = axe.GetComponent<AxeController>();
                equippedWeapons.Add(axeController);
                break;
        }
        RepositionWeapons();
    }

    private void RepositionWeapons()
    {
        for(int i = 0; i < equippedWeapons.Count; i++)
            equippedWeapons[i].InitializeWeaponPositionAndOrientation(playerTransform, playerBodyTransform, GetWeaponPositionAroundPlayer(equippedWeapons.Count, i));
    }

    private Vector3 GetWeaponPositionAroundPlayer(int numberOfWeaponsToSpawn, int index)
    {
        float radiansAroundCircle = 2 * Mathf.PI / numberOfWeaponsToSpawn * index;
        Vector3 positionAroundPlayer = new Vector3(Mathf.Cos(radiansAroundCircle), Mathf.Sin(radiansAroundCircle), 0) * spawnRadius;
        return positionAroundPlayer;
    }
}
