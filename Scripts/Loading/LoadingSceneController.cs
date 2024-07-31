using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Loading UI")]
    [SerializeField] private Slider progressBar;
    
    private void Start()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Loading);
        StartCoroutine(LoadPlayerScene());
    }

    private IEnumerator LoadPlayerScene()
    {
        if (PlayerManager.instance.mainPlayerCustomization == null)
        {
            GameObject player = Instantiate(playerPrefab);
            PlayerManager.instance.InitializePlayer(player);
            DataPersistenceManager.instance.LoadGame();
        }

        string targetScene = SceneTransitionManager.TargetScene;
        Vector3 spawnPosition = SceneTransitionManager.GetSpawnPosition();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        } 

        progressBar.value = 1.0f;
        yield return StartCoroutine(SceneTransitionManager.instance.FadeIn());

        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        GameObject playerInstantiated = PlayerManager.instance.mainPlayerCustomization.gameObject;
         if (PlayerManager.instance.mainPlayerCustomization.transform.position == Vector3.zero)
        {
            playerInstantiated.transform.position = spawnPosition;
        }
        else
        {
            playerInstantiated.transform.position = PlayerManager.instance.mainPlayerCustomization.transform.position;
        }
        // DataPersistenceManager.instance.SaveGame();
        asyncLoad.allowSceneActivation = true;
        yield return StartCoroutine(SceneTransitionManager.instance.FadeOut());
    }
}