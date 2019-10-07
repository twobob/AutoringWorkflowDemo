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
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "sychronize", "Authoring", "Inspector" })
        };

        return provider;
    }

    private static void ChangeAutoSync(ChangeEvent<bool> evt)
    {
        AuthoringComponentInspectorSettings.ChangeAutoSync(evt.newValue);
    }
}
