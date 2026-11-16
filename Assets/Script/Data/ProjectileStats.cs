using UnityEngine;

[CreateAssetMenu(
    fileName = "ProjectileStats",
    menuName = "Chromeheart/Data/Projectile Stats")]
public class ProjectileStats : ScriptableObject
{
    public float speed;
    public float lifetime;
    public int damage;
}