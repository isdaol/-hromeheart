using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerStats",
    menuName = "Chromeheart/Data/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;

    [Header("Shooting")]
    public float shootCooldown;
    public float bulletOffset;

    [Header("Health")]
    public int maxHealth;
    public float invincibilityDuration;
    public float blinkInterval;
}