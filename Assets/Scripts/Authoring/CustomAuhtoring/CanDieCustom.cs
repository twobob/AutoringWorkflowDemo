using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CanDieCustom : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Health;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Health() { Value = Health });
        dstManager.AddComponent<CanDie>(entity);
    }
}
