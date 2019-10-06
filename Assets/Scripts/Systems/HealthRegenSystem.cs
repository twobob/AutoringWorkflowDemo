using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class HealthRegenSystem : JobComponentSystem
{

    [BurstCompile]
    struct HealthRegenJob : IJobForEach<Health, HealthRegen>
    {
        public float deltaTime;

        public void Execute(ref Health Health, ref HealthRegen HealthRegen)
        {
            var health = Health.Value;
            health += HealthRegen.PointPerSec * deltaTime;
            Health.Value = health;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new HealthRegenJob()
        {
            deltaTime = Time.deltaTime
        };
        
        return job.Schedule(this, inputDependencies);
    }
}