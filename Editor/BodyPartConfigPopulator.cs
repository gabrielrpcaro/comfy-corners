using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class BodyPartConfigPopulator : MonoBehaviour
{
    [MenuItem("Tools/Populate Lookinhos")]
    public static void PopulateBodyPartConfig()
    {
        // Path to the BodyPartConfig asset
        string assetPath = "Assets/Resources/Objects/Lookinhos.asset";
        
        // Create or load the BodyPartConfig asset
        BodyPartConfig config = AssetDatabase.LoadAssetAtPath<BodyPartConfig>(assetPath);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<BodyPartConfig>();
            AssetDatabase.CreateAsset(config, assetPath);
        }

        // Clear existing body part configurations
        config.bodyParts.Clear();

        // Path to the resources folder
        string resourcesPath = Path.Combine(Application.dataPath, "Resources", "Sprites", "Player");

         if (!Directory.Exists(resourcesPath))
        {
            Debug.LogError("Resources path does not exist: " + resourcesPath);
            return;
        }

        // Iterate over each body part type
        foreach (PlayerCustomization.BodyPartType bodyPartType in System.Enum.GetValues(typeof(PlayerCustomization.BodyPartType)))
        {
            string bodyPartPath = Path.Combine(resourcesPath, bodyPartType.ToString());

            if (Directory.Exists(bodyPartPath))
            {
                BodyPartConfig.BodyPart bodyPartConfig = new BodyPartConfig.BodyPart
                {
                    bodyPartType = bodyPartType,
                    styles = new List<BodyPartConfig.BodyPartStyle>()
                };

                // Iterate over each style in the body part folder
                foreach (string stylePath in Directory.GetDirectories(bodyPartPath))
                {
                    string styleName = new DirectoryInfo(stylePath).Name;
                    BodyPartConfig.BodyPartStyle styleConfig = new BodyPartConfig.BodyPartStyle
                    {
                        styleName = styleName,
                        colors = new List<string>(),
                        dominantColorsHex = new List<string>()
                    };

                    // Iterate over each color in the style folder
                    foreach (string colorPath in Directory.GetDirectories(stylePath))
                    {
                        string colorName = new DirectoryInfo(colorPath).Name;
                        styleConfig.colors.Add(colorName);
                        string spritePath = Path.Combine("Assets", "Resources", "Sprites", "Player", bodyPartType.ToString(), styleName, colorName, "static.asset");
                        Sprite staticSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

                        if (staticSprite != null)
                        {
                            Texture2D texture = staticSprite.texture;

                            // Ensure the texture is readable
                            string texturePath = AssetDatabase.GetAssetPath(texture);
                            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                            bool wasReadable = textureImporter.isReadable;
                            if (!wasReadable)
                            {
                                textureImporter.isReadable = true;
                                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                            }

                            string dominantColorHex = GetDominantColorHex(texture, staticSprite.rect);
                            styleConfig.dominantColorsHex.Add("#" + dominantColorHex);

                            // Revert the texture to its original state
                            if (!wasReadable)
                            {
                                textureImporter.isReadable = false;
                                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                            }
                        }
                        else
                        {
                            styleConfig.dominantColorsHex.Add("#FFFFFF"); // Default to white if no sprite is found
                        }

                    }

                    bodyPartConfig.styles.Add(styleConfig);
                }

                config.bodyParts.Add(bodyPartConfig);
            }
        }

        // Save the updated BodyPartConfig asset
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BodyPartConfig asset has been populated.");
    }

    private static string GetDominantColorHex(Texture2D texture, Rect rect)
    {
        int width = (int)rect.width;
        int height = (int)rect.height;
        Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, width, height);
        Dictionary<Color32, int> colorFrequency = new Dictionary<Color32, int>();

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Color32 color = pixels[x + y * width];
                if (color.a > 0)
                {
                    if (colorFrequency.ContainsKey(color))
                    {
                        colorFrequency[color]++;
                    }
                    else
                    {
                        colorFrequency[color] = 1;
                    }
                }
            }
        }

        if (colorFrequency.Count == 0)
        {
            Debug.LogError("No valid pixels found in the specified rect.");
            return "FFFFFF"; // Return white if no valid pixels found
        }

        Color32 dominantColor = colorFrequency.OrderByDescending(kvp => kvp.Value).First().Key;
        return ColorUtility.ToHtmlStringRGB(dominantColor);
    }
}