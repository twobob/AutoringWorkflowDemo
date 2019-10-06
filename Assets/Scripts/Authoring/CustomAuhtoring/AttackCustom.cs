using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class AttackCustom : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int Attack;
    public GameObject Entity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Target() { Entity = conversionSystem.GetPrimaryEntity(Entity) });
        dstManager.AddComponentData(entity, new Attack() { Value = Attack });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Entity);
    }
}
