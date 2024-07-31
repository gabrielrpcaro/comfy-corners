#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TimeManager))]
public class TimeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedObject timeManagerObject = new SerializedObject(target);

        // Draw the basic fields
        EditorGUILayout.PropertyField(timeManagerObject.FindProperty("ticksPerSecond"));

        // Draw the initial settings
        SerializedProperty forceStartSettings = timeManagerObject.FindProperty("forceStartSettings");
        EditorGUILayout.PropertyField(forceStartSettings);

        if (forceStartSettings.boolValue)
        {
            EditorGUILayout.PropertyField(timeManagerObject.FindProperty("startSeason"));
            EditorGUILayout.PropertyField(timeManagerObject.FindProperty("startDay"));
            EditorGUILayout.PropertyField(timeManagerObject.FindProperty("startPeriod"));

            SerializedProperty startPeriod = timeManagerObject.FindProperty("startPeriod");

            if (startPeriod.enumValueIndex == (int)TimeManager.StartPeriodOption.None)
            {
                EditorGUILayout.PropertyField(timeManagerObject.FindProperty("startHour"));
                EditorGUILayout.PropertyField(timeManagerObject.FindProperty("startMinute"));
            }
        }

        // Draw the periods
        EditorGUILayout.PropertyField(timeManagerObject.FindProperty("morningPeriod"), true);
        EditorGUILayout.PropertyField(timeManagerObject.FindProperty("afternoonPeriod"), true);
        EditorGUILayout.PropertyField(timeManagerObject.FindProperty("eveningPeriod"), true);
        EditorGUILayout.PropertyField(timeManagerObject.FindProperty("nightPeriod"), true);

        // Draw the events
        EditorGUILayout.PropertyField(timeManagerObject.FindProperty("onDayStart"));
        EditorGUILayout.PropertyField(timeManagerObject.FindProperty("onSeasonChange"));

        timeManagerObject.ApplyModifiedProperties();
    }
}
#endif
