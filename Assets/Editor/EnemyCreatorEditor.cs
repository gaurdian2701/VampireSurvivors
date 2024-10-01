using System;
using System.Linq;
using UnityEditor;
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

        button = new Button();
        button.text = "Create Enemy";
        button.clicked += CreatePrefab;

        box.Add(enemyNameField);
        box.Add(maxHealthField);
        box.Add(enemyDamageField);
        box.Add(enemySpeedField);
        box.Add(enemyStoppingDistanceField);
        box.Add(enemyMovementTypeDropdownField);
        box.Add(button);

        root.Add(label);
        root.Add(box);
    }

    private void CreatePrefab()
    {
        Debug.Log(enemyMovementTypeDropdownField.value);
        if (!string.IsNullOrEmpty(enemyNameField.text))
            GenerateEnemy();
        else
            Debug.LogError("[ENEMY CREATOR] - CANNOT CREATE ENEMY WITH NO NAME");
    }

    private void GenerateEnemy()
    {
        EnemyScriptableObject enemy = CreateInstance<EnemyScriptableObject>();
        string assetPath =
            AssetDatabase.GenerateUniqueAssetPath("Assets/GameData/Enemies/" + enemyNameField.text + ".asset");
        
        InitializeEnemyData(ref enemy);
        AssetDatabase.CreateAsset(enemy, assetPath);
        EditorGUIUtility.PingObject(enemy);
    }

    private void InitializeEnemyData(ref EnemyScriptableObject enemy)
    {
        enemy.name = enemyNameField.text;
        Enum.TryParse(enemyMovementTypeDropdownField.value, out EnemyMovementType enemyMovementType);
        enemy.EnemyMovementType = enemyMovementType;
        enemy.EnemyMaxHealth = maxHealthField.value;
        enemy.EnemyDamage = enemyDamageField.value;
        enemy.EnemySpeed = enemySpeedField.value;
        enemy.EnemyStoppingDistance = enemyStoppingDistanceField.value;
    }
}
