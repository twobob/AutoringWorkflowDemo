using UnityEditor;
using UnityEngine;

// Create a new type of Settings Asset.
class AuthoringComponentInspectorSettings : ScriptableObject
{
    public const string k_MyCustomSettingsPath = "Assets/Editor/Settings/AuthoringComponentInspectorSettings.asset";

    [SerializeField]
    private bool autoSync;

    [SerializeField]
    private bool autoHide;

    internal static AuthoringComponentInspectorSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<AuthoringComponentInspectorSettings>(k_MyCustomSettingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<AuthoringComponentInspectorSettings>();
            settings.autoSync = false;
            settings.autoHide = false;
            AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static void ChangeAutoSync(bool newValue)
    {
        AssetDatabase.LoadAssetAtPath<AuthoringComponentInspectorSettings>(k_MyCustomSettingsPath).autoSync = newValue;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    internal static void ChangeAutoHide(bool newValue)
    {
        AssetDatabase.LoadAssetAtPath<AuthoringComponentInspectorSettings>(k_MyCustomSettingsPath).autoHide = newValue;
    }
}
