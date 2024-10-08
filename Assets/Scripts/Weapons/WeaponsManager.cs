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

    private static float spawnRadius = 0.3f;
    private CrossbowController crossbowController;
    private int totalNumberOfWeaponEquipped = 0;

    private void Awake()
    {
        FillPrefabCacheData();
        InitializeWeaponLists();
    }

    private void Start()
    {
        SpawnWeapon(WeaponType.AXE);
        SpawnWeapon(WeaponType.CROSSBOW);
        crossbowController = equippedWeapons[WeaponType.CROSSBOW][0] as CrossbowController;
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionVector = (mousePos - (Vector2)playerTransform.position).normalized;
        crossbowController.transform.up = directionVector;
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

    public void SpawnWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
            default:
            case WeaponType.AXE:
                CreateAndUpdateWeaponStats(WeaponType.AXE);
                break;
            case WeaponType.CROSSBOW:
                if (equippedWeapons[weaponType].Count == 0)
                    CreateAndUpdateWeaponStats(WeaponType.CROSSBOW);
                break;
        }

        totalNumberOfWeaponEquipped++;
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
            case WeaponType.CROSSBOW:
                newlyAddedWeapon = weapon.GetComponent<CrossbowController>();
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
        Vector3 positionAroundPlayer =
            new Vector3(Mathf.Cos(radiansAroundCircle), Mathf.Sin(radiansAroundCircle), 0) * spawnRadius;
        return positionAroundPlayer;
    }
}