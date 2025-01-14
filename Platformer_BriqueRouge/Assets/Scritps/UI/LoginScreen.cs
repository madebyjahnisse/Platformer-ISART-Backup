using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{

    [SerializeField] private Button _StartButton;
    [SerializeField] private Button _QuitButton;

    private void Awake()
    {
        gameObject.SetActive(false);

        _StartButton.onClick.AddListener(OnStart);
        _QuitButton.onClick.AddListener(OnQuit);
    }

    private void OnStart()
    {
        gameObject.SetActive(false);
        UIScreens.GetInstance().mainMenu.gameObject.SetActive(true);
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
