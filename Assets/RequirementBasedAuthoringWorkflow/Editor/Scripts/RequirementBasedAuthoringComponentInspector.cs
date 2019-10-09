using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(RequirementBasedAuthoringComponent), true)]
public class RequirementBasedAuthoringComponentInspector : Editor
{

    private List<IConvertGameObjectToEntity> AuthoringComponents = new List<IConvertGameObjectToEntity>();

    private Dictionary<string, AuthoringComponentMapping> TextFieldAuthoringComponentMap = new Dictionary<string, AuthoringComponentMapping>();

    VisualElement root;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement ve = new VisualElement();
        root = ve;
        Refresh();
        return root;
    }

    public void Refresh()
    {
        RequirementBasedAuthoringComponent component = (RequirementBasedAuthoringComponent)target;
        foreach (var requiredType in DataComponentHelper.GetRequieredTypes(component.GetType()))
        {
            Foldout f = new Foldout();
            f.text = requiredType.ToString();
            f.AddDataComponentAttributes((IConvertGameObjectToEntity)component.GetComponent(requiredType));
            root.Add(f);
        }
    }
    

}
