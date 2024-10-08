using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponsManager : MonoBehaviour
{
    [FormerlySerializedAs("playerEquipmentData")] [SerializeField] private PlayerWeaponsScriptableObject playerWeaponsData;
    [SerializeField] private Transform playerTransform;

    private Dictionary<WeaponType, GameObject> weaponPrefabCache;
    private Dictionary<WeaponType, List<Weapon>> equippedWeapons;

    private static float spawnRadius = 0.3f;
    
    private void Awake()
    {
        FillPrefabCacheData();
        InitializeWeaponLists();
    }

    private void Start()
    {
        SpawnWeapon(WeaponType.AXE);
    }

    private void FillPrefabCacheData()
    {
        weaponPrefabCache = new Dictionary<WeaponType, GameObject>();
        for (int i = 0; i < playerWeaponsData.WeaponsList.Count; i++)
        {
            PlayerWeaponsScriptableObject.WeaponClassData weaponClassData = playerWeaponsData.WeaponsList[i];
            weaponPrefabCache.Add(weaponClassData.WeaponType, weaponClassData.Weapon);
        }
    }

    private void InitializeWeaponLists()
    {
        equippedWeapons = new Dictionary<WeaponType, List<Weapon>>();
        equippedWeapons.Add(WeaponType.AXE, new List<Weapon>());
    }

    public void SpawnWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
            default:
            case WeaponType.AXE :
                CreateAndUpdateWeaponStats(WeaponType.AXE);
                break;
        }
        RepositionWeapons();
    }

    private void CreateAndUpdateWeaponStats(WeaponType weaponType)
    {
        GameObject weapon = Instantiate(weaponPrefabCache[weaponType]);
        Weapon newlyAddedWeapon;

        switch (weaponType)
        {   
            default:
            case WeaponType.AXE:
                newlyAddedWeapon = weapon.GetComponent<AxeController>();
                break;
        }
        if (equippedWeapons[weaponType].Count != 0)
        {
            Weapon weaponWithUpdatedStats = equippedWeapons[weaponType][0];
            newlyAddedWeapon.SetBaseDamage(weaponWithUpdatedStats.BaseDamage);
            newlyAddedWeapon.SetBaseAttackSpeed(weaponWithUpdatedStats.BaseAttackSpeed);
            newlyAddedWeapon.SetBaseKnockBackForce(weaponWithUpdatedStats.BaseKnockBackForce);
        }
        equippedWeapons[weaponType].Add(newlyAddedWeapon);
    }

    private void RepositionWeapons()
    {
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            List<Weapon> currentWeaponList = equippedWeapons[(WeaponType)i];
            for(int j = 0; j < currentWeaponList.Count; j++)
                currentWeaponList[j].InitializeWeaponPositionAndOrientation(playerTransform, GetWeaponPositionAroundPlayer(currentWeaponList.Count, j));
            //This will spawn different weapon classes on top of each other, have to change later
        }
    }

    public void UpgradeWeapons(WeaponType weaponType, UpgradeType upgradeType)
    {
        List<Weapon> currentWeaponListToBeUpgraded = equippedWeapons[weaponType];
        switch (upgradeType)
        {
            default:
            case UpgradeType.DAMAGE:
                foreach (Weapon weapon in currentWeaponListToBeUpgraded)
                    weapon.SetBaseDamage(weapon.BaseDamage + weapon.baseDamageIncreaseRate);
                break;
            case UpgradeType.SPEED:
                foreach (Weapon weapon in currentWeaponListToBeUpgraded)
                    weapon.SetBaseAttackSpeed(weapon.BaseAttackSpeed + weapon.baseAttackSpeedIncreaseRate);
                break;
            case UpgradeType.KNOCKBACK:
                foreach (Weapon weapon in currentWeaponListToBeUpgraded)
                    weapon.SetBaseKnockBackForce(weapon.BaseKnockBackForce + weapon.baseKnockBackForceIncreaseRate);
                break;
            case UpgradeType.PLUS_ONE:
                SpawnWeapon(weaponType);
                break;
        }
    }

    private Vector3 GetWeaponPositionAroundPlayer(int numberOfWeaponsToSpawn, int index)
    {
        float radiansAroundCircle = 2 * Mathf.PI / numberOfWeaponsToSpawn * index;
        Vector3 positionAroundPlayer = new Vector3(Mathf.Cos(radiansAroundCircle), Mathf.Sin(radiansAroundCircle), 0) * spawnRadius;
        return positionAroundPlayer;
    }
}
