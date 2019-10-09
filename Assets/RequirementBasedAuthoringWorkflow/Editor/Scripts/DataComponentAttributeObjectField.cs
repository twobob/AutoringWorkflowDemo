using System;
using System.Reflection;
using Unity.Entities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DataComponentAttributeObjectField : VisualElement
{
    FieldInfo targetField;
    IConvertGameObjectToEntity dataComponent;
    ObjectField of;

    public DataComponentAttributeObjectField(FieldInfo targetField, IConvertGameObjectToEntity dataComponent, string label)
    {
        this.targetField = targetField;
        this.dataComponent = dataComponent;

        ObjectField of = new ObjectField(label);
        of.objectType = typeof(GameObject);
        of.SetValueWithoutNotify((GameObject)targetField.GetValue(dataComponent));
        of.RegisterValueChangedCallback(ChangeDataComponentAttribute);

        this.of = of;

        Add(of);
    }

    private void ChangeDataComponentAttribute(ChangeEvent<UnityEngine.Object> evt)
    {
        var ofid = ((ObjectField)evt.target).name;
        targetField.SetValue(dataComponent, Convert.ChangeType(evt.newValue, targetField.FieldType));
    }

}

