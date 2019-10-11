using System;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DataComponentAttributeField : VisualElement
{
    public abstract void Refresh();
}


public class DataComponentAttributeTextField : DataComponentAttributeField
{
    FieldInfo targetField;
    IConvertGameObjectToEntity dataComponent;
    TextField tf;

    public DataComponentAttributeTextField(FieldInfo targetField, IConvertGameObjectToEntity dataComponent, string label)
    {
        this.targetField = targetField;
        this.dataComponent = dataComponent;
        
        tf = new TextField(label);
        tf.SetValueWithoutNotify(targetField.GetValue(dataComponent).ToString());

        tf.tooltip = targetField.FieldType.ToString();
        tf.RegisterValueChangedCallback(ChangeDataComponentAttribute);

        Add(tf);
    }

    public override void Refresh()
    {
        tf.SetValueWithoutNotify(targetField.GetValue(dataComponent).ToString());
    }

    private void ChangeDataComponentAttribute(ChangeEvent<string> evt)
    {
        try
        {
            targetField.SetValue(dataComponent, Convert.ChangeType(evt.newValue, targetField.FieldType));
            DataComponentHelper.Refresh(targetField);
        }
#pragma warning disable 168
        catch (Exception e)
#pragma warning restore 168
        {
            ((TextField)evt.target).value = evt.previousValue;
        }

    }
}

