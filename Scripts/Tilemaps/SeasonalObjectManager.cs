using System.Collections.Generic;
using UnityEngine;

public class SeasonalObjectManager : MonoBehaviour
{
    private List<GameObject> seasonalObjects;

    private void Start()
    {
        seasonalObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("SeasonalObject"));
        UpdateSeasonalSprites();

        // Subscribe to the onSeasonChange event to update sprites when the season changes
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onSeasonChange.AddListener(UpdateSeasonalSprites);
        }
    }

    private void UpdateSeasonalSprites()
    {
        TimeManager.Season currentSeason = TimeManager.Instance.GetCurrentSeason();
        string seasonFolder = GetSeasonFolder(currentSeason);

        foreach (GameObject obj in seasonalObjects)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                string spriteName = spriteRenderer.sprite.name;
                string type = CapitalizeFirstLetter(spriteName.Split('_')[0]);

                string newSpriteName = ReplaceSeasonInSpriteName(spriteName, currentSeason);
                string spritesheetPath = $"Sprites/Tilemaps/Outside/{type}/{seasonFolder}/{type.ToLower()}_{seasonFolder.ToLower()}";
                Sprite[] sprites = Resources.LoadAll<Sprite>(spritesheetPath);
                if (sprites.Length > 0)
                {
                    Sprite newSprite = System.Array.Find(sprites, sprite => sprite.name == newSpriteName);
                    if (newSprite != null)
                    {
                        spriteRenderer.sprite = newSprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Sprite '{newSpriteName}' not found in spritesheet at path: {spritesheetPath}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Spritesheet not found at path: {spritesheetPath}");
                }
            }
        }
    }

    private string GetSeasonFolder(TimeManager.Season season)
    {
        switch (season)
        {
            case TimeManager.Season.Summer:
                return "Summer";
            case TimeManager.Season.Winter:
                return "Winter";
            case TimeManager.Season.Autumn:
                return "Autumn";
            case TimeManager.Season.Spring:
                return "Spring";
            default:
                return "Summer";
        }
    }

    private string ReplaceSeasonInSpriteName(string spriteName, TimeManager.Season season)
    {
        string seasonString = season.ToString().ToLower();
        string[] seasons = { "summer", "winter", "autumn", "spring" };

        foreach (string s in seasons)
        {
            if (spriteName.Contains(s))
            {
                return spriteName.Replace(s, seasonString);
            }
        }

        return spriteName;
    }

    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
