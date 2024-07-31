using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Popup")]
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    [Header("Player Creation Panel")]
    [SerializeField] private PlayerCreationPanel playerCreationPanel;

    private SaveSlot[] saveSlots;

    private void Awake() 
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot) 
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
        if (saveSlot.hasData)
        {
            DataPersistenceManager.instance.LoadGame();
            SaveGameAndLoadScene();
        }
        else
        {
            DataPersistenceManager.instance.NewGame();
            OpenPlayerCreationPanel();
        }
    }

    private void SaveGameAndLoadScene() 
    {
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();
        PlayerManager.instance.SetPlayerSittingState(true);
        DataPersistenceManager.instance.SaveGame();
        string targetScene = gameData.playerScene;

        SceneTransitionManager.instance.TransitionToLoadingScene(targetScene);
    }

    public void OnClearClicked(SaveSlot saveSlot) 
    {
        confirmationPopupMenu.ActivateMenu(
            // function to execute if we select 'yes'
            () => {
                DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
                ActivateMenu();
            },
            // function to execute if we select 'cancel'
            () => {
                ActivateMenu();
            }
        );
    }

    public void ActivateMenu() 
    {
        // set this menu to be active
        this.gameObject.SetActive(true);

        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();
        Debug.Log("Number of profiles loaded: " + profilesGameData.Count);


        // loop through each save slot in the UI and set the content appropriately
        foreach (SaveSlot saveSlot in saveSlots) 
        {
            GameData profileData;
            bool found = profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            Debug.Log("Save Slot Profile ID: " + saveSlot.GetProfileId() + " - Found: " + found);

            if (profileData == null)
            {
                Debug.LogWarning("Profile data is null for Profile ID: " + saveSlot.GetProfileId());
            }

            saveSlot.SetData(profileData);
        }
    }

    private void OpenPlayerCreationPanel()
    {
        playerCreationPanel.OpenPanel();
        this.gameObject.SetActive(false);
    }
}