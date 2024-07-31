using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public string playerName;
    public Vector3 playerPosition;
    public string playerScene;
    public bool isPlayerSitting; 

    public List<BodyPartData> bodyPartCustomizations;

    public int currentMinute;
    public int currentHour;
    public int currentDay;
    public int currentSeason;

    [System.Serializable]
    public class BodyPartData
    {
        public PlayerCustomization.BodyPartType partType;
        public string style;
        public string color;
    }

    // The values defined in this constructor will be the default values
    // The game starts with when there's no data to load
    public GameData() 
    {
        this.playerName = "Jogador 1";
        playerPosition = Vector3.zero;
        playerScene = "Principal";
        isPlayerSitting = false;
        bodyPartCustomizations = new List<BodyPartData>();

        currentMinute = 0;
        currentHour = 6;
        currentDay = 1;
        currentSeason = 1;
    }

    public void SetBodyPartCustomization(PlayerCustomization.BodyPartType partType, string style, string color)
    {
        var existingPart = bodyPartCustomizations.Find(part => part.partType == partType);
        if (existingPart != null)
        {
            existingPart.style = style;
            existingPart.color = color;
        }
        else
        {
            bodyPartCustomizations.Add(new BodyPartData { partType = partType, style = style, color = color });
        }
    }

    public BodyPartData GetBodyPartCustomization(PlayerCustomization.BodyPartType partType)
    {
        return bodyPartCustomizations.Find(part => part.partType == partType);
    }
}