using UnityEngine;
using UnityEngine.UI;

public class ServerListUI : MonoBehaviour
{
    public Transform serverListContainer;
    public GameObject serverListItemPrefab;
    public Button refreshButton;
    public GameObject serverListPanel;

    void Start()
    {
        refreshButton.onClick.AddListener(RefreshServerList);
    }

    void RefreshServerList()
    {
        // Implement server discovery and list refreshing here
        ClearExistingServers();
        PopulateServerList();
    }

    void ClearExistingServers()
    {
        foreach (Transform child in serverListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void PopulateServerList()
    {
        GameObject serverItem = Instantiate(serverListItemPrefab, serverListContainer);
        serverItem.GetComponentInChildren<Text>().text = "localhost";
        serverItem.GetComponent<Button>().onClick.AddListener(() => JoinServer("192.168.0.83"));
    }

    void JoinServer(string serverName)
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        CustomNetworkManager.Instance.networkAddress = serverName;
        CustomNetworkManager.Instance.StartClient();

        serverListPanel.SetActive(false);
    }
}