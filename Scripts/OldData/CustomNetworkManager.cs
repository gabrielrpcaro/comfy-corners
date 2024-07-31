using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance { get; private set; }

    public override void Awake()
    {
        // Ensure singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }

    public void SetNetworkAddress(string address)
    {
        networkAddress = address;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        EnsureSpatialHashingInterestManagement();
    }

    private void EnsureSpatialHashingInterestManagement()
    {
        if (gameObject.GetComponent<SpatialHashingInterestManagement>() == null)
        {
            gameObject.AddComponent<SpatialHashingInterestManagement>();
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {

        Transform startPos = GetStartPosition();
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

    }

    // public override void OnServerReady(NetworkConnectionToClient conn)
    // {
    //     base.OnServerReady(conn);
    //     Debug.Log("Connection is ready. Initializing customization...");
    //     InitializePlayerCustomization(conn);
    // }

    // private PlayerData GetPlayerDataForConnection(NetworkConnectionToClient conn)
    // {
    //     int slotId = PlayerPrefs.GetInt("SelectedSlotId");
    //     PlayerData playerData = SaveLoadManager.Instance.LoadGame(slotId);
    //     if (playerData == null)
    //     {
    //         Debug.LogError("Failed to load player data for slot ID: " + slotId);
    //     }
    //     return playerData;
    // }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Handle player disconnection
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        // Cleanup when the server stops
        base.OnStopServer();
    }

    public void ChangeScene(string sceneName)
    {
        // Handle scene changes
        ServerChangeScene(sceneName);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        // Reposition players or handle additional setup in the new scene
        base.OnServerChangeScene(newSceneName);
    }
}
