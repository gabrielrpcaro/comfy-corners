using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BodyPartCustomization))]
public class BodyPartCustomizationEditor : Editor
{
    private SerializedProperty configProp;
    private SerializedProperty bodyPartsProp;

    private void OnEnable()
    {
        configProp = serializedObject.FindProperty("config");
        bodyPartsProp = serializedObject.FindProperty("bodyParts");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(configProp);

        if (configProp.objectReferenceValue != null)
        {
            BodyPartCustomization bodyPartCustomization = (BodyPartCustomization)target;
            BodyPartConfig config = (BodyPartConfig)configProp.objectReferenceValue;

            if (GUILayout.Button("Initialize from Config"))
            {
                bodyPartCustomization.InitializeFromConfig();
                EditorUtility.SetDirty(bodyPartCustomization);
                AssetDatabase.SaveAssets();
            }

            for (int i = 0; i < bodyPartsProp.arraySize; i++)
            {
                SerializedProperty bodyPartData = bodyPartsProp.GetArrayElementAtIndex(i);
                SerializedProperty partTypeProp = bodyPartData.FindPropertyRelative("partType");
                SerializedProperty styleProp = bodyPartData.FindPropertyRelative("style");
                SerializedProperty colorProp = bodyPartData.FindPropertyRelative("color");

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(partTypeProp.enumDisplayNames[partTypeProp.enumValueIndex], EditorStyles.boldLabel);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(partTypeProp, new GUIContent("Part Type"));
                EditorGUI.EndDisabledGroup();

                var bodyPartConfig = config.bodyParts.Find(bp => bp.bodyPartType == (PlayerCustomization.BodyPartType)partTypeProp.enumValueIndex);

                if (bodyPartConfig != null)
                {
                    string[] styleOptions = bodyPartConfig.styles.ConvertAll(style => style.styleName).ToArray();
                    int selectedStyleIndex = Mathf.Max(0, System.Array.IndexOf(styleOptions, styleProp.stringValue));
                    selectedStyleIndex = EditorGUILayout.Popup("Style", selectedStyleIndex, styleOptions);
                    styleProp.stringValue = styleOptions[selectedStyleIndex];

                    var selectedStyle = bodyPartConfig.styles.Find(style => style.styleName == styleProp.stringValue);
                    if (selectedStyle != null)
                    {
                        string[] colorOptions = selectedStyle.colors.ToArray();
                        int selectedColorIndex = Mathf.Max(0, System.Array.IndexOf(colorOptions, colorProp.stringValue));
                        selectedColorIndex = EditorGUILayout.Popup("Color", selectedColorIndex, colorOptions);
                        colorProp.stringValue = colorOptions[selectedColorIndex];
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}