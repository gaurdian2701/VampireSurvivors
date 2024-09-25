using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = System.Random;

public class UIService : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject upgradesPanel;
    [FormerlySerializedAs("settingsPanel")] [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerXpBar;
    [FormerlySerializedAs("playerXpText")] [SerializeField] private TextMeshProUGUI playerXpLevelText;
    [FormerlySerializedAs("weaponToBeUpgradedText")] [SerializeField] private List<TextMeshProUGUI> weaponToBeUpgradedTextList;
    [FormerlySerializedAs("upgradeTypeText")] [SerializeField] private List<TextMeshProUGUI> upgradeTypeTextList;

    private PlayerController playerController;
    private Vector2 xpBarOriginalPos;
    private Vector2 healthBarOriginalPos;
    private float modifier = 5f;
    
    private int currentMaxPlayerHealth;
    private float currentXpToNextLevel;
    private int currentPlayerLevel;
    private int numberOfUpgradeTypes = Enum.GetNames(typeof(UpgradeType)).Length;

    private int[] weaponsData;
    private int[] upgradesData;

    public void Init()
    {
        SubscribeToEvents();
        playerXpBar.fillAmount = 0f;
        currentPlayerLevel = 1;
        pausePanel.SetActive(false);
        upgradesPanel.SetActive(false);
        FillUpgradeCaches();
    }

    private void FillUpgradeCaches()
    {
        weaponsData = new int[3];
        for (int i = 0; i < weaponsData.Length; i++)
            weaponsData[i] = 0;
        
        upgradesData = new int[numberOfUpgradeTypes];
        for (int i = 0; i < upgradesData.Length; i++)
            upgradesData[i] = i;
    }

    private void Start()
    {
        xpBarOriginalPos = playerXpBar.rectTransform.anchoredPosition;
        playerController = GameManager.Instance.PlayerController;
        currentMaxPlayerHealth = playerController.GetPlayerMaxHealth();
        currentXpToNextLevel = playerController.GetCurrentXpToNextLevel();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        ShimmerXpBar();
        ShimmerHealthBar();
        modifier *= -1f;
    }

    private void ShimmerXpBar()
    {
        playerXpBar.rectTransform.anchoredPosition = new Vector2(xpBarOriginalPos.x + modifier, xpBarOriginalPos.y);
    }

    private void ShimmerHealthBar()
    {
        playerHealthBar.rectTransform.anchoredPosition = new Vector2(healthBarOriginalPos.x + modifier, healthBarOriginalPos.y);
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState += OnGamePause;
        GameManager.Instance.EventService.OnGameEnteredPlayState += OnGameResume;
        GameManager.Instance.EventService.OnPlayerTookDamage += DecreaseHealth;
        GameManager.Instance.EventService.OnPlayerPickedUpXp += IncreaseXp;
        GameManager.Instance.EventService.OnPlayerLevelledUp += UpdatePlayerLevelUI;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState -= OnGamePause;
        GameManager.Instance.EventService.OnGameEnteredPlayState -= OnGameResume;
        GameManager.Instance.EventService.OnPlayerTookDamage -= DecreaseHealth;
        GameManager.Instance.EventService.OnPlayerPickedUpXp -= IncreaseXp;
        GameManager.Instance.EventService.OnPlayerLevelledUp -= UpdatePlayerLevelUI;
    }

    private void OnGamePause()
    {
        switch (GameManager.Instance.currentGamePauseType)
        {
            default:
            case GamePauseType.PauseOnPlayerInput :
                pausePanel.SetActive(true);
                break;
            
            case GamePauseType.PauseOnPlayerLevelUp :
                currentXpToNextLevel = playerController.GetCurrentXpToNextLevel();
                upgradesPanel.SetActive(true);
                GenerateUpgradesForPlayer();
                break;
        }
    }

    private void OnGameResume()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        upgradesPanel.SetActive(false);
    }

    public void OnResumeButtonClicked()
    {
        GameManager.Instance.EventService.InvokeGameEnteredPlayStateEvent();
    }

    public void OnOptionsButtonClicked()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void OnOptionsBackButtonClicked()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    private void DecreaseHealth(int damage) => playerHealthBar.fillAmount -= (float) damage / currentMaxPlayerHealth;

    private void IncreaseXp() => playerXpBar.fillAmount = playerController.GetCurrentPlayerXp() / currentXpToNextLevel;

    private void UpdatePlayerLevelUI()
    {
        currentPlayerLevel += 1;
        playerXpBar.fillAmount = 0f;
        playerXpLevelText.text = $"{currentPlayerLevel}";
        upgradesPanel.SetActive(true);
    }

    private void GenerateUpgradesForPlayer()
    {
        FillUpgradeText<WeaponType>(weaponToBeUpgradedTextList, weaponsData);
        FillUpgradeText<UpgradeType>(upgradeTypeTextList, upgradesData);
    }

    private void FillUpgradeText<T>(List<TextMeshProUGUI> textList, int[] dataArray) where T : Enum
    {
        ShuffleUpgradeData(dataArray);
        int i = 0, j = 0;

        while (i < textList.Count)
        {
            textList[i].text = Enum.GetName(typeof(T), dataArray[j]);
            i++;
            if (j < dataArray.Length - 1)
                j++;
        }
    }
    
    private void ShuffleUpgradeData(int[] array)
    {
        Random random = new Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    public void OnUpgradeChosen(int upgradeChosen)
    {
        GameManager.Instance.PlayerWeaponController.
            UpgradeWeapons((WeaponType)weaponsData[upgradeChosen], (UpgradeType)upgradesData[upgradeChosen]);   
        GameManager.Instance.EventService.InvokePlayerSelectedUpgradeEvent();
    }
}
