using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI saveDateText;

    [Header("Clear Data Button")]
    [SerializeField] private Button clearButton;

    [Header("Player Frame")]
    [SerializeField] private GameObject playerPrefab;

    public bool hasData { get; private set; } = false;

    private Button saveSlotButton;

    private void Awake() 
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data) 
    {
        // there's no data for this profileId
        if (data == null) 
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        // there is data for this profileId
        else 
        {
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            nameText.text = data.playerName;
            saveDateText.text = System.DateTime.FromBinary(data.lastUpdated).ToString("g");
            StyleFrame(data);
        }
    }

    public string GetProfileId() 
    {
        return this.profileId;
    }

    private void StyleFrame(GameData data)
    {
        foreach (var bodyPart in data.bodyPartCustomizations)
        {
            string resourcePath = $"Sprites/Player/{bodyPart.partType}/{bodyPart.style}/{bodyPart.color}/static";
            Sprite staticSprite = Resources.Load<Sprite>(resourcePath);
            Transform childTransform = playerPrefab.transform.Find(bodyPart.partType.ToString());
            Image childImage = childTransform.GetComponent<Image>();
            childImage.sprite = staticSprite;
        }
    }
}