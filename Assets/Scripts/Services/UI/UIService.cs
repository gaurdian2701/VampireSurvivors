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

    public void Init()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState += OnGamePause;
        GameManager.Instance.EventService.OnGameEnteredPlayState += OnGameResume;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState -= OnGamePause;
        GameManager.Instance.EventService.OnGameEnteredPlayState -= OnGameResume;
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
}
