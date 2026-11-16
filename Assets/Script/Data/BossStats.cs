using UnityEngine;

[CreateAssetMenu(
    fileName = "BossStats",
    menuName = "Chromeheart/Data/Boss Stats")]
public class BossStats : ScriptableObject
{
    [Header("Boss")]
    public int maxHealth;
    public float fireInterval;

    [Header("Projectile")]
    public float projectileSpeed;
    public int projectileDamage;
    public float projectileLifetime;
}