using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HighScoreText;

    private void Start()
    {
        HighScoreText.text = PlayerPrefs.GetInt("HIGH SCORE MAX LEVEL").ToString();
    }

    public void OnStartGameClicked() => SceneManager.LoadScene((int)SceneBuildIndices.GAME_SCENE);
    public void OnQuitGameClicked() => Application.Quit();
}
