
using UnityEngine;

public class HeartHazard : Hazard
{
    public int healAmount = 1;
    public AudioClip collectSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                health.Heal(healAmount);
                Debug.Log("Heart collected!");

                if (collectSound != null)
                {
                    AudioSource.PlayClipAtPoint(
                        collectSound,
                        transform.position,
                        soundVolume
                    );
                }
                else
                {
                    Debug.LogWarning(
                        "Heart collect sound is not assigned!"
                    );
                }
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
        if (collectSound != null)
{
    AudioSource.PlayClipAtPoint(
        collectSound,
        transform.position,
        soundVolume
    );
}
else
{
    Debug.LogWarning("Heart collect sound is not assigned!");
}
    }
}
