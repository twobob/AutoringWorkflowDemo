using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class DamageSystem : JobComponentSystem
{
    

 
    struct DamageSystemJob : IJobForEachWithEntity<Damage, Health>
    {
        public EntityCommandBuffer.Concurrent cmd;

        public void Execute(Entity entity, int index, ref Damage Damage, ref Health Health)
        {

            var health = Health.Value;
            health -= Damage.Value;
            Health.Value = health;

            cmd.RemoveComponent<Damage>(index, entity);
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        EntityCommandBufferSystem ecbs = EntityManager.World.GetOrCreateSystem<EntityCommandBufferSystem>();

        var job = new DamageSystemJob()
        {
            cmd = ecbs.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDependencies);

        ecbs.AddJobHandleForProducer(job);

        return job;
    }
}