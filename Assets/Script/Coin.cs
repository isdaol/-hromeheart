using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip coinSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (coinSound != null)
            {
                Debug.Log("Звук монетки запускается!");
                GameSounds.PlayGameplayEffect(coinSound);
            }

            Destroy(gameObject);
        }
    }
}
