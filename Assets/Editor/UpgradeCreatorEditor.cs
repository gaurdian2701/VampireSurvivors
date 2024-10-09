using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeCreatorEditor : EditorWindow
{
    private TwoPaneSplitView twoPaneSplitView;
    private Box leftPaneBox;
    private Box rightPaneBox;
    private Label leftPaneLabel;
    private Label rightPaneLabel;
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
    
    private const int splitViewWidth = 250;
    
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
        leftPaneBox = new Box();
        rightPaneBox = new Box();
        twoPaneSplitView =
            new TwoPaneSplitView(0, splitViewWidth, TwoPaneSplitViewOrientation.Horizontal);
        leftPaneLabel = new Label("UPGRADE CREATOR");
        rightPaneLabel = new Label("UPGRADE LIST");
        MiscFunctions.StyliseLabel(ref leftPaneLabel);
        MiscFunctions.StyliseLabel(ref rightPaneLabel);
        FillLeftPane();
        FillRightPane();
        twoPaneSplitView.Add(leftPaneBox);
        twoPaneSplitView.Add(rightPaneBox);
    }

    private void FillLeftPane()
    {
        leftPaneBox.Add(leftPaneLabel);
    }

    private void FillRightPane()
    {
        rightPaneBox.Add(rightPaneLabel);
    }
}
