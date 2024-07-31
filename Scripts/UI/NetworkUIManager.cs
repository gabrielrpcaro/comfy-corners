using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkUIManager : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public GameObject loadGamePanel;
    public GameObject characterCreationPanel;
    public GameObject serverSettingsPanel;
    public GameObject serverListPanel;

    void Start()
    {
        hostButton.onClick.AddListener(() => OpenLoadGamePanel(true, true));
        joinButton.onClick.AddListener(() => OpenLoadGamePanel(true, false));
    }

    void OpenLoadGamePanel(bool multiplayer, bool host)
    {
        loadGamePanel.SetActive(true);
        // GameManager.Instance.SetOnlineData(multiplayer, host) ;
    }

    public void StartNetworkGame()
    {
        loadGamePanel.SetActive(false);
        characterCreationPanel.SetActive(false);

        // if (GameManager.Instance.isHost)
        // {
        //     serverSettingsPanel.SetActive(true);
        //     serverListPanel.SetActive(false);
        // }
        // else
        // {
        //     serverListPanel.SetActive(true);
        //     serverSettingsPanel.SetActive(false);
        // }
    }

    public void StartHostGame()
    {
        try
        {
            CustomNetworkManager.Instance.StartHost();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start host: " + e.Message);
            // Provide UI feedback to the user
        }
    }

    public void StartClientGame()
    {
        try
        {
            string ipAddress = "192.168.0.83";
            CustomNetworkManager.Instance.SetNetworkAddress(ipAddress);
            CustomNetworkManager.Instance.StartClient();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start client: " + e.Message);
            // Provide UI feedback to the user
        }
    }
}
