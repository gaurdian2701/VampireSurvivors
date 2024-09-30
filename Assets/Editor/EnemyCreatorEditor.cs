using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCreatorEditor : EditorWindow
{

    [MenuItem("Window/Custom Editors/Enemy Creator Editor")]
    public static void ShowWindow()
    {
        EnemyCreatorEditor window = GetWindow<EnemyCreatorEditor>();
        window.titleContent = new GUIContent("Enemy Creator Editor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        Box box = new Box();
        Label label = new Label("ENEMY CREATION");
        label.transform.scale = Vector3.one;
        
        DropdownField enemyClassDropdownField = new DropdownField("Enemy Class");
        enemyClassDropdownField.choices = Enum.GetNames(typeof(EnemyClass)).ToList();
        
        DropdownField enemyMovementTypeDropdownField = new DropdownField("Enemy Movement Type");
        enemyMovementTypeDropdownField.choices = Enum.GetNames(typeof(EnemyMovementType)).ToList();
        
        IntegerField maxHealthIntegerField = new IntegerField("Enemy Max Health");
        IntegerField enemyDamageField = new IntegerField("Enemy Damage");
        
        FloatField enemySpeedField = new FloatField("Enemy Speed");
        FloatField enemyStoppingDistanceField = new FloatField("EnemyStoppingDistance");
        
        box.Add(maxHealthIntegerField);
        box.Add(enemyDamageField);
        box.Add(enemySpeedField);
        box.Add(enemyStoppingDistanceField);
        box.Add(enemyClassDropdownField);
        box.Add(enemyMovementTypeDropdownField);
        root.Add(label);
        root.Add(box);
    }
}
