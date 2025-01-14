using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Button _StartButton;

    private void Awake()
    {
        gameObject.SetActive(true);

        _StartButton.onClick.AddListener(OnPlay);
    }

    private void OnPlay()
    {
        gameObject.SetActive(false);
        UIScreens.GetInstance().loginScreen.gameObject.SetActive(true);
    }
}
