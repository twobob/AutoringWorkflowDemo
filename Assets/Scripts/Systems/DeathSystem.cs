using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class DeathSystem : JobComponentSystem
{
    [RequireComponentTag(typeof(CanDie))]
    struct DeathSystemJob : IJobForEachWithEntity<Health>
    {
        public EntityCommandBuffer.Concurrent cmd;

        public void Execute(Entity entity, int index, ref Health Health)
        {
            if(Health.Value < 0)
            {
                cmd.DestroyEntity(index, entity);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        EntityCommandBufferSystem ecbs = EntityManager.World.GetOrCreateSystem<EntityCommandBufferSystem>();

        var job = new DeathSystemJob()
        {
            cmd = ecbs.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDependencies);

        ecbs.AddJobHandleForProducer(job);

        return job;
    }
}