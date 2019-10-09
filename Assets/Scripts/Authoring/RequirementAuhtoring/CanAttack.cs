using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(TargetAuthoring))]
[RequireComponent(typeof(AttackAuthoring))]
public class CanAttack : RequirementBasedAuthoringComponent
{
    public int val;
}
