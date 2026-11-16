using UnityEngine;

[CreateAssetMenu(
    fileName = "HazardStats",
    menuName = "Chromeheart/Data/Hazard Stats")]
public class HazardStats : ScriptableObject
{
    [Header("Common")]
    public float fallSpeed;
    public int damage;

    [Header("Bomb")]
    public float explosionDelay;
    public float explosionRadius;
    public int explosionDamage;

    [Header("Heart")]
    public int healAmount;

    [Range(0f, 1f)]
    public float soundVolume;
}