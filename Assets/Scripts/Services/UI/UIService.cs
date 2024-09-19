using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIService : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Image playerHealthBar;

    private int maxPlayerHealth = 100;

    public void Init()
    {
        SubscribeToEvents();
    }

    private void Start()
    {
        maxPlayerHealth = GameManager.Instance.PlayerController.GetPlayerMaxHealth();
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
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState -= OnGamePause;
        GameManager.Instance.EventService.OnGameEnteredPlayState -= OnGameResume;
        GameManager.Instance.EventService.OnPlayerTookDamage -= DecreaseHealth;
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

    private void DecreaseHealth(int damage)
    {
        playerHealthBar.fillAmount -= (float) damage / maxPlayerHealth;
    }
}
