using UnityEngine;

[CreateAssetMenu(
    fileName = "ShooterStats",
    menuName = "Chromeheart/Data/Shooter Stats")]
public class ShooterStats : ScriptableObject
{
    [Header("Detection")]
    public float detectionRange;

    [Range(0f, 180f)]
    public float viewAngle;

    [Header("Shooting")]
    [Range(0.3f, 0.8f)]
    public float aimTime;

    public float cooldown;
    public float shootAnimationTime;
}