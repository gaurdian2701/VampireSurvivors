using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXpController
{
    public int currentXpToNextLevel { get; private set; }
    public int currentXp { get; private set; }
    private int maxLevel;
    
    private static int xpToNextLevelIncreaseRate = 10;

    public PlayerXpController(PlayerXpControllerScriptableObject playerXpControllerScriptableObject)
    {
        currentXpToNextLevel = playerXpControllerScriptableObject.StartingPlayerXpToNextLevel;
        maxLevel = playerXpControllerScriptableObject.PlayerMaxLevel;
        currentXp = 0;
        SubscribeToEvents();
    }

    ~PlayerXpController()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnPlayerPickedUpXp += IncreaseXp;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnPlayerPickedUpXp -= IncreaseXp;
    }

    private void IncreaseXp()
    {
        currentXp++;
        if (currentXp >= currentXpToNextLevel)
        {
            currentXp = 0;
            currentXpToNextLevel += xpToNextLevelIncreaseRate;
            GameManager.Instance.EventService.InvokePLayerLevelledUpEvent();
        }
    }
}
