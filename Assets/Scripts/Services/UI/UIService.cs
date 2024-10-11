using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = System.Random;

public class UIService : MonoBehaviour
{ 
    [SerializeField] private UpgradesListScriptableObject upgradesListScriptableObjectData;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject gameOverPanel;
    [FormerlySerializedAs("settingsPanel")] [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerXpBar;
    [FormerlySerializedAs("playerXpText")] [SerializeField] private TextMeshProUGUI playerXpLevelText;
    [FormerlySerializedAs("weaponToBeUpgradedText")] [SerializeField] private List<TextMeshProUGUI> weaponToBeUpgradedTextList;
    [FormerlySerializedAs("upgradeTypeText")] [SerializeField] private List<TextMeshProUGUI> upgradeTypeTextList;

    private PlayerController playerController;
    private Vector2 xpBarOriginalPos;
    private Vector2 healthBarOriginalPos;

    private bool xpBarShakeCoroutineActive;
    private bool healthBarShakeCoroutineActive;
    
    private float currentXpToNextLevel;
    
    private int maxPlayerHealth;

    private const float shakeFrequency = 100f;
    private const float shakeAmplitude = 5f;
    private const float shakeDuration = 0.5f;

    private UpgradeData[] upgradesData;

    public void Init()
    {
        SubscribeToEvents();
        playerXpBar.fillAmount = 0f;
        pausePanel.SetActive(false);
        upgradesPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        FillUpgradeCaches();
    }

    private void FillUpgradeCaches()
    {
        if (upgradesListScriptableObjectData.Upgrades.Count < 3)
        {
            Debug.LogError("THERE SHOULD BE ATLEAST 3 UPGRADES CREATED. " +
                           "MAKE SURE YOU HAVE CREATED ENOUGH UPGRADES USING THE UPGRADE CREATION TOOL.");
            EditorApplication.isPlaying = false;
        }
        upgradesData = new UpgradeData[upgradesListScriptableObjectData.Upgrades.Count];
        for(int i = 0; i < upgradesListScriptableObjectData.Upgrades.Count; i++)
            upgradesData[i] = upgradesListScriptableObjectData.Upgrades[i];
    }

    private void Start()
    {
        xpBarOriginalPos = playerXpBar.rectTransform.anchoredPosition; 
        playerController = GameManager.Instance.PlayerController;
        maxPlayerHealth = playerController.GetPlayerMaxHealth();
        currentXpToNextLevel = playerController.GetCurrentXpToNextLevel();
        playerXpLevelText.text = $"{playerController.CurrentPlayerLevel}";
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
        GameManager.Instance.EventService.OnPlayerDied += OnGameOver;
    }
    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState -= OnGamePause;
        GameManager.Instance.EventService.OnGameEnteredPlayState -= OnGameResume;
        GameManager.Instance.EventService.OnPlayerTookDamage -= DecreaseHealth;
        GameManager.Instance.EventService.OnPlayerPickedUpXp -= IncreaseXp;
        GameManager.Instance.EventService.OnPlayerDied -= OnGameOver;
    }
    private void Update()
    {
        UpdateHealthBar();
        AnimateBars();
    }

    private void AnimateBars()
    {
        if(xpBarShakeCoroutineActive)
            playerXpBar.rectTransform.anchoredPosition = new Vector2(xpBarOriginalPos.x + Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude, 
                xpBarOriginalPos.y);
        if(healthBarShakeCoroutineActive)
            playerHealthBar.rectTransform.anchoredPosition = new Vector2(healthBarOriginalPos.x + Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude, 
                healthBarOriginalPos.y);
    }

    private void UpdateHealthBar()
    {
        playerHealthBar.fillAmount = (float)playerController.GetCurrentHealthOfPlayer()/maxPlayerHealth;
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

    private void OnGameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
    }

    public void OnRestartButtonClicked()
    {
        GameManager.Instance.ReloadScene();
    }

    public void OnResumeButtonClicked()
    {
        GameManager.Instance.EventService.InvokeGameEnteredPlayStateEvent();
    }

    public void OnQuitButtonClicked()
    {
        GameManager.Instance.QuitGame();
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

    private void DecreaseHealth(int damage)
    { 
        if(!healthBarShakeCoroutineActive)
            StartCoroutine(nameof(ShakeHealthBarCoroutine));
    }

    private void IncreaseXp()
    {
        playerXpBar.fillAmount = playerController.GetCurrentPlayerXp() / currentXpToNextLevel;
        if(!xpBarShakeCoroutineActive)
            StartCoroutine(nameof(ShakeXpBarCoroutine));
    }

    private IEnumerator ShakeXpBarCoroutine()
    {
        xpBarShakeCoroutineActive = true;
        yield return new WaitForSecondsRealtime(shakeDuration);
        xpBarShakeCoroutineActive = false;
        playerXpBar.rectTransform.anchoredPosition = xpBarOriginalPos;
    }

    private IEnumerator ShakeHealthBarCoroutine()
    {
        healthBarShakeCoroutineActive = true;
        yield return new WaitForSecondsRealtime(shakeDuration);
        healthBarShakeCoroutineActive = false;
        playerHealthBar.rectTransform.anchoredPosition = healthBarOriginalPos;
    }

    public void UpdatePlayerLevelUI()
    {
        playerXpBar.fillAmount = 0f;
        playerXpLevelText.text = $"{playerController.CurrentPlayerLevel}";
        upgradesPanel.SetActive(true);
    }

    private void GenerateUpgradesForPlayer()
    {
        MiscFunctions.ShuffleArray(upgradesData);
        string weaponTypeText;
        string upgradeTypeText;
        
        for (int i = 0; i < weaponToBeUpgradedTextList.Count; i++)
        {
            weaponTypeText = String.Empty;
            upgradeTypeText = String.Empty;
            GenerateUpgradeText(ref weaponTypeText, ref upgradeTypeText, upgradesData[i]);
            weaponToBeUpgradedTextList[i].text = weaponTypeText;
            upgradeTypeTextList[i].text = upgradeTypeText;
        }
    }

    private void GenerateUpgradeText(ref string weaponTypeText,
        ref string upgradeTypeText,
        UpgradeData upgradeData)
    {
        upgradeTypeText += "Damage +" + upgradeData.Damage + "\n" +
                           "Attack Speed + " + upgradeData.AttackSpeed + "\n" +
                           "Knockback + " + upgradeData.Knockback + "\n" +
                           "Number + " + upgradeData.NumberOfWeapons + "\n";

        if (upgradeData.WeaponType == WeaponType.RANGED)
        {
            weaponTypeText = "RANGED WEAPON";
            upgradeTypeText += "Attack Spread + " + upgradeData.ProjectileSpread;
        }
        else
            weaponTypeText = "MELEE WEAPON";
    }

    public void OnUpgradeChosen(int upgradeChosen)
    {
        GameManager.Instance.playerWeaponsManager.
            UpgradeWeapons(upgradesData[upgradeChosen]);   
        GameManager.Instance.EventService.InvokePlayerSelectedUpgradeEvent();
    }
}
