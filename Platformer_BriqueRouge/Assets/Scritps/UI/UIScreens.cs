using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreens : MonoBehaviour
{
    [SerializeField] public TitleScreen titleScreen;
    [SerializeField] public LoginScreen loginScreen;
    [SerializeField] public MainMenu mainMenu;

    public static UIScreens instance;
    private void Awake()
    { instance = this; }
    public static UIScreens GetInstance()
    { return instance; }
}
