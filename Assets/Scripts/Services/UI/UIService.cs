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
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerXpBar;
    [FormerlySerializedAs("playerXpText")] [SerializeField] private TextMeshProUGUI playerXpLevelText;
    [FormerlySerializedAs("weaponToBeUpgradedText")] [SerializeField] private List<TextMeshProUGUI> weaponToBeUpgradedTextList;
    [FormerlySerializedAs("upgradeTypeText")] [SerializeField] private List<TextMeshProUGUI> upgradeTypeTextList;

    private int currentMaxPlayerHealth;
    private int currentXpToNextLevel;
    private int currentPlayerLevel;
    private int numberOfWeaponTypes = Enum.GetNames(typeof(WeaponType)).Length;
    private int numberOfUpgradeTypes = Enum.GetNames(typeof(UpgradeType)).Length;

    private int[] weapons;
    private int[] upgrades;
    
    private Random random = new Random();

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
        weapons = new int[numberOfWeaponTypes];
        for (int i = 0; i < weapons.Length; i++)
            weapons[i] = i;
        
        upgrades = new int[numberOfUpgradeTypes];
        for (int i = 0; i < upgrades.Length; i++)
            upgrades[i] = i;
    }

    private void Start()
    {
        currentMaxPlayerHealth = GameManager.Instance.PlayerController.GetPlayerMaxHealth();
        currentXpToNextLevel = GameManager.Instance.PlayerController.GetCurrentXpToNextLevel();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
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
                upgradesPanel.SetActive(true);
                GenerateUpgradesForPlayer();
                break;
        }
    }

    private void OnGameResume()
    {
        pausePanel.SetActive(false);
    }

    public void OnResumeButtonClicked()
    {
        GameManager.Instance.EventService.InvokeGameEnteredPlayStateEvent();
    }

    private void DecreaseHealth(int damage) => playerHealthBar.fillAmount -= (float) damage / currentMaxPlayerHealth;
    
    private void IncreaseXp() => playerXpBar.fillAmount += (float) currentXpToNextLevel / currentMaxPlayerHealth;

    private void UpdatePlayerLevelUI()
    {
        currentPlayerLevel += 1;
        playerXpBar.fillAmount = 0f;
        playerXpLevelText.text = $"{currentPlayerLevel}";
        upgradesPanel.SetActive(true);
    }

    private void GenerateUpgradesForPlayer()
    {
        FillUpgradeText<WeaponType>(weaponToBeUpgradedTextList, weapons);
        FillUpgradeText<UpgradeType>(upgradeTypeTextList, upgrades);
    }

    private void FillUpgradeText<T>(List<TextMeshProUGUI> textList, int[] dataArray) where T : Enum
    {
        ShuffleArray(dataArray);
        int i = 0, j = 0;
        for (; i < textList.Count && j < dataArray.Length; i++, j++)
            textList[i].text = Enum.GetName(typeof(T), dataArray[j]);
        
        if(i < textList.Count - 1)
            while (i < textList.Count)
            {
                textList[i].text = Enum.GetName(typeof(T), dataArray[j-1]);
                i++;
            }
    }
    
    private void ShuffleArray(int[] array)
    {
        Random random = new Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
