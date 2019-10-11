using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualeElementExtensions
{
    public static void AddDataComponentAttributes(this Foldout ve, IConvertGameObjectToEntity authoringComponent)
    {

        foreach (MemberInfo prop in authoringComponent.GetType().GetTypeInfo().GetMembers())
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            if (prop.DeclaringType.Equals(authoringComponent.GetType()) && MemberTypes.Field.Equals(prop.MemberType))
            {
                if (!properties.ContainsKey(prop.Name))
                {
                    FieldInfo fi = authoringComponent.GetType().GetField(prop.Name);
                    if (fi.FieldType != typeof(GameObject))
                    {
                        DataComponentAttributeTextField dcatf = new DataComponentAttributeTextField(fi, authoringComponent, prop.Name);
                        ve.Add(dcatf);
                        DataComponentHelper.AddAttributeMapping(fi, dcatf, ve.text);
                    }
                    else
                    {
                        DataComponentAttributeObjectField dcaof = new DataComponentAttributeObjectField(fi, authoringComponent, prop.Name);
                        ve.Add(dcaof);
                        DataComponentHelper.AddAttributeMapping(fi, dcaof,ve.text);
                    }
                }
            }
        }
    }

}