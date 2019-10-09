using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthAuthoring), typeof(CanDieAuthoring))]
public class CanDie : RequirementBasedAuthoringComponent
{
}
