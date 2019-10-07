using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
 using UnityEngine;
 [InitializeOnLoad]
class DataComponentModifier
{
    private static bool AutoSync { get => AuthoringComponentInspectorSettings.GetSerializedSettings().FindProperty("autoSync").boolValue; }
    private static bool AutoHide { get => AuthoringComponentInspectorSettings.GetSerializedSettings().FindProperty("autoHide").boolValue; }

    static DataComponentModifier()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (AutoSync)
        {
            Synchronize();
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

    internal static void Synchronize()
    {

        var activeGo = Selection.activeGameObject;
        if (activeGo == null) return;

        // Get the list of required data components
        List<Type> RequiredAuthoringComponentsTypes = new List<Type>();
        var components = activeGo.GetComponents(typeof(MonoBehaviour));
        foreach (var mono in components)
        {
            Type requieringType = mono.GetType();
            foreach (var requiredAttribute in RequireComponent.GetCustomAttributes(requieringType, typeof(RequireComponent)))
            {
                Type requiredType = ((RequireComponent)requiredAttribute).m_Type0;
                if (requiredType != null) RegisterRequierment(RequiredAuthoringComponentsTypes, requieringType, requiredAttribute, requiredType);
                requiredType = ((RequireComponent)requiredAttribute).m_Type1;
                if (requiredType != null) RegisterRequierment(RequiredAuthoringComponentsTypes, requieringType, requiredAttribute, requiredType);
                requiredType = ((RequireComponent)requiredAttribute).m_Type2;
                if (requiredType != null) RegisterRequierment(RequiredAuthoringComponentsTypes, requieringType, requiredAttribute, requiredType);
            }
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

    private static void RegisterRequierment(List<Type> RequiredAuthoringComponentsTypes, Type requieringType, Attribute requiredAttribute, Type requiredType)
    {
        if (((RequireComponent)requiredAttribute).m_Type0 != null)
        {
            RequiredAuthoringComponentsTypes.Add(requiredType);
        }
    }



}