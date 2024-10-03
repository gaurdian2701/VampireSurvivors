using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

public class EnemyCreatorEditor : EditorWindow
{
    
    private Button createNewEnemyButton;
    private Button removeExistingEnemyButton;
    private TextField enemyNameField;
    private DropdownField enemyMovementTypeDropdownField;
    private IntegerField maxHealthField;
    private IntegerField enemyDamageField;
    private FloatField enemySpeedField;
    private FloatField enemyStoppingDistanceField;
    private ObjectField spriteField;
    private Image imageForNewEnemy;
    private Image imageForExistingEnemy;
    private ListView enemyListView;

    private const string enemyEditorScriptName = "[ENEMY CREATOR] - ";
    private const string enemyScriptableObjectFolderPath = "Assets/GameData/Enemies/";
    private const string objectPoolingScriptableObjectFolderPath = "Assets/GameData/Systems/";
    private const string enemyPrefabResourcesPath = "Enemies/";
    private const string enemyPrefabFolderPath = "Assets/Resources/" + enemyPrefabResourcesPath;
    private const int listViewElementsSize = 30;
    private const int labelBorderSize = 5;
    private const int imageBorderSize = 20;
    private const int imageHeight = 150;
    private const int splitViewWidth = 250;

    private List<GameObject> enemiesLoaded;
    private string[] enemyNames;

    [MenuItem("Window/Custom Editors/Enemy Creator Editor")]
    public static void ShowWindow()
    {
        EnemyCreatorEditor window = GetWindow<EnemyCreatorEditor>();
        window.titleContent = new GUIContent("Enemy Creator");
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        Box leftPaneBox = new Box();
        Box rightPaneBox = new Box();
        TwoPaneSplitView twoPaneSplitView = new TwoPaneSplitView(0, splitViewWidth, TwoPaneSplitViewOrientation.Horizontal);
        Label leftPaneLabel = new Label("ENEMY CREATOR");
        Label rightPaneLabel = new Label("ENEMY LIST");
        StyliseLabel(ref leftPaneLabel);
        StyliseLabel(ref rightPaneLabel);

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
        StyliseImage(ref imageForNewEnemy);
        StyliseImage(ref imageForExistingEnemy);
        
        createNewEnemyButton = new Button();
        createNewEnemyButton.text = "Create Enemy";
        createNewEnemyButton.clicked += CreateEnemy;

        removeExistingEnemyButton = new Button();
        removeExistingEnemyButton.text = "Remove Enemy";
        removeExistingEnemyButton.clicked += RemoveEnemy;
        
        enemyListView = new ListView();
        RefreshEnemyListData();
        ConfigureListView(ref enemyListView);
        enemyListView.style.flexGrow = 1;
        enemyListView.selectionChanged += EnemySelectionChanged;

        leftPaneBox.Add(leftPaneLabel);
        leftPaneBox.Add(enemyNameField);
        leftPaneBox.Add(spriteField);
        leftPaneBox.Add(maxHealthField);
        leftPaneBox.Add(enemyDamageField);
        leftPaneBox.Add(enemySpeedField);
        leftPaneBox.Add(enemyStoppingDistanceField);
        leftPaneBox.Add(enemyMovementTypeDropdownField);
        leftPaneBox.Add(imageForNewEnemy);
        leftPaneBox.Add(createNewEnemyButton);
        
        rightPaneBox.Add(rightPaneLabel);
        rightPaneBox.Add(enemyListView);
        rightPaneBox.Add(removeExistingEnemyButton);
        rightPaneBox.Add(imageForExistingEnemy);
        
        twoPaneSplitView.Add(leftPaneBox);
        twoPaneSplitView.Add(rightPaneBox);
        
        root.Add(twoPaneSplitView);
    }

    private void StyliseLabel(ref Label label)
    {
        label.style.borderTopWidth = labelBorderSize;
        label.style.borderBottomWidth = labelBorderSize;
        label.style.color = Color.cyan;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
    }

    private void StyliseImage(ref Image image)
    {
        image.scaleMode = ScaleMode.ScaleToFit;
        image.style.height = imageHeight;
        image.style.borderBottomWidth = imageBorderSize;
    }

    private void RefreshEnemyListData()
    {
        enemiesLoaded = Resources.LoadAll<GameObject>(enemyPrefabResourcesPath).ToList();
        enemyNames = new string[enemiesLoaded.Count];
        for(int i = 0; i < enemyNames.Length; i++)
            enemyNames[i] = enemiesLoaded[i].name;
        enemyListView.itemsSource = enemiesLoaded;
    }

    private void ConfigureListView(ref ListView listView)
    {
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (elementToBeAdded, boundItemIndexInList) =>
            (elementToBeAdded as Label).text = enemyNames[boundItemIndexInList];

        listView = new ListView(enemyNames, listViewElementsSize, makeItem, bindItem);
    }

    private void GetSpritePreview(ChangeEvent<Object> evt) => ChangeSprite(ref imageForNewEnemy, evt.newValue as Sprite);

    private void EnemySelectionChanged(IEnumerable<object> selectedObjects) => ChangeSprite(ref imageForExistingEnemy,
        enemiesLoaded[enemyListView.selectedIndex].GetComponent<EnemyController>().GetEnemySprite());
    
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
        EnemyScriptableObject enemyData = CreateInstance<EnemyScriptableObject>();
        string assetPath =
            AssetDatabase.GenerateUniqueAssetPath(enemyScriptableObjectFolderPath + enemyNameField.text + ".asset");
        
        InitializeEnemyData(enemyData);
        AssetDatabase.CreateAsset(enemyData, assetPath);
        GameObject newlyCreatedEnemy = CreateEnemyPrefab(enemyData);
        UpdatePoolingService(newlyCreatedEnemy);
        EditorGUIUtility.PingObject(newlyCreatedEnemy);
        RefreshEnemyListData();
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
        GameObject baseEnemy = enemiesLoaded[0];
        string newlyCreatedEnemyPath = AssetDatabase.GenerateUniqueAssetPath(enemyPrefabFolderPath + enemyData.name + ".prefab");
        GameObject newlyCreatedEnemy = PrefabUtility.SaveAsPrefabAsset(baseEnemy, newlyCreatedEnemyPath);
        newlyCreatedEnemy.GetComponent<EnemyController>().InitializeEnemyData(enemyData);
        return newlyCreatedEnemy;
    }

    private void RemoveEnemy()
    {
        
    }

    private void UpdatePoolingService(GameObject newlyCreatedEnemy)
    {
        if (File.Exists(objectPoolingScriptableObjectFolderPath + "ObjectPoolingServiceScriptableObject.asset"))
        {
            ObjectPoolingServiceScriptableObject objectPoolingServiceScriptableObject = 
                AssetDatabase.LoadAssetAtPath<ObjectPoolingServiceScriptableObject>(objectPoolingScriptableObjectFolderPath + "ObjectPoolingServiceScriptableObject.asset");
            UpdateDataInPoolingScriptableObject(objectPoolingServiceScriptableObject, newlyCreatedEnemy);
        }
        else
            CreateNewObjectPoolingServiceScriptableObject(newlyCreatedEnemy);
    }

    private void CreateNewObjectPoolingServiceScriptableObject(GameObject newlyCreatedEnemy)
    {
        //Havent completed this functionality yet
        ObjectPoolingServiceScriptableObject objectPoolingServiceScriptableObject = CreateInstance<ObjectPoolingServiceScriptableObject>();
        string newlyCreatedPoolingScriptableObjectPath = AssetDatabase.GenerateUniqueAssetPath(objectPoolingScriptableObjectFolderPath + "ObjectPoolingServiceScriptableObject.asset");
        UpdateDataInPoolingScriptableObject(objectPoolingServiceScriptableObject, newlyCreatedEnemy);
        AssetDatabase.CreateAsset(objectPoolingServiceScriptableObject, newlyCreatedPoolingScriptableObjectPath);
    }

    private void UpdateDataInPoolingScriptableObject(ObjectPoolingServiceScriptableObject objectPoolingServiceScriptableObject, GameObject newlyCreatedEnemy)
    {
        objectPoolingServiceScriptableObject.EnemyPrefabsList.Add(newlyCreatedEnemy.GetComponent<EnemyController>());
        EditorUtility.SetDirty(objectPoolingServiceScriptableObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
