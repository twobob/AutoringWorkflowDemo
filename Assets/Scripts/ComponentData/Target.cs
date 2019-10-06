using System;
using Unity.Entities;

[Serializable]
public struct Target : IComponentData
{
    public Entity Entity;
}
