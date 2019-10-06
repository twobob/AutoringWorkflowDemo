using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class MaxHealthAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Value;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MaxHealth() { Value = Value });
    }
}
