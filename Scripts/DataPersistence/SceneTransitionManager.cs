using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;

    [Header("Transition Settings")]
    [SerializeField] private GameObject transitionCanvasPrefab;
    [SerializeField] private float fadeDuration = 0.5f;

    public static string TargetScene { get; set; }
    private static Vector3 targetSpawnPosition { get; set; }

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSpawnPosition(Vector3 position)
    {
        targetSpawnPosition = position;
    }

    public void TransitionToLoadingScene(string targetScene)
    {
        // DataPersistenceManager.instance.SaveGame();
        TargetScene = targetScene;
        StartCoroutine(TransitionAndLoadScene());
    }


    public IEnumerator TransitionAndLoadScene()
    {
        yield return StartCoroutine(FadeIn());
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        if (transitionCanvasPrefab != null)
        {
            GameObject canvasObj = Instantiate(transitionCanvasPrefab);  
            canvasGroup = canvasObj.GetComponentInChildren<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                while (canvasGroup.alpha < 1f)
                {
                    canvasGroup.alpha += Time.deltaTime / fadeDuration;
                    yield return null;
                }
            }
        }
    }

    public IEnumerator FadeOut()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime / fadeDuration;
                yield return null;
            }

            Destroy(canvasGroup.gameObject);
        }
    }

    public static Vector3 GetSpawnPosition()
    {
        return targetSpawnPosition;
    }
}