using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject PlayButtonsContainer;
    public GameObject MainButtonsContainer;

    private void Start()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.MainMenu);
    }
    public void ExitGame()
    {
        // Quit the game
        Application.Quit();
        Debug.Log("Game is exiting"); // Note: This will not close the game in the editor
    }

    public void TogglePlayMenu()
    {
        PlayButtonsContainer.SetActive(!PlayButtonsContainer.activeSelf);
        MainButtonsContainer.SetActive(!MainButtonsContainer.activeSelf);

    }
}