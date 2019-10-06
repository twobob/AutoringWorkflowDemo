using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class AttackSystem : JobComponentSystem
{
    
    struct DamageSystemJob : IJobForEachWithEntity<Attack, Target>
    {
        public EntityCommandBuffer.Concurrent cmd;
        public bool Clicked;

        public void Execute(Entity entity, int index, ref Attack Attack, ref Target Target)
        {
            if (Clicked)
            {
                Damage damage = new Damage()
                {
                    Value = Attack.Value
                };
                cmd.AddComponent(index, Target.Entity, damage);
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        EntityCommandBufferSystem ecbs = EntityManager.World.GetOrCreateSystem<EntityCommandBufferSystem>();

        var job = new DamageSystemJob()
        {
            cmd = ecbs.CreateCommandBuffer().ToConcurrent(),
            Clicked = Input.GetMouseButtonDown(0)
        }.Schedule(this, inputDependencies);

        ecbs.AddJobHandleForProducer(job);

        return job;
    }
}