using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    public GameData currentGameData;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeNewGame()
    {
        currentGameData = new GameData();
    }

    public void LoadGame(GameData data)
    {
        currentGameData = data;
    }

    public void SaveGame()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    public void UpdateGameData(GameData data)
    {
        currentGameData = data;
    }
}