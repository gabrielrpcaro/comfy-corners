using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BodyPartConfig", menuName = "CharacterCustomization/BodyPartConfig")]
public class BodyPartConfig : ScriptableObject
{
    [System.Serializable]
    public class BodyPartStyle
    {
        public string styleName;
        public List<string> colors;
        public List<string> dominantColorsHex;
    }

    [System.Serializable]
    public class BodyPart
    {
        public PlayerCustomization.BodyPartType bodyPartType;
        public List<BodyPartStyle> styles;
    }

    public List<BodyPart> bodyParts;
}
