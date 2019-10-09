using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
 using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
class DataComponentHelper
{
    private static bool AutoSync { get => AuthoringComponentInspectorSettings.GetSerializedSettings().FindProperty("autoSync").boolValue; }
    private static bool AutoHide { get => AuthoringComponentInspectorSettings.GetSerializedSettings().FindProperty("autoHide").boolValue; }

    static DataComponentHelper()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (AutoSync)
        {
            ActualizeRequieredDataComponentsOnGameObject();
        }
        HideOrShowCOmponentData();
        
    }

    private static void HideOrShowCOmponentData()
    {
        var activeGo = Selection.activeGameObject;
        if (activeGo == null) return;

        foreach (var comp in activeGo.GetComponents(typeof(IConvertGameObjectToEntity)))
        {
            if(AutoHide && HideFlags.None.Equals(comp.hideFlags))
            {
                comp.hideFlags = HideFlags.HideInInspector;
            }
            if (!AutoHide && HideFlags.HideInInspector.Equals(comp.hideFlags))
            {
                comp.hideFlags = HideFlags.None;
            }
        }
    }

    internal static void ActualizeRequieredDataComponentsOnGameObject()
    {

        var activeGo = Selection.activeGameObject;
        if (activeGo == null) return;

        // Get the list of required data components
        List<Type> RequiredAuthoringComponentsTypes = new List<Type>();
        var components = activeGo.GetComponents(typeof(MonoBehaviour));
        foreach (var mono in components)
        {
            Type requieringType = mono.GetType();
            RequiredAuthoringComponentsTypes.AddRange(GetRequieredTypes(requieringType));
        }

        // remove un necessary components
        var existingDataComponents = new List<Type>();
        foreach (var dataComponent in activeGo.GetComponents(typeof(IConvertGameObjectToEntity)))
        {
            if (!RequiredAuthoringComponentsTypes.Contains(dataComponent.GetType()))
            {
                EditorWindow.DestroyImmediate(activeGo.GetComponent(dataComponent.GetType()));
            }
            else
            {
                existingDataComponents.Add(dataComponent.GetType());
            }
        }

        // Add missing required data components
        foreach (var requiredComponent in RequiredAuthoringComponentsTypes)
        {

            if (!existingDataComponents.Contains(requiredComponent))
            {
                activeGo.AddComponent(requiredComponent);
            }

        }
    }

    internal static List<Type> GetRequieredTypes(Type requieringType)
    {
        List<Type> typesRequiredByComponent = new List<Type>();

        foreach (var requiredAttribute in Attribute.GetCustomAttributes(requieringType, typeof(RequireComponent)))
        {
            RequireComponent requiredType = (RequireComponent)requiredAttribute;
            if (requiredType.m_Type0 != null) typesRequiredByComponent.Add(requiredType.m_Type0);
            if (requiredType.m_Type1 != null) typesRequiredByComponent.Add(requiredType.m_Type1);
            if (requiredType.m_Type2 != null) typesRequiredByComponent.Add(requiredType.m_Type2);
        }
        return typesRequiredByComponent;
    }




}