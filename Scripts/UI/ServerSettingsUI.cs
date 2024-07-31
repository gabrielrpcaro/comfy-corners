using UnityEngine;
using UnityEngine.UI;

public class ServerSettingsUI : MonoBehaviour
{
    // public InputField maxPlayersInput;
    // public Toggle passwordProtectedToggle;
    // public InputField passwordInput;
    public Button startServerButton;
    public GameObject serverSettingsPanel;

    void Start()
    {
        startServerButton.onClick.AddListener(StartServer);
    }

    void StartServer()
    {
        // int maxPlayers = int.Parse(maxPlayersInput.text);
        // bool passwordProtected = passwordProtectedToggle.isOn;
        // string password = passwordProtected ? passwordInput.text : string.Empty;

        // CustomNetworkManager.Instance.SetServerSettings(maxPlayers, passwordProtected, password);
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        CustomNetworkManager.Instance.StartHost();

        serverSettingsPanel.SetActive(false);
    }
}