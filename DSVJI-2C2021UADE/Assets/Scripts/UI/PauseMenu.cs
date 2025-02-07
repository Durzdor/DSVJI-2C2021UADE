﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #region SerializedFields
#pragma warning disable 649
    [SerializeField] private GameObject menuGameObject;
    [SerializeField] private List<Button> menuButtons;
    [SerializeField] private List<GameObject> menuScreens;
    [SerializeField] private TextMeshProUGUI controlsText;
    
#pragma warning restore 649
    #endregion

    private Character _character;
    private const string MenuScene = "MainMenu"; // menu scene name

    private void Start()
    {
        _character = GetComponentInParent<Character>();
        _character.OnCharacterPause += PauseMenuActivation;
        ButtonListeners();
        ControlsTextUpdate();
    }
    
    private void ButtonListeners()
    {
        // button order: resume, controls, options, help, quit menu, quit app, return
        menuButtons[0].onClick.AddListener(PauseMenuActivation);
        menuButtons[1].onClick.AddListener(delegate { SwitchPauseMenuScreen(1); });
        menuButtons[2].onClick.AddListener(delegate { SwitchPauseMenuScreen(2); });
        menuButtons[3].onClick.AddListener(delegate { SwitchPauseMenuScreen(3); });
        menuButtons[4].onClick.AddListener(QuitToMenu);
        menuButtons[5].onClick.AddListener(QuitToDesktop);
        menuButtons[6].onClick.AddListener(delegate { SwitchPauseMenuScreen(0); });
    }

    private void ControlsTextUpdate()
    {
        var movement = "WASD - Movement\n";
        var jump = $"{_character.Input.KeyBindData.jump} - Jump\n";
        var run = $"{_character.Input.KeyBindData.changeSpeed} - Run\n";
        var swap = $"{_character.Input.KeyBindData.switchCharacter} - Switch Character\n";
        var interact = $"{_character.Input.KeyBindData.interact} - Interact\n";
        var pause = $"{_character.Input.KeyBindData.pause} - Pause\n";
        var zoom = $"{_character.Input.KeyBindData.mouseWheelAxis} - Zoom\n";
        controlsText.text = $"{movement}{jump}{run}{swap}{interact}{pause}{zoom}";
    }
    
    private void PauseMenuActivation()
    {
        SwitchPauseMenuScreen(0);
        var currentState = menuGameObject.activeInHierarchy;
        menuGameObject.SetActive(!currentState);
        GameStatus.ChangeGameStatus(!currentState ? GameState.Paused : GameState.Playing);
        _character.IsAnimationLocked = !currentState;
    }
    
    private void SwitchPauseMenuScreen(int screenToShow)
    {
        for (int i = 0; i < menuScreens.Count; i++)
        {
            menuScreens[i].gameObject.SetActive(i == screenToShow);
        }
    }
    
    private void QuitToMenu()
    {
        SceneManager.LoadSceneAsync(MenuScene);
        SwitchPauseMenuScreen(0);
        PauseMenuActivation();
        GameStatus.ChangeGameStatus(GameState.Playing);
    }
    
    private void QuitToDesktop()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_STANDALONE_WIN
        Application.Quit();
#endif
    }
}
