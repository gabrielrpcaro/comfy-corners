using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpriteSlicer : EditorWindow
{
    [MenuItem("Tools/Sprite Slicer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSlicer>("Sprite Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Sprite Sheets or Folders in Project Window and Click Slice", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Slice Sprites", GUILayout.Height(30)))
        {
            SliceSelectedSprites();
        }
    }

    private void SliceSelectedSprites()
    {
        var selectedObjects = Selection.objects;
        List<string> spriteSheetPaths = new List<string>();

        foreach (var obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (Directory.Exists(path))
            {
                // If the selected object is a folder, get all images in it
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                          .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg"))
                                          .ToArray();
                spriteSheetPaths.AddRange(files);
            }
            else if (obj is Texture2D)
            {
                spriteSheetPaths.Add(path);
            }
        }

        foreach (string path in spriteSheetPaths)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null) continue;

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.isReadable = true; // Set the texture to be readable
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.spritePixelsPerUnit = 32;

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                int textureWidth = texture.width;
                int textureHeight = texture.height;
                var spriteRects = new List<SpriteRect>();
                int index = 0;

                for (int y = textureHeight; y > 0; y -= 64)
                {
                    for (int x = 0; x < textureWidth; x += 64)
                    {
                        var spriteRect = new SpriteRect
                        {
                            name = texture.name + "_" + index,
                            rect = new Rect(x, y - 64, 64, 64),
                            alignment = (int)SpriteAlignment.Center,
                            pivot = new Vector2(0.5f, 0.5f)
                        };
                        spriteRects.Add(spriteRect);

                        if (texture.name == "Idle" && spriteRect.name == "Idle_6")
                        {
                            CreateStaticCopy(spriteRect, path);
                        }

                        index++;
                    }
                }

                ApplySpriteRects(textureImporter, spriteRects.ToArray());
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                // Generate animations
                var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
                CreateAnimationClips(sprites, path);
            }
        }
    }

    private void ApplySpriteRects(TextureImporter textureImporter, SpriteRect[] spriteRects)
    {
        var serializedObject = new SerializedObject(textureImporter);
        var spritesProperty = serializedObject.FindProperty("m_SpriteSheet.m_Sprites");

        spritesProperty.ClearArray();
        spritesProperty.arraySize = spriteRects.Length;

        for (int i = 0; i < spriteRects.Length; i++)
        {
            var spriteRectProperty = spritesProperty.GetArrayElementAtIndex(i);
            spriteRectProperty.FindPropertyRelative("m_Rect").rectValue = spriteRects[i].rect;
            spriteRectProperty.FindPropertyRelative("m_Name").stringValue = spriteRects[i].name;
            spriteRectProperty.FindPropertyRelative("m_Alignment").intValue = (int)spriteRects[i].alignment;
            spriteRectProperty.FindPropertyRelative("m_Pivot").vector2Value = spriteRects[i].pivot;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateStaticCopy(SpriteRect spriteRect, string spriteSheetPath)
    {
        string folderPath = Path.GetDirectoryName(spriteSheetPath);
        string destinationPath = Path.Combine(folderPath, "static.asset");

        Sprite staticSprite = Sprite.Create(
            (Texture2D)AssetDatabase.LoadAssetAtPath(spriteSheetPath, typeof(Texture2D)),
            spriteRect.rect,
            spriteRect.pivot,
            32
        );
        staticSprite.name = "static";

        AssetDatabase.CreateAsset(staticSprite, destinationPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
}
