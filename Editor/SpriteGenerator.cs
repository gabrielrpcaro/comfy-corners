using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class SpriteGenerator : EditorWindow
{
    private List<Texture2D> spriteSheets = new List<Texture2D>();
    private Color color;

    [MenuItem("Tools/Sprite Generator/Gerar Sprite")]
    public static void ShowWindow()
    {
        GetWindow<SpriteGenerator>("Sprite Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Generator", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Sprite Sheets", EditorStyles.boldLabel);

        for (int i = 0; i < spriteSheets.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            spriteSheets[i] = (Texture2D)EditorGUILayout.ObjectField($"Sprite Sheet {i + 1}", spriteSheets[i], typeof(Texture2D), false, GUILayout.Height(150));

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                spriteSheets.RemoveAt(i);
                i--; // Adjust index after removal
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Sprite Sheet"))
        {
            spriteSheets.Add(null);
        }

        if (spriteSheets.Count > 0 && GUILayout.Button("Generate Animations"))
        {
            GenerateSprites();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Color Asset", EditorStyles.boldLabel);

        color = EditorGUILayout.ColorField("Color", color);

        if (GUILayout.Button("Create Color Asset"))
        {
            CreateColorAsset();
        }
        EditorGUILayout.EndVertical();
    }

    private void GenerateSprites()
    {
        foreach (var spriteSheet in spriteSheets)
        {
            if (!ValidateSpriteSheet(spriteSheet)) continue;

            var path = AssetDatabase.GetAssetPath(spriteSheet);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            if (sprites.Length < 4)
            {
                Debug.LogError("The sprite sheet does not contain enough sprites to be divided into four groups.");
                continue;
            }

            CreateAnimationClips(sprites, path);
        }
    }

    private bool ValidateSpriteSheet(Texture2D spriteSheet)
    {
        if (spriteSheet == null)
        {
            Debug.LogError("No sprite sheet selected!");
            return false;
        }

        var path = AssetDatabase.GetAssetPath(spriteSheet);
        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter == null || textureImporter.spriteImportMode != SpriteImportMode.Multiple)
        {
            Debug.LogError("Selected texture is not a sprite sheet with multiple sprites!");
            return false;
        }

        if (!textureImporter.isReadable)
        {
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        return true;
    }

    private void CreateAnimationClips(Sprite[] sprites, string path)
    {
        var baseName = Path.GetFileNameWithoutExtension(path);
        var groupSize = sprites.Length / 4;

        for (int i = 0; i < 4; i++)
        {
            var clip = new AnimationClip { frameRate = 12 };
            var curveBinding = new EditorCurveBinding { type = typeof(SpriteRenderer), path = "", propertyName = "m_Sprite" };
            var keyFrames = baseName.Contains("Sitting") ? new ObjectReferenceKeyframe[2] : new ObjectReferenceKeyframe[groupSize];

            for (int j = 0; j < keyFrames.Length; j++)
            {
                keyFrames[j] = new ObjectReferenceKeyframe
                {
                    time = j / 12f,
                    value = sprites[i * groupSize + j]
                };
            }

            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = !baseName.Contains("Sitting");
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var clipPath = Path.Combine(Path.GetDirectoryName(path), $"{baseName}{GetDirection(i)}.anim");
            AssetDatabase.CreateAsset(clip, clipPath);
        }

        AssetDatabase.SaveAssets();
    }

    private string GetDirection(int index)
    {
        return index switch
        {
            0 => "Up",
            1 => "Left",
            2 => "Down",
            3 => "Right",
            _ => ""
        };
    }

    private void CreateColorAsset()
    {
        if (color == null)
        {
            Debug.LogError("No color provided!");
            return;
        }

        var path = GetSavePath();
        if (string.IsNullOrEmpty(path)) return;

        var colorAsset = ScriptableObject.CreateInstance<ColorAssetGenerated>();
        colorAsset.color = color;

        var assetPath = Path.Combine(path, "ColorAsset.asset");
        var newPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        AssetDatabase.CreateAsset(colorAsset, newPath);
        AssetDatabase.SaveAssets();
    }

    private string GetSavePath()
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No object selected. Please select a folder in the Project Window.");
            path = EditorUtility.OpenFolderPanel("Select Folder to Save Color Asset", "Assets", "");
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No folder selected!");
                return null;
            }
            path = FileUtil.GetProjectRelativePath(path);
        }
        else if (!Directory.Exists(path))
        {
            path = Path.GetDirectoryName(path);
        }
        return path;
    }

    [System.Serializable]
    public class ColorAssetGenerated : ScriptableObject
    {
        public Color color;
    }
}
