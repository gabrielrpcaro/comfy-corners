using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCreationPanel : MonoBehaviour
{
    [Header("Player Creation Fields")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button startNewGameButton;

    [Header("Customization Config")]
    [SerializeField] private BodyPartConfig bodyPartConfig;
    [SerializeField] private BodyPartCustomization playerCustomizationData;
    [SerializeField] private PlayerCustomization playerCustomization;

    [Header("Static Sprites")]
    [SerializeField] private Button headButton;
    [SerializeField] private Button accessoryButton;
    [SerializeField] private Button hairButton;
    [SerializeField] private Button torsoButton;
    [SerializeField] private Button legsButton;
    [SerializeField] private Button feetButton;
    [SerializeField] private Button bodyButton;

    [Header("Body Part Changers")]
    [SerializeField] private GameObject headOptions;
    [SerializeField] private GameObject accessoryOptions;
    [SerializeField] private GameObject hairOptions;
    [SerializeField] private GameObject torsoOptions;
    [SerializeField] private GameObject legsOptions;
    [SerializeField] private GameObject feetOptions;
    [SerializeField] private GameObject bodyOptions;

    [Header("Color Button Prefab")]
    [SerializeField] private GameObject colorButtonPrefab;

    private Dictionary<PlayerCustomization.BodyPartType, Button> bodyPartButtons;
    private Dictionary<PlayerCustomization.BodyPartType, (string style, string color)> originalStyles;
    private Dictionary<PlayerCustomization.BodyPartType, GameObject> bodyPartChangers;

    private void Awake()
    {
        startNewGameButton.onClick.AddListener(OnStartNewGameClicked);

        bodyPartButtons = new Dictionary<PlayerCustomization.BodyPartType, Button>
        {
            { PlayerCustomization.BodyPartType.Head, headButton },
            { PlayerCustomization.BodyPartType.Acessory, accessoryButton },
            { PlayerCustomization.BodyPartType.Hair, hairButton },
            { PlayerCustomization.BodyPartType.Torso, torsoButton },
            { PlayerCustomization.BodyPartType.Legs, legsButton },
            { PlayerCustomization.BodyPartType.Feet, feetButton },
            { PlayerCustomization.BodyPartType.Body, bodyButton }
        };

        bodyPartChangers = new Dictionary<PlayerCustomization.BodyPartType, GameObject>
        {
            { PlayerCustomization.BodyPartType.Head, headOptions },
            { PlayerCustomization.BodyPartType.Acessory, accessoryOptions },
            { PlayerCustomization.BodyPartType.Hair, hairOptions },
            { PlayerCustomization.BodyPartType.Torso, torsoOptions },
            { PlayerCustomization.BodyPartType.Legs, legsOptions },
            { PlayerCustomization.BodyPartType.Feet, feetOptions },
            { PlayerCustomization.BodyPartType.Body, bodyOptions }
        };

        originalStyles = new Dictionary<PlayerCustomization.BodyPartType, (string style, string color)>();
    }

    private void Start()
    {
        RandomizePlayerCustomization(false);
        UpdateButtonImages();
        AddButtonListeners();
        AddBodyPartChangersListeners();
        CreateColorButtons();
    }

    private void OnStartNewGameClicked()
    {
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();
        gameData.playerName = playerNameInputField.text;
        gameData.playerScene = "Principal";
        foreach (var bodyPart in playerCustomization.bodyParts)
        {
            gameData.SetBodyPartCustomization(bodyPart.partType, bodyPart.style, bodyPart.color);
        }

        DataPersistenceManager.instance.SaveGame();
        SceneTransitionManager.instance.TransitionToLoadingScene("Principal");
    }

    public void OpenPanel()
    {
        this.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }

    public void RandomizePlayerCustomization(bool onlyColors)
    {
        if (!onlyColors) { RandomizeStylesAndColors(); CreateColorButtons(); } else { RandomizeColorsOnly(); }
        playerCustomization.ApplyCustomization(playerCustomizationData);
        UpdateButtonImages();
    }

    private void RandomizeStylesAndColors()
    {
        playerCustomizationData.bodyParts.Clear();
        string skinColor = "";

        foreach (var bodyPart in bodyPartConfig.bodyParts)
        {
            var randomStyle = bodyPart.styles[Random.Range(0, bodyPart.styles.Count)];
            var randomColor = randomStyle.colors[Random.Range(0, randomStyle.colors.Count)];

            randomColor = EnsureMatchingHeadAndBodyColor(bodyPart.bodyPartType, randomColor, ref skinColor);

            playerCustomizationData.bodyParts.Add(new BodyPartCustomization.BodyPartData
            {
                partType = bodyPart.bodyPartType,
                style = randomStyle.styleName,
                color = randomColor
            });
        }
    }

    private void RandomizeColorsOnly()
    {
        string skinColor = "";

        foreach (var bodyPartData in playerCustomizationData.bodyParts)
        {
            var bodyPartConfigData = bodyPartConfig.bodyParts.Find(bp => bp.bodyPartType == bodyPartData.partType);
            if (bodyPartConfigData != null)
            {
                var selectedStyle = bodyPartConfigData.styles.Find(style => style.styleName == bodyPartData.style);
                if (selectedStyle != null)
                {
                    var randomColor = selectedStyle.colors[Random.Range(0, selectedStyle.colors.Count)];

                    randomColor = EnsureMatchingHeadAndBodyColor(bodyPartData.partType, randomColor, ref skinColor);

                    bodyPartData.color = randomColor;
                }
            }
        }
    }

    private string EnsureMatchingHeadAndBodyColor(PlayerCustomization.BodyPartType partType, string randomColor, ref string skinColor)
    {
        if (partType == PlayerCustomization.BodyPartType.Head)
        {
            skinColor = randomColor;
        }
        else if (partType == PlayerCustomization.BodyPartType.Body && !string.IsNullOrEmpty(skinColor))
        {
            randomColor = skinColor;
        }
        return randomColor;
    }

    private void UpdateButtonImages()
    {
        foreach (var bodyPart in playerCustomization.bodyParts)
        {
            if (bodyPartButtons.TryGetValue(bodyPart.partType, out var button))
            {
                string resourcePath = $"Sprites/Player/{bodyPart.partType}/{bodyPart.style}/{bodyPart.color}/static";
                Sprite staticSprite = Resources.Load<Sprite>(resourcePath);
                if (staticSprite != null)
                {
                    Image childImage = button.transform.GetChild(0).GetComponent<Image>();
                    if (childImage != null && childImage != button.image)
                    {
                        childImage.sprite = staticSprite;
                    }
                }
            }
        }
    }

    private void AddButtonListeners()
    {
        foreach (var kvp in bodyPartButtons)
        {
            var bodyPartType = kvp.Key;
            var button = kvp.Value;

            if (bodyPartType == PlayerCustomization.BodyPartType.Head || bodyPartType == PlayerCustomization.BodyPartType.Body)
                continue;

            button.onClick.AddListener(() => ToggleBodyPartStyle(bodyPartType));
        }
    }

    private void ToggleBodyPartStyle(PlayerCustomization.BodyPartType partType)
    {
        var bodyPartData = playerCustomizationData.bodyParts.Find(bp => bp.partType == partType);
        if (bodyPartData != null)
        {
            if (originalStyles.TryGetValue(partType, out var originalStyle))
            {
                // Toggle back to the original style
                bodyPartData.style = originalStyle.style;
                bodyPartData.color = originalStyle.color;
                originalStyles.Remove(partType);
            }
            else
            {
                // Save the current style and set to default
                originalStyles[partType] = (bodyPartData.style, bodyPartData.color);
                bodyPartData.style = "Blank";
                bodyPartData.color = "Default";
            }

            // Update the button image
            UpdateButtonImage(partType);
            CreateColorButton(partType);
            playerCustomization.ApplyCustomization(playerCustomizationData);
        }
    }

    private void UpdateButtonImage(PlayerCustomization.BodyPartType partType)
    {
        var bodyPartData = playerCustomizationData.bodyParts.Find(bp => bp.partType == partType);
        if (bodyPartData != null && bodyPartButtons.TryGetValue(partType, out var button))
        {
            string resourcePath = $"Sprites/Player/{bodyPartData.partType}/{bodyPartData.style}/{bodyPartData.color}/static";
            Sprite staticSprite = Resources.Load<Sprite>(resourcePath);
            if (staticSprite != null)
            {
                Image childImage = button.transform.GetChild(0).GetComponent<Image>();
                if (childImage != null && childImage != button.image)
                {
                    childImage.sprite = staticSprite;
                }
            }
        }
    }

    private void AddBodyPartChangersListeners()
    {
        foreach (var kvp in bodyPartChangers)
        {
            var bodyPartType = kvp.Key;
            var options = kvp.Value;

            var leftArrow = options.transform.Find("Option/LeftArrow").GetComponent<Button>();
            var rightArrow = options.transform.Find("Option/RightArrow").GetComponent<Button>();

            leftArrow.onClick.AddListener(() => ChangeBodyPartStyle(bodyPartType, -1));
            rightArrow.onClick.AddListener(() => ChangeBodyPartStyle(bodyPartType, 1));
        }
    }

    private void ChangeBodyPartStyle(PlayerCustomization.BodyPartType partType, int direction)
    {
        var bodyPartData = playerCustomizationData.bodyParts.Find(bp => bp.partType == partType);
        if (bodyPartData != null)
        {
            var bodyPartConfigData = bodyPartConfig.bodyParts.Find(bp => bp.bodyPartType == partType);
            if (bodyPartConfigData != null)
            {
                var currentIndex = bodyPartConfigData.styles.FindIndex(style => style.styleName == bodyPartData.style);
                if (currentIndex != -1)
                {
                    var newIndex = (currentIndex + direction + bodyPartConfigData.styles.Count) % bodyPartConfigData.styles.Count;
                    var newStyle = bodyPartConfigData.styles[newIndex];

                    if (newStyle.colors.Contains(bodyPartData.color))
                    {
                        bodyPartData.style = newStyle.styleName;
                    }
                    else
                    {
                        bodyPartData.style = newStyle.styleName;
                        bodyPartData.color = newStyle.colors[Random.Range(0, newStyle.colors.Count)];
                    }

                    UpdateButtonImage(partType);
                    CreateColorButton(partType);
                    playerCustomization.ApplyCustomization(playerCustomizationData);
                }
            }
        }
    }

    private void CreateColorButtons()
    {
        foreach (var kvp in bodyPartChangers)
        {
            CreateColorButton(kvp.Key);
        }
    }

    private void CreateColorButton(PlayerCustomization.BodyPartType bodyPartType)
    {
        if (bodyPartChangers.TryGetValue(bodyPartType, out var options))
        {
            var colorsParent = options.transform.Find("Colors");
            if (colorsParent != null)
            {
                // Clear existing color buttons
                foreach (Transform child in colorsParent)
                {
                    Destroy(child.gameObject);
                }

                // Create new color buttons
                var bodyPartData = playerCustomizationData.bodyParts.Find(bp => bp.partType == bodyPartType);
                var bodyPartConfigData = bodyPartConfig.bodyParts.Find(bp => bp.bodyPartType == bodyPartType);

                if (bodyPartData != null && bodyPartConfigData != null)
                {
                    var selectedStyle = bodyPartConfigData.styles.Find(style => style.styleName == bodyPartData.style);
                    if (selectedStyle != null)
                    {
                        for (int i = 0; i < selectedStyle.colors.Count; i++)
                        {
                            var color = selectedStyle.colors[i];
                            var dominantColorHex = selectedStyle.dominantColorsHex[i];

                            GameObject colorButton = Instantiate(colorButtonPrefab, colorsParent);
                            var button = colorButton.GetComponent<Button>();
                            var image = colorButton.GetComponent<Image>();
                            image.color = HexToColor(dominantColorHex);

                            button.onClick.AddListener(() => ChangeBodyPartColor(bodyPartType, color));
                        }
                    }
                }
            }
        }
    }

    private void ChangeBodyPartColor(PlayerCustomization.BodyPartType partType, string color)
    {
        var bodyPartData = playerCustomizationData.bodyParts.Find(bp => bp.partType == partType);
        if (bodyPartData != null)
        {
            bodyPartData.color = color;
            if (partType == PlayerCustomization.BodyPartType.Head)
            {
                var bodyData = playerCustomizationData.bodyParts.Find(bp => bp.partType == PlayerCustomization.BodyPartType.Body);
                if (bodyData != null)
                {
                    bodyData.color = color;
                    UpdateButtonImage(PlayerCustomization.BodyPartType.Body);
                }
            }
            else if (partType == PlayerCustomization.BodyPartType.Body)
            {
                var headData = playerCustomizationData.bodyParts.Find(bp => bp.partType == PlayerCustomization.BodyPartType.Head);
                if (headData != null)
                {
                    headData.color = color;
                    UpdateButtonImage(PlayerCustomization.BodyPartType.Head);
                }
            }

            UpdateButtonImage(partType);
            playerCustomization.ApplyCustomization(playerCustomizationData);
        }
    }

    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white;
    }
}