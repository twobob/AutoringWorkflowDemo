using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthAuthoring), typeof(HealthRegenAuthoring))]
public class HasPassiveHealthRegen : RequirementBasedAuthoringComponent
{
}
