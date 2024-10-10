using System;
using System.Linq;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class UpgradeCreatorEditor : EditorWindow
{
    private Box mainBox;
    private Label leftPaneLabel;
    private TextField nameOfUpgradeField;
    private DropdownField weaponTypeForUpgradeDropdownField;
    private IntegerField damageIncreaseForUpgradeField;
    private FloatField attackSpeedIncreaseForUpgradeField;
    private FloatField projectileSpreadIncreaseForUpgradeField;
    private IntegerField knockBackIncreaseForUpgradeField;
    private IntegerField numberOfWeaponsIncreasedForUpgradeField;
    private Button createNewUpgradeButton;

    private const string upgradesDataDirectory = "Assets/GameData/Upgrades/UpgradesListScriptableObject.asset";
    private const string upgradeCreatorString = "[UPGRADE CREATOR] - ";

    private UpgradesListScriptableObject upgradesListScriptableObject;

    [MenuItem("Window/Custom Editors/Upgrade Creator Editor")]
    public static void ShowWindow()
    {
        UpgradeCreatorEditor window = GetWindow<UpgradeCreatorEditor>();
        window.titleContent = new GUIContent("Upgrade Creator");
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        upgradesListScriptableObject =
            AssetDatabase.LoadAssetAtPath<UpgradesListScriptableObject>(upgradesDataDirectory);
        LoadUpgradeCreatorUI();
        root.Add(mainBox);
    }

    private void LoadUpgradeCreatorUI()
    {
        InitializeFields();
        FillBox();
    }

    private void InitializeFields()
    {
        mainBox = new Box();
        leftPaneLabel = new Label("UPGRADE CREATOR");
        MiscFunctions.StyliseLabel(ref leftPaneLabel);

        nameOfUpgradeField = new TextField("Name of Upgrade");

        weaponTypeForUpgradeDropdownField = new DropdownField("Weapon Type for Upgrade");
        weaponTypeForUpgradeDropdownField.choices = Enum.GetNames(typeof(WeaponType)).ToList();
        weaponTypeForUpgradeDropdownField.RegisterValueChangedCallback(OnWeaponTypeSelected);

        damageIncreaseForUpgradeField = new IntegerField("Damage Increase");
        attackSpeedIncreaseForUpgradeField = new FloatField("Attack Speed Increase");
        projectileSpreadIncreaseForUpgradeField = new FloatField("Projectile Spread Increase");
        projectileSpreadIncreaseForUpgradeField.style.display = DisplayStyle.None;
        knockBackIncreaseForUpgradeField = new IntegerField("KnockBack Increase");
        numberOfWeaponsIncreasedForUpgradeField = new IntegerField("Number of Weapons/Projectiles Added");

        createNewUpgradeButton = new Button();
        createNewUpgradeButton.text = "Create Upgrade";
        createNewUpgradeButton.clicked += CreateNewUpgrade;
    }

    private void OnWeaponTypeSelected(ChangeEvent<string> evt)
    {
        Enum.TryParse(evt.newValue, out WeaponType weaponType);
        if (weaponType == WeaponType.MELEE)
        {
            projectileSpreadIncreaseForUpgradeField.style.display = DisplayStyle.None;
        }
        else if (weaponType == WeaponType.RANGED)
        {
            projectileSpreadIncreaseForUpgradeField.style.display = DisplayStyle.Flex;
        }
    }

    private void CreateNewUpgrade()
    {
        EditorGUIUtility.PingObject(upgradesListScriptableObject);
        if (string.IsNullOrEmpty(nameOfUpgradeField.value))
        {
            Debug.LogError(upgradeCreatorString + "CANNOT CREATE UPGRADE WITH NO NAME");
            return;
        }

        if (AllInputFieldsAreEmpty())
        {
            Debug.LogError(upgradeCreatorString + "FILL IN AT LEAST ONE FIELD");
            return;
        }

        Enum.TryParse(weaponTypeForUpgradeDropdownField.value, out WeaponType weaponType);


        UpgradeData newUpgrade = new UpgradeData();
        newUpgrade.InitializeBaseUpgradeStats(nameOfUpgradeField.value,
            weaponType,
            damageIncreaseForUpgradeField.value,
            attackSpeedIncreaseForUpgradeField.value,
            knockBackIncreaseForUpgradeField.value,
            numberOfWeaponsIncreasedForUpgradeField.value,
            projectileSpreadIncreaseForUpgradeField.value);
        newUpgrade.NumberOfWeapons = numberOfWeaponsIncreasedForUpgradeField.value;
        upgradesListScriptableObject.Upgrades.Add(newUpgrade);
    }

    private bool AllInputFieldsAreEmpty() => damageIncreaseForUpgradeField.value +
        attackSpeedIncreaseForUpgradeField.value +
        projectileSpreadIncreaseForUpgradeField.value +
        knockBackIncreaseForUpgradeField.value +
        numberOfWeaponsIncreasedForUpgradeField.value <= 0;

    private void FillBox()
    {
        mainBox.Add(leftPaneLabel);
        mainBox.Add(nameOfUpgradeField);
        mainBox.Add(weaponTypeForUpgradeDropdownField);
        mainBox.Add(damageIncreaseForUpgradeField);
        mainBox.Add(attackSpeedIncreaseForUpgradeField);
        mainBox.Add(projectileSpreadIncreaseForUpgradeField);
        mainBox.Add(knockBackIncreaseForUpgradeField);
        mainBox.Add(numberOfWeaponsIncreasedForUpgradeField);
        mainBox.Add(createNewUpgradeButton);
    }
}