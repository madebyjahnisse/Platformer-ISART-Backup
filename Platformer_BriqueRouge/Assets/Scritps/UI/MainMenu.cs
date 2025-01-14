using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _ContinueButton;
    [SerializeField] private Button _NewGameButton;
    [SerializeField] private Button _LeaderBoardButton;
    [SerializeField] private Button _SettingsButton;
    [SerializeField] private Button _BackButton;
    [SerializeField] private Button _QuitButton;

    private void Awake()
    {
        gameObject.SetActive(false);

        _ContinueButton.onClick.AddListener(OnContinue);
        _NewGameButton.onClick.AddListener(OnNewGame);
        _LeaderBoardButton.onClick.AddListener(OnLeaderBoard);
        _SettingsButton.onClick.AddListener(OnSettings);
        _BackButton.onClick.AddListener(OnBack);
        _QuitButton.onClick.AddListener(OnQuit);
    }

    private void OnContinue()
    {
        gameObject.SetActive(false);
    }
    private void OnNewGame()
    {
        gameObject.SetActive(false);
    }
    private void OnLeaderBoard()
    {
        gameObject.SetActive(false);
    }
    private void OnSettings()
    {
        gameObject.SetActive(false);
    }
    private void OnBack()
    {
        gameObject.SetActive(false);
        UIScreens.GetInstance().loginScreen.gameObject.SetActive(true);
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
