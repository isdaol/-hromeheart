using UnityEngine;

public class HeartHazard : Hazard
{
    public AudioClip collectSound;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                health.Heal(stats.healAmount);
                Debug.Log("Heart collected!");

                if (collectSound != null)
                {
                    GameSounds.PlayEffectAt(
                        collectSound,
                        transform.position,
                        stats.soundVolume
                    );
                }
                else
                {
                    Debug.LogWarning("Heart collect sound is not assigned!");
                }
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Bullet"))
            Destroy(other.gameObject);
    }
}
