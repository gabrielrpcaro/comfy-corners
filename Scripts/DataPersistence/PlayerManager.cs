using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerManager : MonoBehaviour, IDataPersistence
{
    public static PlayerManager instance;

    public PlayerCustomization mainPlayerCustomization;
    private bool isSitting { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadData(GameData data)
    {
        if (mainPlayerCustomization != null)
        {
            foreach (var bodyPartData in data.bodyPartCustomizations)
            {
                var bodyPart = System.Array.Find(mainPlayerCustomization.bodyParts, part => part.partType == bodyPartData.partType);
                if (bodyPart != null)
                {
                    bodyPart.style = bodyPartData.style;
                    bodyPart.color = bodyPartData.color;
                    mainPlayerCustomization.InitializeBodyPart(bodyPart);
                }
            }

            mainPlayerCustomization.transform.position = data.playerPosition;
            mainPlayerCustomization.SetSittingState(data.isPlayerSitting);
        }
    }

    public void SaveData(GameData data)
    {
        if (mainPlayerCustomization != null)
        {
            foreach (var bodyPart in mainPlayerCustomization.bodyParts)
            {
                data.SetBodyPartCustomization(bodyPart.partType, bodyPart.style, bodyPart.color);
            }

            if(GameManager.Instance.CurrentGameState == GameManager.GameState.Playing || GameManager.Instance.CurrentGameState == GameManager.GameState.Paused) {
                data.playerPosition = mainPlayerCustomization.transform.position;
            }
            data.isPlayerSitting = mainPlayerCustomization.isSitting;
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != "TitleScreen" && currentScene != "LoadingScene")
            {
                data.playerScene = currentScene;
            }
            
        }
    }

    public void InitializePlayer(GameObject player)
    {
        mainPlayerCustomization = player.GetComponent<PlayerCustomization>();
        DontDestroyOnLoad(player);
    }

    public void ApplyCustomizationData(PlayerCustomization playerCustomization, GameData data)
    {
        foreach (var bodyPartData in data.bodyPartCustomizations)
        {
            var bodyPart = System.Array.Find(playerCustomization.bodyParts, part => part.partType == bodyPartData.partType);
            if (bodyPart != null)
            {
                bodyPart.style = bodyPartData.style;
                bodyPart.color = bodyPartData.color;
                playerCustomization.InitializeBodyPart(bodyPart);
            }
        }
    }

    public void SetPlayerSittingState(bool isSitting)
    {
        if (mainPlayerCustomization != null)
        {
            mainPlayerCustomization.SetSittingState(isSitting);
        }
    }

    public void DestroyPlayer()
    {
        if (mainPlayerCustomization != null)
        {
            Destroy(mainPlayerCustomization.gameObject);
            mainPlayerCustomization = null;
        }
    }

    public void SetVirtualCameraFollow(CinemachineVirtualCamera virtualCamera)
    {
        if (virtualCamera != null && mainPlayerCustomization != null)
        {
            virtualCamera.Follow = mainPlayerCustomization.transform;
        }
    }

}