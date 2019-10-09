using System.Reflection;
using Unity.Entities;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public  class AuthoringComponentMapping
{
    public TextField tf;
    public ObjectField objectField;
    public IConvertGameObjectToEntity authoringComponent;
    public FieldInfo fieldInfo;
}

