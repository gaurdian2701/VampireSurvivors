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
    private int numberOfWeaponTypes = Enum.GetNames(typeof(WeaponType)).Length - 1;
    private int numberOfUpgradeTypes = Enum.GetNames(typeof(UpgradeType)).Length - 1;
    
    private Random random = new Random();

    public void Init()
    {
        SubscribeToEvents();
        playerXpBar.fillAmount = 0f;
        currentPlayerLevel = 1;
        pausePanel.SetActive(false);
        upgradesPanel.SetActive(false);
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
        for(int i = 0; i < weaponToBeUpgradedTextList.Count; i++)
            weaponToBeUpgradedTextList[i].text = GenerateRandomWeaponTypeForUpgrade().ToString();

        for(int i = 0; i < upgradeTypeTextList.Count; i++)
            upgradeTypeTextList[i].text = GenerateRandomUpgradeType().ToString();
    }

    private WeaponType GenerateRandomWeaponTypeForUpgrade() =>
        (WeaponType)random.Next(0, numberOfWeaponTypes);

    private UpgradeType GenerateRandomUpgradeType() =>
        (UpgradeType)random.Next(0, numberOfUpgradeTypes);
}
