using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponsManager : MonoBehaviour
{
    [FormerlySerializedAs("playerEquipmentData")] [SerializeField]
    private PlayerWeaponsScriptableObject playerWeaponsData;

    [SerializeField] private Transform playerTransform;

    private Dictionary<WeaponType, GameObject> weaponPrefabCache;
    private Dictionary<WeaponType, List<Weapon>> equippedWeapons;

    private readonly float spawnRadius = 0.3f;
    private readonly int startingNumberOfWeaponsPerCategory = 1;
    private RangedWeaponController rangedWeaponController;
    private int totalNumberOfWeaponEquipped;

    private void Awake()
    {
        FillPrefabCacheData();
        InitializeWeaponLists();
    }

    private void Start()
    {
        SpawnWeapons(WeaponType.MELEE, startingNumberOfWeaponsPerCategory);
        SpawnWeapons(WeaponType.RANGED, startingNumberOfWeaponsPerCategory);
        rangedWeaponController = equippedWeapons[WeaponType.RANGED][0] as RangedWeaponController;
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionVector = (mousePos - (Vector2)playerTransform.position).normalized;
        rangedWeaponController.transform.up = directionVector;
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
        foreach (int i in Enum.GetValues(typeof(WeaponType)))
            equippedWeapons.Add((WeaponType)i, new List<Weapon>());
    }
    

    private void SpawnWeapons(WeaponType weaponType, int numberOfWeapons)
    {
        switch (weaponType)
        {
            default:
            case WeaponType.MELEE:
                for(int i = 0; i < numberOfWeapons; i++)
                    CreateWeapon(WeaponType.MELEE);
                break;
            case WeaponType.RANGED:
                if (equippedWeapons[weaponType].Count == 0)
                    CreateWeapon(WeaponType.RANGED);
                else
                    rangedWeaponController.IncreaseArrowsLoosenedPerShot(numberOfWeapons);
                break;
        }

        totalNumberOfWeaponEquipped++;
        RepositionWeapons();
    }

    private void CreateWeapon(WeaponType weaponType)
    {
        GameObject weapon = Instantiate(weaponPrefabCache[weaponType]);
        Weapon newlyAddedWeapon;
        switch (weaponType)
        {
            default:
            case WeaponType.MELEE:
                newlyAddedWeapon = weapon.GetComponent<MeleeWeaponController>();
                UpdateNewWeaponWithStatsInCurrentWeaponCategory(newlyAddedWeapon, WeaponType.MELEE);
                break;
            
            case WeaponType.RANGED:
                newlyAddedWeapon = weapon.GetComponent<RangedWeaponController>();
                break;
        }
        equippedWeapons[weaponType].Add(newlyAddedWeapon);
    }

    private void UpdateNewWeaponWithStatsInCurrentWeaponCategory(Weapon newlyAddedWeapon, WeaponType weaponType)
    {
        if (equippedWeapons[weaponType].Count != 0)
        {
            Weapon weaponWithUpdatedStats = equippedWeapons[weaponType][0];
            newlyAddedWeapon.InitalizeBaseStats(weaponWithUpdatedStats.BaseDamage,
                weaponWithUpdatedStats.BaseAttackSpeed,
                weaponWithUpdatedStats.BaseKnockBackForce
                );
        }
    }

    private void RepositionWeapons()
    {
        int indexOfCurrentWeaponsListInDictionary = 0, weaponIndexInCurrentList = 0;
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            List<Weapon> currentWeaponList = equippedWeapons[(WeaponType)i];
            for (; weaponIndexInCurrentList < currentWeaponList.Count; weaponIndexInCurrentList++)
                currentWeaponList[weaponIndexInCurrentList].InitializeWeaponPositionAndOrientation(playerTransform,
                    GetWeaponPositionAroundPlayer(totalNumberOfWeaponEquipped,indexOfCurrentWeaponsListInDictionary + weaponIndexInCurrentList));
            
            //I am doing this to ensure that weapons of different classes/types do not spawn on top of each other
            indexOfCurrentWeaponsListInDictionary = weaponIndexInCurrentList;
            weaponIndexInCurrentList = 0;
        }
    }

    public void UpgradeWeapons(UpgradeData upgradeData)
    {
        List<Weapon> currentWeaponListToBeUpgraded = equippedWeapons[upgradeData.WeaponType];

        for (int i = 0; i < currentWeaponListToBeUpgraded.Count; i++)
        {
            Weapon weapon = currentWeaponListToBeUpgraded[i];
            weapon.UpgradeBaseStats(upgradeData);
            switch (upgradeData.WeaponType)
            {
                case WeaponType.RANGED:
                    weapon.GetComponent<RangedWeaponController>().IncreaseAttackSpread(upgradeData.ProjectileSpread);
                    break;
            }
        }
        SpawnWeapons(upgradeData.WeaponType, upgradeData.NumberOfWeapons);
    }

    private Vector3 GetWeaponPositionAroundPlayer(int numberOfWeaponsToSpawn, int index)
    {
        float radiansAroundCircle = 2 * Mathf.PI / numberOfWeaponsToSpawn * index;
        Vector3 positionAroundPlayer =
            new Vector3(Mathf.Cos(radiansAroundCircle), Mathf.Sin(radiansAroundCircle), 0) * spawnRadius;
        return positionAroundPlayer;
    }
}