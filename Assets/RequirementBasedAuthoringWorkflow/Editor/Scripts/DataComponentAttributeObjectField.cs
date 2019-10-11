using System;
using System.Reflection;
using Unity.Entities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DataComponentAttributeObjectField : DataComponentAttributeField
{
    FieldInfo targetField;
    IConvertGameObjectToEntity dataComponent;
    ObjectField of;

    public DataComponentAttributeObjectField(FieldInfo targetField, IConvertGameObjectToEntity dataComponent, string label)
    {
        this.targetField = targetField;
        this.dataComponent = dataComponent;

        of = new ObjectField(label);
        of.objectType = typeof(GameObject);

        of.SetValueWithoutNotify((GameObject)targetField.GetValue(dataComponent));
        of.RegisterValueChangedCallback(ChangeDataComponentAttribute);
        
        Add(of);
    }

    public override void Refresh()
    {
        of.SetValueWithoutNotify((GameObject)targetField.GetValue(dataComponent));
    }

    private void ChangeDataComponentAttribute(ChangeEvent<UnityEngine.Object> evt)
    {
        targetField.SetValue(dataComponent, Convert.ChangeType(evt.newValue, targetField.FieldType));
        DataComponentHelper.Refresh(targetField);
    }

}

