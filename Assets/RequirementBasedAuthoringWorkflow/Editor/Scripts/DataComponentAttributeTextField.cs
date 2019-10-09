using System;
using System.Reflection;
using Unity.Entities;
using UnityEngine.UIElements;

public class DataComponentAttributeTextField : VisualElement
{
    FieldInfo targetField;
    IConvertGameObjectToEntity dataComponent;

    public DataComponentAttributeTextField(FieldInfo targetField, IConvertGameObjectToEntity dataComponent, string label)
    {
        this.targetField = targetField;
        this.dataComponent = dataComponent;

        TextField tf = new TextField(label);

        tf.SetValueWithoutNotify(targetField.GetValue(dataComponent).ToString());

        tf.tooltip = targetField.FieldType.ToString();


        tf.RegisterValueChangedCallback(ChangeDataComponentAttribute);

        Add(tf);
    }


    private void ChangeDataComponentAttribute(ChangeEvent<string> evt)
    {
        try
        {
            targetField.SetValue(dataComponent, Convert.ChangeType(evt.newValue, targetField.FieldType));
        }
#pragma warning disable 168
        catch (Exception e)
#pragma warning restore 168
        {
            ((TextField)evt.target).value = evt.previousValue;
        }

    }
}

