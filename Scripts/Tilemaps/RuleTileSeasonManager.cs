using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RuleTileSeasonManager : MonoBehaviour
{
    private string ruleTileFolderPath = "Sprites/Tilemaps/Outside/Terrain";

    private void Start()
    {
        UpdateRuleTiles();

        // Subscribe to the onSeasonChange event to update RuleTiles when the season changes
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onSeasonChange.AddListener(UpdateRuleTiles);
        }
    }

    private void UpdateRuleTiles()
    {
        TimeManager.Season currentSeason = TimeManager.Instance.GetCurrentSeason();
        string seasonFolder = GetSeasonFolder(currentSeason);

        string fullPath = $"{ruleTileFolderPath}/{seasonFolder}";
        RuleTile[] ruleTiles = Resources.LoadAll<RuleTile>(fullPath);

        foreach (RuleTile ruleTile in ruleTiles)
        {
            UpdateRuleTileSprites(ruleTile, currentSeason);
        }
    }

    private void UpdateRuleTileSprites(RuleTile ruleTile, TimeManager.Season currentSeason)
    {
        foreach (var tilingRule in ruleTile.m_TilingRules)
        {
            for (int i = 0; i < tilingRule.m_Sprites.Length; i++)
            {
                string newSpriteName = ReplaceSeasonInSpriteName(tilingRule.m_Sprites[i].name, currentSeason);
                string seasonFolder = GetSeasonFolder(currentSeason);
                string spritePath = $"{ruleTileFolderPath}/{seasonFolder}/{newSpriteName}";

                Sprite newSprite = Resources.Load<Sprite>(spritePath);
                if (newSprite != null)
                {
                    tilingRule.m_Sprites[i] = newSprite;
                }
                else
                {
                    Debug.LogWarning($"Sprite '{newSpriteName}' not found at path: {spritePath}");
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
}
