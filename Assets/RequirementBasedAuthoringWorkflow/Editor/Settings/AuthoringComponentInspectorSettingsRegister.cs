using System.Collections.Generic;

using UnityEditor;
using UnityEngine.UIElements;
// Register a SettingsProvider using UIElements for the drawing framework:
static class AuthoringComponentInspectorSettingsRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
        var provider = new SettingsProvider("Project/AuthoringComponentInspectorSettings", SettingsScope.Project)
        {
            label = "Authoring Component Inspector Settings",
            // activateHandler is called when the user clicks on the Settings item in the Settings window.
            activateHandler = (searchContext, rootElement) =>
            {
                var settings = AuthoringComponentInspectorSettings.GetSerializedSettings();

               

                Toggle autoSyncVE = new Toggle("Automaticaly sychronize data component")
                {
                    value = settings.FindProperty("autoSync").boolValue,
                    tooltip = "When true, the Authoring Component Inspector will automatically add and remove relevent data components from the inspected GameObject."
                };
                rootElement.Add(autoSyncVE);

                autoSyncVE.RegisterValueChangedCallback(ChangeAutoSync);

                Toggle autoHideVE = new Toggle("Hide/Show data component in the inspector")
                {
                    value = settings.FindProperty("autoHide").boolValue,
                    tooltip = "When true, data components with a hideFlag of None will be set to HideInInspector. When false, data components with a hideFlag of HideInInspector will be set to None."
                };
                rootElement.Add(autoHideVE);

                autoHideVE.RegisterValueChangedCallback(ChangeAutoHide);
            }

        };

        return provider;
    }

    private static void ChangeAutoSync(ChangeEvent<bool> evt)
    {
        AuthoringComponentInspectorSettings.ChangeAutoSync(evt.newValue);
    }

    private static void ChangeAutoHide(ChangeEvent<bool> evt)
    {
        AuthoringComponentInspectorSettings.ChangeAutoHide(evt.newValue);
    }
}
