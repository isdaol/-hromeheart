using UnityEngine;

[CreateAssetMenu(
    fileName = "ChargerStats",
    menuName = "Chromeheart/Data/Charger Stats"
)]
public class ChargerStats : ScriptableObject
{
    [Header("Detection")]
    [Min(0f)] public float detectionRange = 8f;
    [Range(0f, 180f)] public float viewAngle = 60f;

    [Header("Attack")]
    [Range(0.3f, 0.8f)] public float windUpTime = 0.5f;
    [Min(0f)] public float chargeSpeed = 9f;
    [Min(0f)] public float chargeDuration = 0.9f;
    [Min(0f)] public float cooldownTime = 1.5f;
    [Min(0)] public int attackDamage = 1;

    [Header("Return")]
    [Min(0f)] public float returnSpeed = 2f;
    [Min(0f)] public float returnDistance = 0.15f;

    [Header("Idle Look")]
    [Min(0f)] public float turnInterval = 3f;

    [Header("Path Safety")]
    [Min(0.01f)] public float groundPathSampleStep = 0.2f;
    [Min(0f)] public float groundHeightTolerance = 0.45f;
    [Min(0f)] public float groundLookAheadDistance = 0.65f;
    [Min(0f)] public float edgeClearance = 0.1f;
    [Min(0f)] public float collisionPadding = 0.05f;
    [Min(0f)] public float minimumMovementThreshold = 0.01f;
    [Range(0f, 1f)] public float wallNormalThreshold = 0.5f;
}