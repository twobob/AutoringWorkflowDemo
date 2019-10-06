using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class RegenCustom : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Regen;
    public int Health;
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new HealthRegen() { PointPerSec = Regen });
        dstManager.AddComponentData(entity, new Health() { Value = Health });
    }
}
