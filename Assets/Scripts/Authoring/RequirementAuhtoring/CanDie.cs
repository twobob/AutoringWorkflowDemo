using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthAuthoring), typeof(CanDieAuthoring))]
[RequireComponent(typeof(Rigidbody))]
public class CanDie : RequirementBasedAuthoringComponent
{
}
