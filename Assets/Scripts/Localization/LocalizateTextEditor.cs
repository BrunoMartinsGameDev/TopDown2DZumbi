#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // Use Newtonsoft.Json (Json.NET) para parsing fácil

[CustomEditor(typeof(LocalizateText))]
public class LocalizateTextEditor : Editor
{
    private List<string> keys = new();
    private int selectedIndex = 0;

    void OnEnable()
    {
        // Caminho do JSON (ajuste se necessário)
        string path = "Assets/StreamingAssets/localization.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JObject obj = JObject.Parse(json);
            keys = new List<string>();
            foreach (var prop in obj.Properties())
                keys.Add(prop.Name);
        }
    }

    public override void OnInspectorGUI()
    {
        LocalizateText script = (LocalizateText)target;

        if (keys.Count > 0)
        {
            selectedIndex = Mathf.Max(0, keys.IndexOf(script.key));
            selectedIndex = EditorGUILayout.Popup("Key", selectedIndex, keys.ToArray());
            script.key = keys[selectedIndex];
        }
        else
        {
            script.key = EditorGUILayout.TextField("Key", script.key);
        }

        DrawDefaultInspector();
    }
}
#endif