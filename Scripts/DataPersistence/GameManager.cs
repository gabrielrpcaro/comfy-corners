using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { MainMenu, Loading, Playing, Paused }
    public GameState CurrentGameState { get; private set; }

    public event Action<GameState> OnGameStateChanged;

    public GameObject UiPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ToggleUIPanel();
    }

    private void Update()
    {
        ToggleUIPanel();
    }

    public void SetGameState(GameState newGameState)
    {
        if (CurrentGameState != newGameState)
        {
            CurrentGameState = newGameState;
            OnGameStateChanged?.Invoke(newGameState);
        }
    }

    public void ToggleUIPanel()
    {
        if (CurrentGameState == GameState.Playing)
        {
            UiPanel.SetActive(true);
        } else {
            UiPanel.SetActive(false);
        }
    }
}