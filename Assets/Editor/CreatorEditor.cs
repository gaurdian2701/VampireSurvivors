using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatorEditor : EditorWindow
{
    private EnemyCreator enemyCreator; 
    private TwoPaneSplitView twoPaneSplitView;
    public Box leftPaneBox;
    public Box rightPaneBox;
    public Label leftPaneLabel;
    public Label rightPaneLabel;
    public Button createButton;
    public Button removeButton;
    public ListView createdItemsListView;
    
    private const string objectPoolingScriptableObjectFolderPath = "Assets/GameData/Systems/";
    
    public readonly int listViewElementsSize = 30;
    public readonly int labelBorderSize = 5;
    public readonly int imageBorderSize = 20;
    public readonly int imageHeight = 150;
    public readonly int splitViewWidth = 250;
    
    public ObjectPoolingServiceScriptableObject objectPoolingServiceScriptableObject;
    
    private List<GameObject> enemyPrefabsList;
    private List<EnemyScriptableObject> enemyDataList = new List<EnemyScriptableObject>();
    private string[] enemyNames;

    [MenuItem("Window/Custom Editors/Creator Editor")]
    public static void ShowWindow()
    {
        CreatorEditor window = GetWindow<CreatorEditor>();
        window.titleContent = new GUIContent("Creator");
    }

    private void CreateGUI()
    {
        objectPoolingServiceScriptableObject =
            AssetDatabase.LoadAssetAtPath<ObjectPoolingServiceScriptableObject>(objectPoolingScriptableObjectFolderPath
                + "ObjectPoolingServiceScriptableObject.asset");

        VisualElement root = rootVisualElement;
        twoPaneSplitView =
            new TwoPaneSplitView(0, splitViewWidth, TwoPaneSplitViewOrientation.Horizontal);
        leftPaneBox = new Box();
        rightPaneBox = new Box();
        leftPaneLabel = new Label();
        rightPaneLabel = new Label();
        createButton = new Button();
        removeButton = new Button();
        StyliseLabel(ref leftPaneLabel);
        StyliseLabel(ref rightPaneLabel);
        
        enemyCreator = new EnemyCreator(this);
        ChangeUIToEnemyCreator();

        twoPaneSplitView.Add(leftPaneBox);
        twoPaneSplitView.Add(rightPaneBox);

        root.Add(twoPaneSplitView);
    }

    private void ChangeUIToEnemyCreator()
    {
        enemyCreator.LoadGUI();
    }

    private void StyliseLabel(ref Label label)
    {
        label.style.borderTopWidth = labelBorderSize;
        label.style.borderBottomWidth = labelBorderSize;
        label.style.color = Color.cyan;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
    }

    public void StyliseImage(ref Image image)
    {
        image.scaleMode = ScaleMode.ScaleToFit;
        image.style.height = imageHeight;
        image.style.borderBottomWidth = imageBorderSize;
    }
}