using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIService : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerXpBar;
    [FormerlySerializedAs("playerXpText")] [SerializeField] private TextMeshProUGUI playerXpLevelText;

    private int currentMaxPlayerHealth;
    private int currentXpToNextLevel;
    private int currentPlayerLevel;

    public void Init()
    {
        SubscribeToEvents();
        playerXpBar.fillAmount = 0f;
        currentPlayerLevel = 1;
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
        pausePanel.SetActive(true);
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
    }
}
