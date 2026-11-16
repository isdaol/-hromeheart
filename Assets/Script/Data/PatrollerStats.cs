using UnityEngine;

[CreateAssetMenu(
    fileName = "PatrollerStats",
    menuName = "Chromeheart/Data/Patroller Stats")]
public class PatrollerStats : ScriptableObject
{
    [Header("Movement")]
    [Min(0f)] public float patrolSpeed;
    [Min(0f)] public float chaseSpeed;
    [Min(0f)] public float patrolWaitTime;
    [Min(0f)] public float waypointArrivalTolerance;

    [Header("Detection")]
    [Min(0f)] public float detectionRange;
    [Min(0f)] public float visionOriginOffset;

    [Header("Attack")]
    [Min(0f)] public float attackRange;

    [Range(0.3f, 0.8f)]
    public float windUpTime;

    [Min(0f)] public float attackCooldown;
    [Min(0)] public int attackDamage;
}