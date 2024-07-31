using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SeasonalTilemapManager : MonoBehaviour
{
    private List<Tilemap> seasonalTilemaps;

    private void Start()
    {
        seasonalTilemaps = new List<Tilemap>(FindObjectsOfType<Tilemap>());
        seasonalTilemaps.RemoveAll(tilemap => tilemap.tag != "SeasonalTilemap");
        UpdateSeasonalTilemaps();

        // Subscribe to the onSeasonChange event to update tilemaps when the season changes
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onSeasonChange.AddListener(UpdateSeasonalTilemaps);
        }
    }

    private void UpdateSeasonalTilemaps()
    {
        TimeManager.Season currentSeason = TimeManager.Instance.GetCurrentSeason();
        string seasonFolder = GetSeasonFolder(currentSeason);

        foreach (Tilemap tilemap in seasonalTilemaps)
        {
            string type = CapitalizeFirstLetter(tilemap.name.Split('_')[0]);

            string spritesheetPath = $"Sprites/Tilemaps/Outside/{type}/{seasonFolder}/{type.ToLower()}_{seasonFolder.ToLower()}";
            Sprite[] sprites = Resources.LoadAll<Sprite>(spritesheetPath);

            if (sprites.Length > 0)
            {
                UpdateTilemapSprites(tilemap, sprites, currentSeason);
            }
            else
            {
                Debug.LogWarning($"Spritesheet not found at path: {spritesheetPath}");
            }
        }
    }

    private void UpdateTilemapSprites(Tilemap tilemap, Sprite[] sprites, TimeManager.Season currentSeason)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile is Tile)
            {
                Tile currentTile = tile as Tile;
                string spriteName = currentTile.sprite.name;
                string newSpriteName = ReplaceSeasonInSpriteName(spriteName, currentSeason);

                Sprite newSprite = System.Array.Find(sprites, sprite => sprite.name == newSpriteName);
                if (newSprite != null)
                {
                    Tile newTile = ScriptableObject.CreateInstance<Tile>();
                    newTile.sprite = newSprite;
                    tilemap.SetTile(pos, newTile);
                }
                else
                {
                    Debug.LogWarning($"Sprite '{newSpriteName}' not found in spritesheet.");
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
