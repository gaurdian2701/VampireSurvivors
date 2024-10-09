using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class UpgradeCreatorEditor : EditorWindow
{
    private TwoPaneSplitView twoPaneSplitView;
    private Box leftPaneBox;
    private Box rightPaneBox;
    private Label leftPaneLabel;
    private Label rightPaneLabel;
    private DropdownField weaponTypeForUpgradeDropdownField;
    private IntegerField damageIncreaseForUpgradeField;
    private FloatField attackSpeedIncreaseForUpgradeField;
    private FloatField attackSpreadIncreaseForUpgradeField;
    private IntegerField knockBackIncreaseForUpgradeField;
    private IntegerField numberOfWeaponsIncreasedForUpgradeField;
    private Button createNewUpgradeButton;
    private ListView enemyListView;
    
    private const int splitViewWidth = 250;
    private const string upgradesDataDirectory = "Assets/GameData/Upgrades/UpgradesListScriptableObject.asset";
    
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
        
        LoadUpgradeCreatorUI();

        FillLeftPane();
        FillRightPane();

        twoPaneSplitView.Add(leftPaneBox);
        twoPaneSplitView.Add(rightPaneBox);

        root.Add(twoPaneSplitView);
    }

    private void LoadUpgradeCreatorUI()
    {
        InitializeFields();
        FillLeftPane();
        FillRightPane();
        twoPaneSplitView.Add(leftPaneBox);
        twoPaneSplitView.Add(rightPaneBox);
    }

    private void InitializeFields()
    {
        upgradesListScriptableObject =
            AssetDatabase.LoadAssetAtPath<UpgradesListScriptableObject>(upgradesDataDirectory);
        twoPaneSplitView =
            new TwoPaneSplitView(0, splitViewWidth, TwoPaneSplitViewOrientation.Horizontal);
        leftPaneBox = new Box();
        rightPaneBox = new Box();
        leftPaneLabel = new Label("UPGRADE CREATOR");
        rightPaneLabel = new Label("UPGRADE LIST");
        MiscFunctions.StyliseLabel(ref leftPaneLabel);
        MiscFunctions.StyliseLabel(ref rightPaneLabel);
        
        weaponTypeForUpgradeDropdownField = new DropdownField("Weapon Type for Upgrade");
        weaponTypeForUpgradeDropdownField.choices = Enum.GetNames(typeof(WeaponType)).ToList();
        weaponTypeForUpgradeDropdownField.RegisterValueChangedCallback(OnWeaponTypeSelected);
        
        damageIncreaseForUpgradeField = new IntegerField("Damage Increase");
        attackSpeedIncreaseForUpgradeField = new FloatField("Attack Speed Increase");
        attackSpreadIncreaseForUpgradeField =  new FloatField("Attack Spread Increase");
        knockBackIncreaseForUpgradeField = new IntegerField("KnockBack Increase");
        numberOfWeaponsIncreasedForUpgradeField = new IntegerField("Number of Weapons Added");
        
        createNewUpgradeButton = new Button();
        createNewUpgradeButton.text = "Create Upgrade";
        createNewUpgradeButton.clicked += CreateNewUpgrade;
    }

    private void OnWeaponTypeSelected(ChangeEvent<string> evt)
    {
        Enum.TryParse(evt.newValue, out WeaponType weaponType);
        if (weaponType == WeaponType.AXE)
        {
            leftPaneBox.Remove(attackSpreadIncreaseForUpgradeField);
            if(!leftPaneBox.Contains(numberOfWeaponsIncreasedForUpgradeField))
                leftPaneBox.Add(numberOfWeaponsIncreasedForUpgradeField);
        }
        else if (weaponType == WeaponType.CROSSBOW)
        {
            leftPaneBox.Remove(numberOfWeaponsIncreasedForUpgradeField);
            if(!leftPaneBox.Contains(attackSpreadIncreaseForUpgradeField))
                leftPaneBox.Add(attackSpreadIncreaseForUpgradeField);
        }
    }

    private void CreateNewUpgrade()
    {
        Enum.TryParse(weaponTypeForUpgradeDropdownField.value, out WeaponType weaponType);
        if (weaponType == WeaponType.AXE)
        {
            AxeUpgradeData axeUpgrade = new AxeUpgradeData();
            axeUpgrade.InitializeBaseUpgradeStats(damageIncreaseForUpgradeField.value,
                attackSpeedIncreaseForUpgradeField.value,
                knockBackIncreaseForUpgradeField.value);
            axeUpgrade.NumberOfWeapons = numberOfWeaponsIncreasedForUpgradeField.value;
            upgradesListScriptableObject.Upgrades.Add(axeUpgrade);
        }
        else if (weaponType == WeaponType.CROSSBOW)
        {
            CrossbowUpgradeData crossbowUpgrade = new CrossbowUpgradeData();
            crossbowUpgrade.InitializeBaseUpgradeStats(damageIncreaseForUpgradeField.value,
                attackSpeedIncreaseForUpgradeField.value,
                knockBackIncreaseForUpgradeField.value);
            crossbowUpgrade.AttackSpread = attackSpreadIncreaseForUpgradeField.value;
            upgradesListScriptableObject.Upgrades.Add(crossbowUpgrade);
        }
    }
    
    private void FillLeftPane()
    {
        leftPaneBox.Add(leftPaneLabel);
        leftPaneBox.Add(weaponTypeForUpgradeDropdownField);
        leftPaneBox.Add(damageIncreaseForUpgradeField);
        leftPaneBox.Add(attackSpreadIncreaseForUpgradeField);
        leftPaneBox.Add(knockBackIncreaseForUpgradeField);
        leftPaneBox.Add(numberOfWeaponsIncreasedForUpgradeField);
        leftPaneBox.Add(createNewUpgradeButton);
    }

    private void FillRightPane()
    {
        rightPaneBox.Add(rightPaneLabel);
    }
}
