using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif // Import the TextMeshPro namespace

public class PauseMenuManager : MonoBehaviour
{
    private static PauseMenuManager instance;
    public static PauseMenuManager Instance => instance;

    public GameObject pauseMenuPrefab; // Reference to the pause menu prefab
    private GameObject pauseMenuInstance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.MainMenu)
        {
            return;
        }
        if (pauseMenuInstance == null)
        {
            pauseMenuInstance = Instantiate(pauseMenuPrefab);
            pauseMenuInstance.transform.SetParent(transform, false); // Set as a child of the manager

            Transform pauseMenuPanel = pauseMenuInstance.transform.Find("PauseMenuPanel");
            if (pauseMenuPanel == null)
            {
                return;
            }

            Transform verticalGroup = pauseMenuPanel.Find("Vertical");
            Debug.Log("Finding Vertical group: " + (verticalGroup != null));

            if (verticalGroup != null)
            {
                Transform resumeButton = verticalGroup.Find("ResumeButton");
                Transform returnToTitleButton = verticalGroup.Find("ReturnTitleButton");
                Transform exitButton = verticalGroup.Find("ExitButton");
                Transform disconnectButton = verticalGroup.Find("DisconnectButton"); // New disconnect button

                if (resumeButton != null)
                {
                    resumeButton.GetComponent<Button>().onClick.AddListener(ResumeGame);
                }

                if (returnToTitleButton != null)
                {
                    returnToTitleButton.GetComponent<Button>().onClick.AddListener(ReturnToTitle);
                }

                if (exitButton != null)
                {
                    exitButton.GetComponent<Button>().onClick.AddListener(ExitGame);
                }

                if (disconnectButton != null)
                {
                    disconnectButton.GetComponent<Button>().onClick.AddListener(DisconnectFromServer);
                    disconnectButton.gameObject.SetActive(false);
                }

                Transform onlineStatusText = pauseMenuPanel.Find("OnlineStatusText");
                if (onlineStatusText != null)
                {
                    UpdateOnlineStatus(onlineStatusText.GetComponent<TMP_Text>(), disconnectButton.GetComponent<Button>());
                }
            }
        }

        pauseMenuInstance.SetActive(true);
        Time.timeScale = 0f;

        GameManager.Instance.SetGameState(GameManager.GameState.Paused);

        // Ensure the online status is updated when opening the menu
        Transform pauseMenuPanelCheck = pauseMenuInstance.transform.Find("PauseMenuPanel");
        Transform verticalGroupCheck = pauseMenuPanelCheck.Find("Vertical");
        Transform onlineStatusTextCheck = pauseMenuPanelCheck.Find("OnlineStatusText");
        if (onlineStatusTextCheck != null && verticalGroupCheck != null)
        {
            UpdateOnlineStatus(onlineStatusTextCheck.GetComponent<TMP_Text>(), verticalGroupCheck.Find("DisconnectButton").GetComponent<Button>());
        }
    }

    public void ResumeGame()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Paused)
        {
            return;
        }
        Time.timeScale = 1f;
        if (pauseMenuInstance != null)
        {
            pauseMenuInstance.SetActive(false);
        }
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        DataPersistenceManager.instance.SaveGame();
        PlayerManager.instance.DestroyPlayer();
        SceneManager.LoadScene("TitleScreen");
        GameManager.Instance.SetGameState(GameManager.GameState.MainMenu);
        pauseMenuInstance.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
                Application.Quit(); // original code to quit Unity player
        #endif
    }

    public void DisconnectFromServer()
    {
        // GameManager.Instance.SaveGame();
        if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
            UpdateOnlineStatus();
        }
        ReturnToTitle();
    }

    private void UpdateOnlineStatus(TMP_Text statusText = null, Button disconnectButton = null)
    {
        if (pauseMenuInstance == null) return;

        if (statusText == null)
        {
            statusText = pauseMenuInstance.transform.Find("PauseMenuPanel/OnlineStatusText").GetComponent<TMP_Text>();
        }
        if (disconnectButton == null)
        {
            disconnectButton = pauseMenuInstance.transform.Find("PauseMenuPanel/Vertical/DisconnectButton").GetComponent<Button>();
        }

        if (NetworkClient.isConnected)
        {
            statusText.text = "Online";
            disconnectButton.gameObject.SetActive(true);
        }
        else
        {
            statusText.text = "Offline";
            disconnectButton.gameObject.SetActive(false);
        }
    }
}
