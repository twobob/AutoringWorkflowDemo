using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
 using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
class DataComponentHelper
{
    private static bool AutoSync { get => AuthoringComponentInspectorSettings.GetSerializedSettings().FindProperty("autoSync").boolValue; }
    private static bool AutoHide { get => AuthoringComponentInspectorSettings.GetSerializedSettings().FindProperty("autoHide").boolValue; }

    private  static Dictionary<FieldInfo,HashSet<DataComponentAttributeField>> AtrtributeVisualElementMap = new Dictionary<FieldInfo, HashSet<DataComponentAttributeField>>();

    public static void AddAttributeMapping(FieldInfo fi, DataComponentAttributeField ve,string name)
    {
        if (!AtrtributeVisualElementMap.ContainsKey(fi))
        {
            AtrtributeVisualElementMap[fi] = new HashSet<DataComponentAttributeField>() { ve };
        }
        else
        {
            AtrtributeVisualElementMap[fi].Add(ve);
        }
        Debug.Log($"Added {name} for {fi.Name}");
    }

    static DataComponentHelper()
    {
        EditorApplication.update += Update;
    }

    internal static void Refresh(FieldInfo targetField)
    {

        Debug.Log($"Refresh {targetField.Name}");
        foreach (DataComponentAttributeField ve in AtrtributeVisualElementMap[targetField])
        {
            ve.Refresh();
        }
    }

    static void Update()
    {
        if (AutoSync)
        {
            ActualizeRequieredDataComponentsOnGameObject();
        }
        HideOrShowCOmponentData();        
    }

    /// <summary>
    /// Will hide or show the currently selected gameobject's component implementing the IConvertGameObjectToEntity interface.
    /// </summary>
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

    /// <summary>
    /// Will add to the game object  missing requiered component implementing the IConvertGameObjectToEntity interface.
    /// Will remove from the game object component implementing the IConvertGameObjectToEntity interface that are no onger required.
    /// </summary>
    internal static void ActualizeRequieredDataComponentsOnGameObject()
    {

        var activeGo = Selection.activeGameObject;
        if (activeGo == null) return;
        AtrtributeVisualElementMap.Clear();
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

    /// <summary>
    /// List all the required types for a given component type.
    /// </summary>
    /// <param name="requieringType"></param>
    /// <returns></returns>
    internal static List<Type> GetRequieredTypes(Type requieringType)
    {
        List<Type> typesRequiredByComponent = new List<Type>();

        foreach (var requiredAttribute in Attribute.GetCustomAttributes(requieringType, typeof(RequireComponent)))
        {
            RequireComponent requiredType = (RequireComponent)requiredAttribute;
            if (requiredType.m_Type0 != null && typeof(IConvertGameObjectToEntity).IsAssignableFrom(requiredType.m_Type0)) typesRequiredByComponent.Add(requiredType.m_Type0);
            if (requiredType.m_Type1 != null && typeof(IConvertGameObjectToEntity).IsAssignableFrom(requiredType.m_Type1)) typesRequiredByComponent.Add(requiredType.m_Type1);
            if (requiredType.m_Type2 != null && typeof(IConvertGameObjectToEntity).IsAssignableFrom(requiredType.m_Type2)) typesRequiredByComponent.Add(requiredType.m_Type2);
        }
        return typesRequiredByComponent;
    }




}