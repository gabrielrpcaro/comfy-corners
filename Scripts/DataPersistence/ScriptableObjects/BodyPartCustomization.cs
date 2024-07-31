using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BodyPartCustomization", menuName = "CharacterCustomization/BodyPartCustomization")]
public class BodyPartCustomization : ScriptableObject
{
    [System.Serializable]
    public class BodyPartData
    {
        public PlayerCustomization.BodyPartType partType;
        public string style;
        public string color;
    }

    public List<BodyPartData> bodyParts = new List<BodyPartData>();
    public BodyPartConfig config;

    public void InitializeFromConfig()
    {
        if (config == null)
        {
            Debug.LogError("BodyPartConfig is not assigned.");
            return;
        }

        bodyParts.Clear();
        foreach (var bodyPart in config.bodyParts)
        {
            bodyParts.Add(new BodyPartData
            {
                partType = bodyPart.bodyPartType,
                style = bodyPart.styles.Count > 0 ? bodyPart.styles[0].styleName : "",
                color = bodyPart.styles.Count > 0 && bodyPart.styles[0].colors.Count > 0 ? bodyPart.styles[0].colors[0] : ""
            });
        }
    }
}