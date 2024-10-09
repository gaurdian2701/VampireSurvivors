using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class EnemyCreator
{
    private CreatorEditor creatorEditor;
    private List<GameObject> enemyPrefabsList = new List<GameObject>();
    private List<EnemyScriptableObject> enemyDataList;
    private string[] enemyNames;
    
    private TextField enemyNameField;
    private DropdownField enemyMovementTypeDropdownField;
    private IntegerField maxHealthField;
    private IntegerField enemyDamageField;
    private FloatField enemySpeedField;
    private FloatField enemyStoppingDistanceField;
    private ObjectField spriteField;
    private Image imageForNewEnemy;
    private Image imageForExistingEnemy;
    
    private const string enemyEditorScriptName = "[ENEMY CREATOR] - ";
    private const string enemyScriptableObjectFolderPath = "Assets/GameData/Enemies/";
    private const string objectPoolingScriptableObjectFolderPath = "Assets/GameData/Systems/";
    private const string enemyFolderNamePath = "Enemies/";
    private const string enemyPrefabFolderPath = "Assets/Resources/" + enemyFolderNamePath;
    private const string enemyArtFolderPath = "Assets/KeyArt/" + enemyFolderNamePath;
    public EnemyCreator(CreatorEditor creatorEditor)
    {
        this.creatorEditor = creatorEditor;
    }

    public void LoadGUI()
    {
        creatorEditor.leftPaneLabel.text = "ENEMY CREATOR";
        creatorEditor.rightPaneLabel.text = "ENEMY LIST";
        enemyNameField = new TextField("Enemy Name");

        enemyMovementTypeDropdownField = new DropdownField("Enemy Movement Type");
        enemyMovementTypeDropdownField.choices = Enum.GetNames(typeof(EnemyMovementType)).ToList();

        maxHealthField = new IntegerField("Enemy Max Health");
        enemyDamageField = new IntegerField("Enemy Damage");

        enemySpeedField = new FloatField("Enemy Speed");
        enemyStoppingDistanceField = new FloatField("EnemyStoppingDistance");

        spriteField = new ObjectField("Enemy Sprite");
        spriteField.RegisterValueChangedCallback(GetSpritePreview);

        imageForNewEnemy = new Image();
        imageForExistingEnemy = new Image();
        creatorEditor.StyliseImage(ref imageForNewEnemy);
        creatorEditor.StyliseImage(ref imageForExistingEnemy);
        
        creatorEditor.createButton.text = "Create Enemy";
        creatorEditor.createButton.clicked += CreateEnemy;
        
        creatorEditor.removeButton.text = "Remove Enemy";
        creatorEditor.removeButton.clicked += RemoveEnemy;

        creatorEditor.createdItemsListView = new ListView();
        
        RefreshEnemyData();
        ConfigureListView(creatorEditor.createdItemsListView);
        creatorEditor.createdItemsListView.style.flexGrow = 1;
        creatorEditor.createdItemsListView.selectionChanged += CreatedItemsSelectionChanged;
        
        AddLeftPaneVisualElements();
        AddRightPaneVisualElements();
    }
    
    private void AddLeftPaneVisualElements()
    {
        creatorEditor.leftPaneBox.Add(creatorEditor.leftPaneLabel);
        creatorEditor.leftPaneBox.Add(enemyNameField);
        creatorEditor.leftPaneBox.Add(spriteField);
        creatorEditor.leftPaneBox.Add(maxHealthField);
        creatorEditor.leftPaneBox.Add(enemyDamageField);
        creatorEditor.leftPaneBox.Add(enemySpeedField);
        creatorEditor.leftPaneBox.Add(enemyStoppingDistanceField);
        creatorEditor.leftPaneBox.Add(enemyMovementTypeDropdownField);
        creatorEditor.leftPaneBox.Add(imageForNewEnemy);
        creatorEditor.leftPaneBox.Add(creatorEditor.createButton);
    }
    private void AddRightPaneVisualElements()
    {
        creatorEditor.rightPaneBox.Add(creatorEditor.rightPaneLabel);
        creatorEditor.rightPaneBox.Add(creatorEditor.createdItemsListView);
        creatorEditor.rightPaneBox.Add(creatorEditor.removeButton);
        creatorEditor.rightPaneBox.Add(imageForExistingEnemy);
    }
    private void RefreshEnemyData()
    {
        enemyPrefabsList = Resources.LoadAll<GameObject>(enemyFolderNamePath).ToList();
        enemyNames = new string[enemyPrefabsList.Count];
        for (int i = 0; i < enemyNames.Length; i++)
            enemyNames[i] = enemyPrefabsList[i].name;
        creatorEditor.createdItemsListView.itemsSource = enemyNames;

        
        string[] loadedEnemyDataPaths = Directory.GetFiles(enemyScriptableObjectFolderPath, "*.asset");
        enemyDataList = new List<EnemyScriptableObject>();

        for (int i = 0; i < loadedEnemyDataPaths.Length; i++)
        {
            EnemyScriptableObject enemyLoaded =
                AssetDatabase.LoadAssetAtPath<EnemyScriptableObject>(loadedEnemyDataPaths[i]);
            enemyDataList.Add(enemyLoaded);
        }

        UpdateDataInPoolingScriptableObject();
    }

    private void ConfigureListView(ListView listView)
    {
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (elementToBeAdded, boundItemIndexInList) =>
            (elementToBeAdded as Label).text = enemyNames[boundItemIndexInList];

        listView = new ListView(enemyNames, creatorEditor.listViewElementsSize, makeItem, bindItem);
    }

    private void GetSpritePreview(ChangeEvent<Object> evt)
    {
        ChangeSprite(ref imageForNewEnemy, evt.newValue as Sprite);
        Object enemyArt = AssetDatabase.LoadAssetAtPath<Object>(enemyArtFolderPath + "raven.png");
        EditorGUIUtility.PingObject(enemyArt);
    }

    private void CreatedItemsSelectionChanged(IEnumerable<object> selectedObjects) => ChangeSprite(ref imageForExistingEnemy,
        enemyPrefabsList[creatorEditor.createdItemsListView.selectedIndex].GetComponent<EnemyController>().GetEnemySprite());

    private void ChangeSprite(ref Image spriteToBeChanged, Sprite newSprite) => spriteToBeChanged.sprite = newSprite;

    private void CreateEnemy()
    {
        if (string.IsNullOrEmpty(enemyNameField.value))
        {
            Debug.LogError(enemyEditorScriptName + " CANNOT CREATE ENEMY WITH NO NAME");
            return;
        }

        if (spriteField.value as Sprite == null)
        {
            Debug.LogError(enemyEditorScriptName + " CANNOT CREATE ENEMY WITH NO/INVALID SPRITE");
            return;
        }

        if (AssetDatabase.FindAssets("t:prefab" + " " + enemyNameField.value, new[] { enemyPrefabFolderPath })
                .Length != 0)
        {
            Debug.LogError(enemyEditorScriptName + " FILE WITH SAME NAME ALREADY EXISTS");
            return;
        }

        GenerateEnemy();
    }

    private void GenerateEnemy()
    {
        EnemyScriptableObject enemyData = ScriptableObject.CreateInstance<EnemyScriptableObject>();
        string assetPath =
            AssetDatabase.GenerateUniqueAssetPath(enemyScriptableObjectFolderPath + enemyNameField.text + ".asset");

        InitializeEnemyData(enemyData);
        AssetDatabase.CreateAsset(enemyData, assetPath);
        GameObject newlyCreatedEnemy = CreateEnemyPrefab(enemyData);
        UpdatePoolingService();
        EditorGUIUtility.PingObject(newlyCreatedEnemy);
        RefreshEnemyData();
    }

    private void InitializeEnemyData(EnemyScriptableObject enemyData)
    {
        enemyData.name = enemyNameField.text;
        Enum.TryParse(enemyMovementTypeDropdownField.value, out EnemyMovementType enemyMovementType);
        enemyData.EnemyMovementType = enemyMovementType;
        enemyData.EnemyMaxHealth = maxHealthField.value;
        enemyData.EnemyDamage = enemyDamageField.value;
        enemyData.EnemySpeed = enemySpeedField.value;
        enemyData.EnemyStoppingDistance = enemyStoppingDistanceField.value;
        Sprite enemySprite = spriteField.value as Sprite;
        enemyData.EnemySprite = enemySprite;
    }

    private GameObject CreateEnemyPrefab(EnemyScriptableObject enemyData)
    {
        GameObject baseEnemy = enemyPrefabsList[0];
        string newlyCreatedEnemyPath =
            AssetDatabase.GenerateUniqueAssetPath(enemyPrefabFolderPath + enemyData.name + ".prefab");
        GameObject newlyCreatedEnemy = PrefabUtility.SaveAsPrefabAsset(baseEnemy, newlyCreatedEnemyPath);
        newlyCreatedEnemy.GetComponent<EnemyController>().InitializeEnemyData(enemyData);
        return newlyCreatedEnemy;
    }

    private void RemoveEnemy()
    {
        AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(enemyPrefabsList[creatorEditor.createdItemsListView.selectedIndex]));
        AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(enemyDataList[creatorEditor.createdItemsListView.selectedIndex]));
        RefreshEnemyData();
        UpdateDataInPoolingScriptableObject();
    }

    private void UpdatePoolingService()
    {
        if (System.IO.File.Exists(
                objectPoolingScriptableObjectFolderPath + "ObjectPoolingServiceScriptableObject.asset"))
            UpdateDataInPoolingScriptableObject();
        else
            Debug.LogError("");
    }

    private void UpdateDataInPoolingScriptableObject()
    {
        creatorEditor.objectPoolingServiceScriptableObject.EnemyPrefabsList =
            Resources.LoadAll<EnemyController>(enemyFolderNamePath).ToList();
        EditorUtility.SetDirty(creatorEditor.objectPoolingServiceScriptableObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}