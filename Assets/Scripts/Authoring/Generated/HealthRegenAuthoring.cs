using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class HealthRegenAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float PointPerSec;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new HealthRegen() { PointPerSec = PointPerSec });
    }
}
