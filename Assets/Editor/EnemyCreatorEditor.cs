using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

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
    private string baseEnemyPrefabPath = "Assets/Resources/Enemies/Merfolk_Enemy.prefab";

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
            Debug.Log(enemyEditorScriptName + " CANNOT CREATE ENEMY WITH NO NAME");
            return;
        }
        
        if (spriteField.value as Sprite == null)
        {
            Debug.LogError(enemyEditorScriptName + " CANNOT CREATE ENEMY WITH NO/INVALID SPRITE");
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
        EditorGUIUtility.PingObject(CreateEnemyPrefab(enemyData));
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
        GameObject baseEnemy =
            (GameObject)AssetDatabase.LoadAssetAtPath(baseEnemyPrefabPath, typeof(GameObject));
        string otherEnemyPath = AssetDatabase.GenerateUniqueAssetPath(enemyPrefabFolderPath + enemyData.name + ".prefab");
        GameObject newlyCreatedEnemy = PrefabUtility.SaveAsPrefabAsset(baseEnemy, otherEnemyPath);
        newlyCreatedEnemy.GetComponent<EnemyController>().InitializeEnemyData(enemyData);
        return newlyCreatedEnemy;
    }
}
