using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class EnemyCreatorEditor : EditorWindow
{
    
    private Button button;
    private TextField enemyNameField;
    private DropdownField enemyMovementTypeDropdownField;
    private IntegerField maxHealthField;
    private IntegerField enemyDamageField;
    private FloatField enemySpeedField;
    private FloatField enemyStoppingDistanceField;
    private ObjectField spriteField;

    private string enemyEditorScriptName = "[ENEMY CREATOR] - ";
    private string enemyPrefabFolderPath = "Assets/Resources/Enemies/";
    private string enemyScriptableObjectFolderPath = "Assets/GameData/Enemies/";
    private string objectPoolingScriptableObjectFolderPath = "Assets/GameData/Systems/";
    private string baseEnemyPrefabPath = "Enemies/Merfolk_Enemy";

    [MenuItem("Window/Custom Editors/Enemy Creator Editor")]
    public static void ShowWindow()
    {
        EnemyCreatorEditor window = GetWindow<EnemyCreatorEditor>();
        window.titleContent = new GUIContent("Enemy Creator");
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        Box box = new Box();
        Label label = new Label("ENEMY CREATION");
        label.transform.scale = new Vector3(1, 1, 0);

        enemyNameField = new TextField("Enemy Name");

        enemyMovementTypeDropdownField = new DropdownField("Enemy Movement Type");
        enemyMovementTypeDropdownField.choices = Enum.GetNames(typeof(EnemyMovementType)).ToList();

        maxHealthField = new IntegerField("Enemy Max Health");
        enemyDamageField = new IntegerField("Enemy Damage");

        enemySpeedField = new FloatField("Enemy Speed");
        enemyStoppingDistanceField = new FloatField("EnemyStoppingDistance");
        
        spriteField = new ObjectField("Enemy Sprite");
        
        button = new Button();
        button.text = "Create Enemy";
        button.clicked += CreateEnemy;

        box.Add(enemyNameField);
        box.Add(spriteField);
        box.Add(maxHealthField);
        box.Add(enemyDamageField);
        box.Add(enemySpeedField);
        box.Add(enemyStoppingDistanceField);
        box.Add(enemyMovementTypeDropdownField);
        box.Add(button);

        root.Add(label);
        root.Add(box);
    }

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
        GameObject baseEnemy = Resources.Load<GameObject>(baseEnemyPrefabPath);
        Debug.Log(baseEnemy);
        string newlyCreatedEnemyPath = AssetDatabase.GenerateUniqueAssetPath(enemyPrefabFolderPath + enemyData.name + ".prefab");
        GameObject newlyCreatedEnemy = PrefabUtility.SaveAsPrefabAsset(baseEnemy, newlyCreatedEnemyPath);
        newlyCreatedEnemy.GetComponent<EnemyController>().InitializeEnemyData(enemyData);
        return newlyCreatedEnemy;
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
